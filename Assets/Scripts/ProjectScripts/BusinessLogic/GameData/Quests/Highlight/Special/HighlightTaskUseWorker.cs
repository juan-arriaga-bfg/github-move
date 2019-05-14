using Debug = IW.Logger;
using System.Collections.Generic;
using DG.Tweening;
using Random = UnityEngine.Random;

public class HighlightTaskUseWorker : TaskHighlightUsingArrow
{
    protected override bool ShowArrow(TaskEntity task, float delay)
    {
        var views = BoardService.Current.FirstBoard.PartPiecesLogic.GetAllView();
        
        if (GetAllPositions(out var workPlacesFree, out var workPlacesLock, out var unfinishedFree) == false && views.Count == 0) return false;

        if (CheckEnergy(workPlacesFree, out var selected, out var workPlacesBusy) || CheckUnfinished(unfinishedFree, out selected))
        {
            HintArrowView.Show(selected);
            return true;
        }
        
        if (views.Count > 0)
        {
            var view = views[Random.Range(0, views.Count)].AddView(ViewType.Bubble);
            
            DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    const float DURATION = 1f;

                    if (BoardService.Current.FirstBoard.Manipulator.CameraManipulator.CameraMove.IsLocked == false)
                    {
                        BoardService.Current.FirstBoard.Manipulator.CameraManipulator.MoveTo(view.transform.position, true, DURATION);
                    }
                        
                    DOTween.Sequence()
                        .AppendInterval(DURATION - 0.15f)
                        .AppendCallback(() => { view.Attention(); });
                });
            
            return true;
        }

        if (CheckBusy(workPlacesBusy, out selected) || CheckEnergy(workPlacesLock, out selected, out workPlacesBusy))
        {
            HintArrowView.Show(selected);
            return true;
        }
        
        return false;
    }

    private bool GetAllPositions(out List<BoardPosition> workPlacesFree, out List<BoardPosition> workPlacesLock, out List<BoardPosition> unfinishedFree)
    {
        workPlacesFree = new List<BoardPosition>();
        workPlacesLock = new List<BoardPosition>();
        unfinishedFree = new List<BoardPosition>();

        var board = BoardService.Current.FirstBoard;
        var posCache = board.BoardLogic.PositionsCache;
        var workPlacesList = posCache.GetPiecePositionsByFilter(PieceTypeFilter.Mine);
        
        workPlacesList.AddRange(posCache.GetPiecePositionsByFilter(PieceTypeFilter.Obstacle));
        workPlacesList.AddRange(posCache.GetPiecePositionsByFilter(PieceTypeFilter.Multicellular | PieceTypeFilter.Analytics, PieceTypeFilter.Fake));

        if (workPlacesList.Count > 0)
        {
            workPlacesFree = HighlightTaskPathHelper.GetAccessiblePositions(workPlacesList);

            if (workPlacesFree.Count == 0) workPlacesLock = workPlacesList;
            if (workPlacesFree.Count > 0) return true;
        }
        
        var unfinishedList = posCache.GetPiecePositionsByFilter(PieceTypeFilter.Simple | PieceTypeFilter.Fake | PieceTypeFilter.Workplace);

        if (unfinishedList.Count > 0) unfinishedFree = HighlightTaskPathHelper.GetAccessiblePositions(unfinishedList);
        
        return unfinishedFree.Count > 0;
    }

    private bool CheckEnergy(List<BoardPosition> positions, out BoardPosition selected, out List<Piece> busy)
    {
        selected = BoardPosition.Default();
        busy = new List<Piece>();
        
        if (positions.Count == 0) return false;

        // Select pieces with low energy requirements
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var selectedPositions = new List<BoardPosition>(); 
        var lastEnergy = int.MaxValue;

        for (var i = 0; i < positions.Count; i++)
        {
            var position = positions[i];
            int energyCost;

            var piece = logic.GetPieceAt(position);
            if (piece == null)
            {
                Debug.LogError($"piece not found at {position}");
                continue;
            }
            
            var life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
            
            if (life == null)
            {
                var def = PieceType.GetDefById(piece.PieceType);
                if (def.Filter.Has(PieceTypeFilter.Mine) && def.Filter.Has(PieceTypeFilter.Fake))
                {
                    energyCost = 0;
                }
                else
                {
                    Debug.LogError($"WorkplaceLifeComponent not found at {position}");
                    continue;
                }
            }
            else
            {
                if (life.IsDead)
                {
                    Debug.Log($"WorkplaceLifeComponent is dead at {position}");
                    continue;
                }

                if (life.TimerWork != null && life.TimerWork.IsExecuteable() || life.TimerCooldown != null && life.TimerCooldown.IsExecuteable())
                {
                    busy.Add(life.Context);
                    continue;
                }
                
                energyCost = life.Energy.Amount;
            }

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

        if (selectedPositions.Count == 0) return false;
        
        selected = selectedPositions[Random.Range(0, selectedPositions.Count)];
        
        return true;
    }

    private bool CheckBusy(List<Piece> pieces, out BoardPosition selected)
    {
        selected = BoardPosition.Default();

        if (pieces.Count == 0) return false;
        
        var selectedPositions = new List<BoardPosition>(); 
        var lastEnergy = int.MaxValue;

        for (var i = 0; i < pieces.Count; i++)
        {
            var life = pieces[i].GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
            var energyCost = life.Energy.Amount;
            
            if (lastEnergy > energyCost)
            {
                lastEnergy = energyCost;
                selectedPositions.Clear();
                selectedPositions.Add(pieces[i].CachedPosition);
            }
            else if (energyCost == lastEnergy)
            {
                selectedPositions.Add(pieces[i].CachedPosition);
            }
        }
        
        if (selectedPositions.Count == 0) return false;
        
        selected = selectedPositions[Random.Range(0, selectedPositions.Count)];
        
        return true;
    }

    private bool CheckUnfinished(List<BoardPosition> positions, out BoardPosition selected)
    {
        selected = BoardPosition.Default();
        
        if (positions.Count == 0) return false;
        
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var selectedPositions = new List<BoardPosition>(); 
        var lastDelay = int.MaxValue;

        for (var i = 0; i < positions.Count; i++)
        {
            var position = positions[i];
            PieceStateComponent unfinished = logic.GetPieceAt(position)?.PieceState;

            if (unfinished == null)
            {
                Debug.LogError($"PieceStateComponent not found at {position}");
                continue;
            }

            if (unfinished.State == BuildingState.InProgress || unfinished.State == BuildingState.Complete)
            {
                Debug.Log($"PieceStateComponent InProgress at {position}");
                continue;
            }
            
            var delay = unfinished.Timer.Delay;
            
            if (lastDelay > delay)
            {
                lastDelay = delay;
                selectedPositions.Clear();
                selectedPositions.Add(position);
            }
            else if (delay == lastDelay)
            {
                selectedPositions.Add(position);
            }
        }

        if (selectedPositions.Count == 0) return false;
        
        selected = selectedPositions[Random.Range(0, selectedPositions.Count)];
        
        return true;
    }
}