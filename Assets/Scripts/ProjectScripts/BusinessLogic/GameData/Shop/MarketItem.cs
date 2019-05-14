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

    private GameDataManager gameDataManager;
    
    public GameDataManager GameDataManager
    {
        set => gameDataManager = value;
    }
    
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
            
            gameDataManager.MarketManager.UpdateState?.Invoke();
        }
    }
    
    public CurrencyPair Reward;
    public CurrencyPair Price;

    public bool IsPiece => string.IsNullOrEmpty(Reward.Currency) == false && PieceType.Parse(Reward.Currency) != -1;
    
    public string Name => IsPiece ? $"piece.name.{Reward.Currency}" : current.Name;
    public string Icon => IsPiece ? Reward.Currency : current.Icon;
    public string Description
    {
        get
        {
            if (string.IsNullOrEmpty(current?.Description)) return string.Format(LocalizationService.Get("window.market.description.lock", "window.market.description.lock {0}"), Level);
            if (current.Bundle != MarketItemBundle.Chests) return LocalizationService.Get(current.Description, current.Description);
            
            var def = gameDataManager.ChestsManager.GetChest(PieceType.Parse(Reward.Currency));
            var min = int.MaxValue;

            foreach (var weight in def.PieceWeights)
            {
                var pieceDef = PieceType.GetDefById(weight.Piece);
                if (pieceDef.Filter.Has(PieceTypeFilter.Analytics) == false || min < weight.Piece) continue;

                min = weight.Piece;
            }

            if (min == int.MaxValue) min = PieceType.None.Id;
                
            return string.Format(LocalizationService.Get(current.Description, $"{current.Description} {0}"), $"<sprite name={PieceType.Parse(min)}>");
        }
    }

    private readonly List<MarketDef> defs = new List<MarketDef>();

    private MarketDef current;
    public MarketDef Current
    {
        get
        {
            if (current == null) Update(Reward == null);
            if (state < MarketItemState.Purchased && Level > gameDataManager.LevelsManager.Level) State = MarketItemState.Lock;
            
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

        if (State == MarketItemState.Normal && Price == null) State = MarketItemState.Saved;
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
            if (def.UnlockLevel > gameDataManager.LevelsManager.Level) continue;
            
            weights.Add(def.Weight);
        }
        
        Index = ItemWeight.GetRandomItemIndex(weights);
        current = Index == -1 ? null : SetPiece(defs[Index]);

        if (Index == -1) Reward = new CurrencyPair {Currency = PieceType.Parse(PieceType.Empty.Id), Amount = defs[0].Amount};
        
        State = current != null && Price == null ? MarketItemState.Saved : MarketItemState.Normal;
    }
    
    private MarketDef SetPiece(MarketDef def)
    {
        string piece;
        
        switch (def.RandomType)
        {
            case MarketRandomType.BasePiecesEasy:
                piece = GetRandomPiece(2, 2);
                break;
            case MarketRandomType.BasePiecesNormal:
                piece = GetRandomPiece(3, 4);
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
                piece = gameDataManager.CharactersManager.Characters.Count == 0 ? string.Empty : def.Weight.Uid;
                break;
            case MarketRandomType.Worker:
                piece = gameDataManager.TutorialDataManager.CheckUnlockWorker() == false ? string.Empty : def.Weight.Uid;
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
        var def = gameDataManager.PiecesManager.GetPieceDef(PieceType.Parse(name));

        return def?.ExchangePrice == null ? null : new CurrencyPair{Currency = def.ExchangePrice.Currency, Amount = def.ExchangePrice.Amount * Reward.Amount};
    }
    
    private string GetRandomPiece(int min, int max)
    {
        max -= 1;

        var definition = gameDataManager.MatchDefinition;

        if (definition == null) return string.Empty;
        
        var pieces = PieceType.GetIdsByFilter(PieceTypeFilter.Normal, PieceTypeFilter.Fake);
        var used = gameDataManager.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Pieces);
        
        pieces = pieces.FindAll(id =>
        {
            var index = definition.GetIndexInChain(id);
            
            return index > min && gameDataManager.CodexManager.IsPieceUnlocked(id);
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
        var definition = gameDataManager.MatchDefinition;

        if (definition == null) return string.Empty;

        var weights = new List<ItemWeight>(gameDataManager.LevelsManager.ResourcesWeights);
        var used = gameDataManager.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Ingredients);
        
        weights = weights.FindAll(weight =>
        {
            var prev = definition.GetPrevious(weight.Piece);
            return gameDataManager.CodexManager.IsPieceUnlocked(prev);
        });
        
        if (weights.Count > used.Count)
        {
            weights = weights.FindAll(weight => used.Find(item => item.Reward.Currency == weight.Uid) == null);
        }
        
        return ItemWeight.GetRandomItem(weights)?.Uid;
    }
    
    private string GetRandomChest(int index)
    {
        var definition = gameDataManager.MatchDefinition;

        if (definition == null) return string.Empty;
        
        var chests = PieceType.GetIdsByFilter(PieceTypeFilter.Chest, PieceTypeFilter.Bag);
        var used = gameDataManager.MarketManager.Defs.FindAll(def => def.current != null && def.current.Bundle == MarketItemBundle.Chests);

        chests.Remove(PieceType.CH_Free.Id);
        chests.Remove(PieceType.CH_NPC.Id);
        
        chests = chests.FindAll(id => definition.GetPrevious(id) == PieceType.None.Id && gameDataManager.CodexManager.IsPieceUnlocked(id));

        if (chests.Count > used.Count)
        {
            chests = chests.FindAll(id => used.Find(item => definition.GetFirst(PieceType.Parse(item.Reward.Currency)) == id) == null);
        }
        
        if (chests.Count == 0) return string.Empty;
        
        var chest = chests[Random.Range(0, chests.Count)];
        var chain = definition.GetChain(chest);
        
        return PieceType.Parse(chain[index]);
    }
}