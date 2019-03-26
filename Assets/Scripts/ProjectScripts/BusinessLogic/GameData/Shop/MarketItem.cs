using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MarketItemState
{
    Lock,
    Normal,
    Purchased,
    Saved,
    Claimed
}

public class MarketItem
{
    public int Uid;
    public int Index;
    public int Level = int.MaxValue;
    
    private MarketItemState state;

    public MarketItemState State
    {
        get { return state; }
        set
        {
            if (current != null && current.IsPermanent)
            {
                if (value < MarketItemState.Purchased) state = value;
            }
            else
            {
                state = value;
            }
            
            GameDataService.Current.MarketManager.UpdateState?.Invoke();
        }
    }
    
    public CurrencyPair Reward;
    public CurrencyPair Price;

    public bool IsPiece => string.IsNullOrEmpty(Reward.Currency) == false && PieceType.Parse(Reward.Currency) != -1;
    
    public string Name => IsPiece ? $"piece.name.{Reward.Currency}" : current.Name;
    public string Icon => IsPiece ? Reward.Currency : current.Icon;
    
    private readonly List<MarketDef> defs = new List<MarketDef>();

    private MarketDef current;
    public MarketDef Current
    {
        get
        {
            if (current == null) Update(Reward == null);
            if (Level > GameDataService.Current.LevelsManager.Level) State = MarketItemState.Lock;
            
            return current;
        }
        set => current = value;
    }

    public void Init(int index, int piece, int amount, MarketItemState initState)
    {
        State = initState;
        Index = Mathf.Clamp(index, -1, defs.Count - 1);

        if (Index == -1) return;
        
        current = defs[Index];

        Reward = new CurrencyPair{Currency = piece == -1 ? current.Weight.Uid : PieceType.Parse(piece), Amount = amount};
        Price = current.Price ?? GetPrice(Reward.Currency);
    }
    
    public void AddDef(MarketDef def)
    {
        defs.Add(def);

        if (Level > def.UnlockLevel) Level = def.UnlockLevel;
    }

    public void Update(bool isTimer = false)
    {
        if (isTimer)
        {
            if (State == MarketItemState.Purchased || State == MarketItemState.Saved) return;
        }
        else
        {
            if (State != MarketItemState.Lock && Index != -1) return;
        }
        
        var weights = new List<ItemWeight>();
        
        foreach (var def in defs)
        {
            if (def.UnlockLevel > GameDataService.Current.LevelsManager.Level) continue;
            
            weights.Add(def.Weight);
        }
        
        Index = ItemWeight.GetRandomItemIndex(weights);
        current = Index == -1 ? null : SetPiece(defs[Index]);

        if (Index == -1) Reward = new CurrencyPair {Currency = PieceType.Parse(PieceType.Empty.Id), Amount = defs[0].Amount};
        
        State = current != null && Price == null ? MarketItemState.Saved : MarketItemState.Normal;
    }
    
    private MarketDef SetPiece(MarketDef def)
    {
        var piece = "";
        
        switch (def.RandomType)
        {
            case MarketRandomType.BasePiecesEasy:
                piece = GetRandomPiece(2, 4);
                break;
            case MarketRandomType.BasePiecesHard:
                piece = GetRandomPiece(5, 6);
                break;
            case MarketRandomType.Ingredients:
                piece = GetRandomIngredient();
                break;
            case MarketRandomType.BaseChestsFirst:
                piece = GetRandomChest(0);
                break;
            case MarketRandomType.BaseChestsSecond:
                piece = GetRandomChest(1);
                break;
            case MarketRandomType.BaseChestsLast:
                piece = GetRandomChest(2);
                break;
            case MarketRandomType.NPCChests:
                piece = GameDataService.Current.CharactersManager.Characters.Count == 0 ? null : def.Weight.Uid;
                break;
            default :
                piece = def.Weight.Uid;
                break;
        }
        
        if (string.IsNullOrEmpty(piece))
        {
            Index = -1;
            return null;
        }
        
        Reward = new CurrencyPair{Currency = piece, Amount = def.Amount};
        Price = def.Price ?? GetPrice(piece);
        
        return def;
    }

    private CurrencyPair GetPrice(string name)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(name));

        return def?.ExchangePrice == null ? null : new CurrencyPair{Currency = def.ExchangePrice.Currency, Amount = def.ExchangePrice.Amount * Reward.Amount};
    }
    
    private string GetRandomPiece(int min, int max)
    {
        max -= 1;
        
        var definition = BoardService.Current.FirstBoard.BoardLogic.MatchDefinition;
        var pieces = PieceType.GetIdsByFilter(PieceTypeFilter.Normal, PieceTypeFilter.Fake);
        var used = GameDataService.Current.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Pieces);
        
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

            if (chains.TryGetValue(key, out var value) == false)
            {
                chains.Add(key, id);
                continue;
            }

            if (value < id) chains[key] = id;
        }

        if (chains.Count > used.Count)
        {
            foreach (var item in used)
            {
                chains.Remove(definition.GetFirst(PieceType.Parse(item.Reward.Currency)));
            }
        }
        
        var ids = chains.Values.ToList();
        var maxId = ids[Random.Range(0, ids.Count)];
        var chain = definition.GetChain(maxId);
        var find = Mathf.Min(max, chain.IndexOf(maxId) - 1);
        
        return PieceType.Parse(chain[find]);
    }
    
    private string GetRandomIngredient()
    {
        var definition = BoardService.Current.FirstBoard.BoardLogic.MatchDefinition;
        var weights = new List<ItemWeight>(GameDataService.Current.LevelsManager.ResourcesWeights);
        var used = GameDataService.Current.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Ingredients);
        
        weights = weights.FindAll(weight =>
        {
            var prev = definition.GetPrevious(weight.Piece);
            return GameDataService.Current.CodexManager.IsPieceUnlocked(prev);
        });
        
        if (weights.Count > used.Count)
        {
            weights = weights.FindAll(weight => used.Find(item => item.Reward.Currency == weight.Uid) == null);
        }
        
        return ItemWeight.GetRandomItem(weights)?.Uid;
    }
    
    private string GetRandomChest(int index)
    {
        var definition = BoardService.Current.FirstBoard.BoardLogic.MatchDefinition;
        var chests = PieceType.GetIdsByFilter(PieceTypeFilter.Chest, PieceTypeFilter.Bag);
        var used = GameDataService.Current.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Chests);

        chests.Remove(PieceType.CH_Free.Id);
        chests.Remove(PieceType.CH_NPC.Id);
        
        chests = chests.FindAll(id => definition.GetPrevious(id) == PieceType.None.Id && GameDataService.Current.CodexManager.IsPieceUnlocked(id));

        if (chests.Count > used.Count)
        {
            chests = chests.FindAll(id => used.Find(item => definition.GetFirst(PieceType.Parse(item.Reward.Currency)) == id) == null);
        }
        
        if (chests.Count == 0) return null;
        
        var chest = chests[Random.Range(0, chests.Count)];
        var chain = definition.GetChain(chest);
        
        return PieceType.Parse(chain[index]);
    }
}