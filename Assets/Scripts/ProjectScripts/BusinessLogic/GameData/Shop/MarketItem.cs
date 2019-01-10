using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarketItem
{
    public string Uid;
    public int Index;
    public int Level = int.MaxValue;
    
    public bool IsPurchased;
    public bool IsClaimed = true;
    public bool IsLock => Level > GameDataService.Current.LevelsManager.Level;

    public CurrencyPair Reward;
    
    private readonly List<MarketDef> defs = new List<MarketDef>();

    private MarketDef current;
    public MarketDef Current
    {
        get
        {
            if(current == null) Update();
            
            return current;
        }
    }

    public void Init(int index, int piece, int amount, bool isPurchased)
    {
        IsPurchased = isPurchased;
        Index = index;
        
        if(Index == -1) return;

        IsClaimed = piece != PieceType.None.Id;
        
        current = defs[Index];
        Reward = new CurrencyPair{Currency = PieceType.Parse(piece), Amount = amount};
    }
    
    public void AddDef(MarketDef def)
    {
        defs.Add(def);
        
        if(Level > def.UnlockLevel) Level = def.UnlockLevel;
    }

    public void Update()
    {
        if(IsClaimed == false) return;
        
        var weights = new List<ItemWeight>();
        
        foreach (var def in defs)
        {
            if(def.UnlockLevel > GameDataService.Current.LevelsManager.Level) continue;
            
            weights.Add(def.Weight);
        }
        
        Index = ItemWeight.GetRandomItemIndex(weights);
        current = Index == -1 ? null : SetPiece(defs[Index]);
        
        if(Index == -1) Reward = new CurrencyPair{Currency = PieceType.Parse(PieceType.Empty.Id), Amount = defs[0].Amount};
        
        IsPurchased = current != null && current.Price == null;
        IsClaimed = Index == -1;
    }
    
    private MarketDef SetPiece(MarketDef def)
    {
        var piece = "";
        
        switch (def.RandomType)
        {
            case MarketRandomType.BasePiecesEasy:
                piece = GetRandomPiece(2, 3);
                break;
            case MarketRandomType.BasePiecesHard:
                piece = GetRandomPiece(3, 5);
                break;
            case MarketRandomType.Ingredients:
                piece = GetRandomIngredient();
                break;
            case MarketRandomType.BaseСhests:
                piece = GetRandomChest();
                
                if (string.IsNullOrEmpty(piece))
                {
                    Index = -1;
                    return null;
                }
                
                break;
            default :
                piece = def.Weight.Uid;
                break;
        }

        if (string.IsNullOrEmpty(piece)) piece = PieceType.Parse(PieceType.Empty.Id);
        
        Reward = new CurrencyPair{Currency = piece, Amount = def.Amount};
        
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