using System.Collections.Generic;
using BfgAnalytics;

public class PartDef
{
    private int id  = PieceType.None.Id;

    public int Id
    {
        get
        {
            if (id != PieceType.None.Id || Seconds.Count == 0) return id;
            
            var piece = context.Context.BoardLogic.GetPieceAt(Key);

            if (piece.PieceType != PieceType.Boost_CR.Id) return piece.PieceType;

            foreach (var position in Seconds[0])
            {
                piece = context.Context.BoardLogic.GetPieceAt(position);

                if (piece.PieceType == PieceType.Boost_CR.Id) continue;

                return piece.PieceType;
            }

            return PieceType.None.Id;
        }
    }

    public BoardPosition Key;
    public List<BoardPosition> Main;
    public readonly List<List<BoardPosition>> Seconds = new List<List<BoardPosition>>();
    public bool IsFree => Seconds.Count == 0 && Main == null;

    private PartPiecesLogicComponent context;
    public ViewDefinitionComponent ViewDefinition;
    
    public void Init(BoardPosition key, PartPiecesLogicComponent logic)
    {
        Key = key;
        context = logic;
    }
    
    public void Add(List<BoardPosition> option)
    {
        var key = option[2];
        
        if (key.Equals(Key))
        {
            Main = option;
            OpenBubble();
            return;
        }

        foreach (var second in Seconds)
        {
            if(second[2].Equals(key) == false) continue;
            
            return;
        }

        Seconds.Add(option);
    }

    public void Remove(List<BoardPosition> option)
    {
        if (option[2].Equals(Key))
        {
            ViewDefinition.HideView(ViewType.Bubble);
            
            Main = null;
            ViewDefinition = null;
            id = PieceType.None.Id;
            return;
        }
        
        Seconds.Remove(option);
    }

    private void OpenBubble()
    {
        if (id != PieceType.None.Id) return;
        
        var piece = context.Context.BoardLogic.GetPieceAt(Key);
        
        id = piece.PieceType != PieceType.Boost_CR.Id ? piece.PieceType : context.Context.BoardLogic.GetPieceAt(Main[0]).PieceType;
        ViewDefinition = piece.ViewDefinition;
        
        var view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
        var lastId = context.Context.BoardLogic.MatchDefinition.GetLast(id);
        var def = GameDataService.Current.PiecesManager.GetPieceDef(lastId);
        
        var title = string.Format(LocalizationService.Get("gameboard.bubble.message.castle.build", "gameboard.bubble.message.castle.build\n{0}?"), DateTimeExtension.GetDelayText(def.Delay, true));
        var button = string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), $"<sprite name={Currency.Worker.Icon}>");
        
        view.SetData(title, button, OnClick);
        view.Change(true);
    }
    
    private void OnClick(Piece piece)
    {
        if (context.Context.WorkerLogic.Get(piece.CachedPosition, null) == false) return;
        context.Work(piece.CachedPosition, false);
    }
}

public class PartPiecesLogicComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public BoardController Context;
    
    private readonly Dictionary<BoardPosition, PartDef> links = new Dictionary<BoardPosition, PartDef>();
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Context = entity as BoardController;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Add(List<BoardPosition> positions)
    {
        foreach (var key in positions)
        {
            if (links.TryGetValue(key, out var def) == false)
            {
                def = new PartDef();
                def.Init(key, this);
                links.Add(key, def);
            }
            
            def.Add(positions);
        }
    }
    
    public void Remove(BoardPosition position)
    {
        if (links.TryGetValue(position, out var def) == false) return;
        
        if (def.Main != null)
        {
            Remove(def.Main, position);
            def.Remove(def.Main);
        }

        for (var i = def.Seconds.Count - 1; i >= 0; i--)
        {
            var option = def.Seconds[i];
            Remove(option, position);
            def.Remove(option);
        }
        
        links.Remove(position);
    }

    private void Remove(List<BoardPosition> positions, BoardPosition ignore)
    {
        foreach (var pos in positions)
        {
            if (ignore.Equals(pos) || links.TryGetValue(pos, out var child) == false) continue;
                
            child.Remove(positions);
            
            if (child.IsFree) links.Remove(pos);
        }
    }
    
    public bool Work(BoardPosition position, bool isExtra)
    {
        if (links.TryGetValue(position, out var def) == false) return false;
        
        var positions = new List<BoardPosition>();
        var action = Context.BoardLogic.MatchActionBuilder.GetMatchAction(positions, def.Id, position);
        
        if (action == null) return false;
        
        SendAnalytics(positions, def.Id);
        Remove(position);
        Context.ActionExecutor.PerformAction(action);
        
        if (!isExtra) return true;
        
        var result = Context.BoardLogic.GetPieceAt(positions[0])?.PieceState;
            
        result?.Timer.Subtract(GameDataService.Current.ConstantsManager.ExtraWorkerDelay, 1.5f);

        return true;
    }

    public List<ViewDefinitionComponent> GetAllView()
    {
        var result = new List<ViewDefinitionComponent>();

        foreach (var value in links.Values)
        {
            if (value.ViewDefinition == null) continue;
            
            result.Add(value.ViewDefinition);
        }

        return result;
    }
    
    public List<BoardPosition> GetAllPositions()
    {
        var result = new List<BoardPosition>();

        foreach (var value in links.Values)
        {
            result.Add(value.Key);
        }

        return result;
    }
    
    private void SendAnalytics(List<BoardPosition> positions, int id)
    {
        var logic = Context.BoardLogic;
        
        foreach (var position in positions)
        {
            if (logic.GetPieceAt(position).PieceType != PieceType.Boost_CR.Id) continue;

            var next = logic.MatchDefinition.GetNext(id);
            var pair = new CurrencyPair {Currency = PieceType.Parse(PieceType.Boost_CR.Id), Amount = 1};
            
            Analytics.SendPurchase("board_merge", PieceType.Parse(next), new List<CurrencyPair>{pair}, null, false, false);
            return;
        }
    }
}