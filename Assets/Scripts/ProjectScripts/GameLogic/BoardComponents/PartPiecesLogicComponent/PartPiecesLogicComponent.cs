using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartDef
{
    public List<BoardPosition> Pattern;
    public ViewDefinitionComponent ViewDefinition;
}

public class PartPiecesLogicComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private BoardController context;
    
    private readonly Dictionary<BoardPosition, ViewDefinitionComponent> bubbles = new Dictionary<BoardPosition, ViewDefinitionComponent>();
    private readonly Dictionary<BoardPosition, List<PartDef>> links = new Dictionary<BoardPosition, List<PartDef>>();
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Add(List<BoardPosition> positions)
    {
        var index = positions.Count / 2;
        var position = positions[index];
        
        if (bubbles.ContainsKey(position)) return;
        
        var piece = context.BoardLogic.GetPieceAt(position);
        var pieceType = piece.PieceType;
        
        positions.Find(tempPosition =>
        {
            pieceType = context.BoardLogic.GetPieceAt(tempPosition).PieceType;
            return pieceType != PieceType.Boost_CR.Id;
        });
        
        bubbles.Add(position, piece.ViewDefinition);

        foreach (var key in positions)
        {
            if (links.TryGetValue(key, out var defs) == false)
            {
                defs = new List<PartDef>();
                links.Add(key, defs);
            }
            
            defs.Add(new PartDef{ViewDefinition = piece.ViewDefinition, Pattern = positions});
        }
        
        var view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType + 2);

        var title = string.Format(LocalizationService.Get("gameboard.bubble.message.castle.build", "gameboard.bubble.message.castle.build\n{0}?"), DateTimeExtension.GetDelayText(def.Delay));
        var button = string.Format(LocalizationService.Get("gameboard.bubble.button.send", "gameboard.bubble.button.send {0}"), $"<sprite name={Currency.Worker.Icon}>");
        
        view.SetData(title, button, OnClick);
        view.Change(true);
    }
    
    public void Remove(BoardPosition position)
    {
        if (links.TryGetValue(position, out var defs) == false) return;
        
        if (defs.Count == 0)
        {
            links.Remove(position);
            return;
        }
        
        var partDef = new List<PartDef>(defs);

        foreach (var def in partDef)
        {
            Remove(def.Pattern);
        }
    }

    public void Remove(List<BoardPosition> positions)
    {
        var index = positions.Count / 2;
        var position = positions[index];
        
        if (bubbles.TryGetValue(position, out var view) == false) return;

        foreach (var key in positions)
        {
            if (links.TryGetValue(key, out var defs) == false) continue;

            var partDef = defs.Find(def => def.ViewDefinition == view);

            defs.Remove(partDef);
            
            if (defs.Count == 0) links.Remove(key);
        }
        
        bubbles.Remove(position);
        view.AddView(ViewType.Bubble).Change(false);
    }
    
    private void OnClick(Piece piece)
    {
        if (context.WorkerLogic.Get(piece.CachedPosition, null) == false) return;
        
        Work(piece, false);
    }

    public bool Work(Piece piece, bool isExtra)
    {
        var positions = new List<BoardPosition>();
        var action = context.BoardLogic.MatchActionBuilder.GetMatchAction(positions, piece.PieceType, piece.CachedPosition);

        if (action == null) return false;

        Remove(positions);
        context.ActionExecutor.PerformAction(action);

        if (!isExtra) return true;
        
        var result = context.BoardLogic.GetPieceAt(positions[0])?.PieceState;
            
        result?.Timer.Subtract(GameDataService.Current.ConstantsManager.ExtraWorkerDelay);

        return true;
    }

    public List<ViewDefinitionComponent> GetAllView()
    {
        return bubbles.Values.ToList();
    }
    
    public List<BoardPosition> GetAllPositions()
    {
        var result = new List<BoardPosition>();

        foreach (var value in links.Values)
        {
            foreach (var def in value)
            {
                result.AddRange(def.Pattern);
            }
        }

        return result;
    }
}