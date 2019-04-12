using System.Collections.Generic;
using UnityEngine;

public class PartDef
{
    public int Id = PieceType.None.Id;
    public BoardPosition Key;
    public List<BoardPosition> Main;
    public readonly List<List<BoardPosition>> Second = new List<List<BoardPosition>>();
    public bool IsFree => Second.Count == 0 && Main == null;

    private PartPiecesLogicComponent context;
    public ViewDefinitionComponent ViewDefinition;
    
    public void Init(BoardPosition key, PartPiecesLogicComponent logic)
    {
        Key = key;
        context = logic;
    }
    
    public void Add(List<BoardPosition> option)
    {
        if (option[2].Equals(Key))
        {
            Main = option;
            OpenBubble();
            return;
        }
        
        Second.Add(option);
    }

    public void Remove(List<BoardPosition> option)
    {
        if (option[2].Equals(Key))
        {
            var view = ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
            
            view.Change(false);
            
            Main = null;
            ViewDefinition = null;
            Id = PieceType.None.Id;
            return;
        }
        
        Second.Remove(option);
    }

    private void OpenBubble()
    {
        if (Id != PieceType.None.Id) return;
        
        var piece = context.Context.BoardLogic.GetPieceAt(Key);
        
        Id = piece.PieceType != PieceType.Boost_CR.Id ? piece.PieceType : context.Context.BoardLogic.GetPieceAt(Main[0]).PieceType;
        ViewDefinition = piece.ViewDefinition;
        
        var view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
        var lastId = context.Context.BoardLogic.MatchDefinition.GetLast(Id);
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

        for (var i = def.Second.Count - 1; i >= 0; i--)
        {
            var option = def.Second[i];
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
        
        Remove(position);
        Context.ActionExecutor.PerformAction(action);

        if (!isExtra) return true;
        
        var result = Context.BoardLogic.GetPieceAt(positions[0])?.PieceState;
            
        result?.Timer.Subtract(GameDataService.Current.ConstantsManager.ExtraWorkerDelay);

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
}