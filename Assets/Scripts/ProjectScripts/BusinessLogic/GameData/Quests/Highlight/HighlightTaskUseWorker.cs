using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HighlightTaskUseWorker : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var boardLogic = BoardService.Current.FirstBoard.BoardLogic;
        var workPlacesList = boardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Workplace);
        
        if (workPlacesList.Count == 0)
        {
            return false;
        }

        // If we have accessible pieces, use them as target
        var accessiblePoints = HighlightTaskPathHelper.GetAccessiblePositions(workPlacesList);
        List<BoardPosition> positions = accessiblePoints.Count > 0 ? accessiblePoints : workPlacesList;

        // Select pieces with low energy requirements
        List<BoardPosition> selectedPositions = new List<BoardPosition>(); 
        int lastEnergy = Int32.MaxValue;

        for (var i = 0; i < positions.Count; i++)
        {
            var position = positions[i];
            var piece = boardLogic.GetPieceAt(position);
            if (piece == null)
            {
                continue;
            }

            LifeComponent        lifeComponent        = piece.GetComponent<LifeComponent>(LifeComponent.ComponentGuid);
            StorageLifeComponent storageLifeComponent = lifeComponent as StorageLifeComponent;
            if (storageLifeComponent == null)
            {
                Debug.LogError($"storageLifeComponent not found for pos {position}");
                continue;
            }

            int energyCost = storageLifeComponent.Energy.Amount;
            if (lastEnergy > energyCost)
            {
                lastEnergy = energyCost;
                selectedPositions.Clear();
                selectedPositions.Add(position);
            }
            else if (energyCost == lastEnergy)
            {
                selectedPositions.Add(position);
            }
        }

        if (selectedPositions.Count == 0)
        {
            selectedPositions = positions;
        }

        int index = Random.Range(0, selectedPositions.Count);
        BoardPosition selectedPosition = selectedPositions[index];

        HintArrowView.Show(selectedPosition);
        
        return true;
    }
}