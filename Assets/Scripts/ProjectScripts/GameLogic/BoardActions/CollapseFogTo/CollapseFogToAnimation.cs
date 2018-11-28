using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollapseFogToAnimation : BoardAnimation
{
    public CollapseFogToAction Action { get; set; }
    
    protected List<BoardElementView> fogElements = new List<BoardElementView>();

    protected FogPieceView fogPieceView;

    public override void PrepareAnimation(BoardRenderer context)
    {
        var points = Action.Positions;
        
        fogPieceView = context.GetElementAt(points[0]) as FogPieceView;
        
        fogElements.Clear();
        foreach (var point in points)
        {
            fogElements.Add(context.RemoveElementAt(point, false));
        }
    }

    public override void Animate(BoardRenderer context)
    {
        var points = Action.Positions;
        var to = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
		
        var sequence = DOTween.Sequence().SetId(animationUid);

        if (fogPieceView == null)
        {
            CompleteAnimation(context);
            return;
        }
            
        fogPieceView.SyncRendererLayers(context.Context.BoardDef.MaxPoit);

        var fogMaterial = fogPieceView.GetExistingCustomMaterial(BoardElementMaterialType.FogDefaultMaterial, fogPieceView);

        if (fogMaterial != null)
        {
            sequence.Insert(0f, fogMaterial.DOFloat(0f, "_AlphaCoef", 0.5f));
        }
        
        var fakePiecesFogMaterial = fogPieceView.GetExistingCustomMaterial(BoardElementMaterialType.PiecesUnderFogMaterial, fogPieceView);
        
        if (fakePiecesFogMaterial != null)
        {
            sequence.Insert(0f, fakePiecesFogMaterial.DOFloat(0f, "_AlphaCoef", 0.5f));
        }
        
        for (int i = 0; i < fogPieceView.FakePieces.Count; i++)
        {
            var fakePiece = fogPieceView.FakePieces[i];
            sequence.Insert(0f, fakePiece.CachedTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InSine));
        }
        
        // wait for spawn animation
        sequence.AppendInterval(1f);
		
        sequence.OnComplete(() =>
        {
            for (int i = 0; i < fogPieceView.FakePieces.Count; i++)
            {
                var fakePiece = fogPieceView.FakePieces[i];
                context.Context.BoardLogic.DragAndDrop.DestroyFakePiece(fakePiece);
            }
            fogPieceView.FakePieces.Clear();

            foreach (var fogElement in fogElements)
            {
                context.DestroyElement(fogElement);
            }

            CompleteAnimation(context);
        });
    }
}
