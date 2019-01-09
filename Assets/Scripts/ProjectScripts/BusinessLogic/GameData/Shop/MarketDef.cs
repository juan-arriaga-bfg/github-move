using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum MarketRandomType
{
    Base,
    BasePieces,
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
            case MarketRandomType.BasePieces:
                def.Weight.Uid = GetRandomPiece();
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

    private string GetRandomPiece()
    {
        return PieceType.A3.Abbreviations[0];
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