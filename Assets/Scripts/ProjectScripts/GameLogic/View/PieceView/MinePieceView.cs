using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinePieceView : PieceBoardElementView
{
    private ViewDefinitionComponent viewDef;
    private PiceLabelView labelView;
    private List<bool> viewLastStates = new List<bool>();
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        if (!piece.IsHasComponent(ViewDefinitionComponent.ComponentGuid))
            piece.RegisterComponent(new ViewDefinitionComponent());
        viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        labelView = viewDef.AddView(ViewType.LevelLabel) as PiceLabelView;
        Debug.Log(labelView.IsShow);
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        //labelView.FadeAlpha(0, 0.2f, (_) => { labelView.SetActiveState(false); });
        foreach (var view in viewDef.GetViews())
        {
            viewLastStates.Add(view.isActiveAndEnabled);
            view.SetActiveState(false);
        }
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        //labelView.FadeAlpha(1, 0.2f, (_) => { labelView.SetActiveState(true); });
        int counter = 0;
        var views = viewDef.GetViews();
        foreach (var view in views)
        {
            if (counter >= views.Count)
                break;
            view.SetActiveState(viewLastStates[counter]);
            counter++;
        }
        viewLastStates.Clear();
    }
}