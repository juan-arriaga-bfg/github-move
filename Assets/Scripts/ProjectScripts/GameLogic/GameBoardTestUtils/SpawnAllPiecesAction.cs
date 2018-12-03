using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SpawnAllPiecesAction  : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public bool PerformAction(BoardController gameBoardController)
    {
        var targetPieces = gameBoardController.BoardLogic.EmptyCellsFinder.FindAllWithCondition(position =>
        {
            bool isEmpty = gameBoardController.BoardLogic.IsEmpty(position);

            if (isEmpty) return true;

            return false;
       
        }, BoardLayer.Piece.Layer);
        
        targetPieces.Shuffle(1000);

        var pieces = GetPieceIdsBy(PieceTypeFilter.Default);

        var sequence = DOTween.Sequence().SetId(this);
        for (int i = 0; i < pieces.Count; i++)
        {
            var pieceId = pieces[i];
            
            if (targetPieces.Count <= 0) continue;

            var point = targetPieces[targetPieces.Count - 1];
            targetPieces.RemoveAt(targetPieces.Count - 1);

            sequence.AppendInterval(0.3f).AppendCallback(() =>
            {
                var spawnPieceAction = new SpawnPieceAtAction{ At = point, PieceTypeId = pieceId };
                gameBoardController.ActionExecutor.AddAction(spawnPieceAction);
            });
        }
        
        // // add delay to spawn animations
        // var animationsQueue = gameBoardController.RendererContext.GetAnimationsQueue();
        // for (int i = 0; i < animationsQueue.Count; i++)
        // {
        //     var spawnAnimation = animationsQueue[i] as SpawnPieceAtAnimation;
        //     
        //     if (spawnAnimation == null) continue;
        //
        //     spawnAnimation.Delay = 0.1f * i;
        // }

        return true;
    }
    
    public List<int> GetPieceIdsBy(PieceTypeFilter typeFilter)
    {
        var exclude = new HashSet<int>
        {
            0, 
            -1,
            1,
            PieceType.Fog.Id
        };
        
        var ret = PieceType.GetIdsByFilter(typeFilter);

        ret = ret.Where(e => !PieceType.GetDefById(e).Filter.Has(PieceTypeFilter.Fake) &&  exclude.Contains(e) == false)
                 .ToList();
        
        return ret;
    }
}
