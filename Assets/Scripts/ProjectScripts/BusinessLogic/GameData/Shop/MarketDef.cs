using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MarketRandomType
{
    Base,
    BasePiecesEasy,
    BasePiecesHard,
    Ingredients,
    BaseСhests,
}

public class MarketDef
{
    public string Uid;
    public int Amount;
    public MarketRandomType RandomType;
    public CurrencyPair Price;
    public ItemWeight Weight;
}

public class MarketDefParrent
{
    public string Uid;
    
    private List<MarketDef> defs = new List<MarketDef>();
    private List<ItemWeight> weights = new List<ItemWeight>();

    public void AddDef(MarketDef def)
    {
        defs.Add(def);
        weights.Add(def.Weight);
    }
    
    public MarketDef GetDef()
    {
        var index = ItemWeight.GetRandomItemIndex(weights);

        return index == -1 ? null : SetPiece(defs[index]);
    }

    private MarketDef SetPiece(MarketDef def)
    {
        switch (def.RandomType)
        {
            case MarketRandomType.BasePiecesEasy:
                def.Weight.Uid = GetRandomPiece(2, 3);
                break;
            case MarketRandomType.BasePiecesHard:
                def.Weight.Uid = GetRandomPiece(3, 5);
                break;
            case MarketRandomType.Ingredients:
                def.Weight.Uid = GetRandomIngredient();
                break;
            case MarketRandomType.BaseСhests:
                def.Weight.Uid = GetRandomChest();
                break;
        }
        
        return def;
    }
    
    private string GetRandomPiece(int min, int max)
    {
        var board = BoardService.Current.FirstBoard;
        var definition = board.BoardLogic.MatchDefinition;
        
        var pieces = PieceType.GetIdsByFilter(PieceTypeFilter.Normal, PieceTypeFilter.Fake);
        
        pieces = pieces.FindAll(id =>
        {
            var index = definition.GetIndexInChain(id);
            
            return index > min && GameDataService.Current.CodexManager.IsPieceUnlocked(id);
        });

        if (pieces.Count == 0) return null;

        var chains = new Dictionary<int, int>();
        
        foreach (var id in pieces)
        {
            var key = definition.GetFirst(id);
            int value;

            if (chains.TryGetValue(key, out value) == false)
            {
                chains.Add(key, id);
                continue;
            }

            if (value < id) chains[key] = id;
        }
        
        var ids = chains.Values.ToList();
        var maxId = ids[Random.Range(0, ids.Count)];
        var chain = definition.GetChain(maxId);
        var find = Mathf.Min(max, chain.IndexOf(maxId) - 1);

        return PieceType.Parse(chain[find]);
    }
    
    private string GetRandomIngredient()
    {
        var item = ItemWeight.GetRandomItem(GameDataService.Current.LevelsManager.ResourcesWeights);
        
        return item?.Uid;
    }
    
    private string GetRandomChest()
    {
        var chests = PieceType.GetIdsByFilter(PieceTypeFilter.Chest, PieceTypeFilter.Bag);

        chests.Remove(PieceType.CH_Free.Id);
        chests = chests.FindAll(id => GameDataService.Current.CodexManager.IsPieceUnlocked(id));

        if (chests.Count == 0) return null;

        var chest = chests[Random.Range(0, chests.Count)];
        var board = BoardService.Current.FirstBoard;
        var definition = board.BoardLogic.MatchDefinition;
        
        return PieceType.Parse(definition.GetLast(chest));
    }
}