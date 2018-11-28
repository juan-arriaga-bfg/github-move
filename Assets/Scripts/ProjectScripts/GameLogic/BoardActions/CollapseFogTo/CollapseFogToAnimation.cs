using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollapseFogToAnimation : BoardAnimation
{
    public CollapseFogToAction Action { get; set; }
	
    public override void Animate(BoardRenderer context)
    {
        var points = Action.Positions;
        var to = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
		
        var sequence = DOTween.Sequence().SetId(animationUid);
		
        var boardElement = context.GetElementAt(points[0]) as FogPieceView;

        if (boardElement == null)
        {
            CompleteAnimation(context);
            return;
        }
            
        boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);

        var fogMaterial = boardElement.GetExistingCustomMaterial(BoardElementMaterialType.FogDefaultMaterial, boardElement);

        if (fogMaterial != null)
        {
            sequence.Insert(0f, fogMaterial.DOFloat(0f, "_AlphaCoef", 0.5f));
        }
        
        var fakePiecesFogMaterial = boardElement.GetExistingCustomMaterial(BoardElementMaterialType.PiecesUnderFogMaterial, boardElement);
        
        if (fakePiecesFogMaterial != null)
        {
            sequence.Insert(0f, fakePiecesFogMaterial.DOFloat(0f, "_AlphaCoef", 0.5f));
        }
        
        for (int i = 0; i < boardElement.FakePieces.Count; i++)
        {
            var fakePiece = boardElement.FakePieces[i];
            sequence.Insert(0f, fakePiece.CachedTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InSine));
        }
        
        // foreach (var point in points)
        // {
        //     
        // }

        // wait for spawn animation
        sequence.AppendInterval(1f);
		
        sequence.OnComplete(() =>
        {
            for (int i = 0; i < boardElement.FakePieces.Count; i++)
            {
                var fakePiece = boardElement.FakePieces[i];
                context.Context.BoardLogic.DragAndDrop.DestroyFakePiece(fakePiece);
            }
            boardElement.FakePieces.Clear();

            foreach (var point in points)
            {
                context.RemoveElementAt(point);
            }

            CompleteAnimation(context);
        });
    }
}
