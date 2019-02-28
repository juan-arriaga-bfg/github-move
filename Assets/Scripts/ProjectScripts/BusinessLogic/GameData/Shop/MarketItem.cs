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
    public string Uid;
    public int Index;
    public int Level = int.MaxValue;
    
    private MarketItemState state;

    public MarketItemState State
    {
        get { return state; }
        set
        {
            state = value;
            GameDataService.Current.MarketManager.UpdateState?.Invoke();
        }
    }
    
    public CurrencyPair Reward;
    
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
    }

    public void Init(int index, int piece, int amount, MarketItemState state)
    {
        State = state;
        Index = index;
        
        if(Index == -1) return;
        
        current = defs[Index];
        Reward = new CurrencyPair{Currency = PieceType.Parse(piece), Amount = amount};
    }
    
    public void AddDef(MarketDef def)
    {
        defs.Add(def);
        
        if(Level > def.UnlockLevel) Level = def.UnlockLevel;
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
            if(def.UnlockLevel > GameDataService.Current.LevelsManager.Level) continue;
            
            weights.Add(def.Weight);
        }
        
        Index = ItemWeight.GetRandomItemIndex(weights);
        current = Index == -1 ? null : SetPiece(defs[Index]);
        
        if(Index == -1) Reward = new CurrencyPair{Currency = PieceType.Parse(PieceType.Empty.Id), Amount = defs[0].Amount};
        
        State = current != null && current.Price == null ? MarketItemState.Saved : MarketItemState.Normal;
    }
    
    private MarketDef SetPiece(MarketDef def)
    {
        var piece = "";
        
        switch (def.RandomType)
        {
            case MarketRandomType.BasePiecesEasy:
                piece = GetRandomPiece(3, 4);
                break;
            case MarketRandomType.BasePiecesHard:
                piece = GetRandomPiece(4, 6);
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

            if (chains.TryGetValue(key, out var value) == false)
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
    
    private string GetRandomChest(int index)
    {
        var chests = PieceType.GetIdsByFilter(PieceTypeFilter.Chest, PieceTypeFilter.Bag);

        chests.Remove(PieceType.CH_Free.Id);
        chests.Remove(PieceType.CH_NPC.Id);
        
        chests = chests.FindAll(id => GameDataService.Current.CodexManager.IsPieceUnlocked(id));

        if (chests.Count == 0) return null;

        var chest = chests[Random.Range(0, chests.Count)];
        var board = BoardService.Current.FirstBoard;
        var definition = board.BoardLogic.MatchDefinition;
        var chain = definition.GetChain(chest);
        
        return PieceType.Parse(chain[index]);
    }
}