using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleProgressView : UIBoardView
{
    [SerializeField] private BoardProgressbar bar;
    
    private WorkplaceLifeComponent life;
    
    public override bool IsTop => true;

    protected override Vector3 offset => new Vector3(0, 1.5f);
    
    protected override ViewType Id => ViewType.ObstacleProgress;

    private bool isAddListener;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 1;
        
        life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        bar.IsVisible = life.HP != -1;
        bar.Init(life.HP);
        
        UpdateViewInternal();
        
    }

    public override void ResetViewOnDestroy()
    {
        bar.Clear();
        
        base.ResetViewOnDestroy();
    }
    
    public override void UpdateVisibility(bool isVisible)
    {   
//        if (isVisible)
//        {
//            Context.Context.HintCooldown.Pause(this);
//        }
//        else
//        {
//            Context.Context.HintCooldown.Resume(this);
//        }

        base.UpdateVisibility(isVisible);
        
        if (IsShow == false) return;

        UpdateViewInternal();
    }


    private void ToggleInternal(bool state)
    {
        canvas.enabled = state;
        group.alpha = state ? group.alpha : 0f;
    }
    
    private void UpdateViewInternal()
    {
        if (life != null && bar != null)
        {
            if (life.Current <= 0)
            {
                ToggleInternal(false);
                return;
            }
            
            if (life.IsHideTimer == false)
            {
                ToggleInternal(false);
                return;
            }
            
            if (life.IsUseCooldown)
            {
                ToggleInternal(false);
                return;
            }
            
            if (life.IsDead)
            {
                ToggleInternal(false);
                return;
            }
            
            if (life.TimerWork != null && life.TimerWork.IsStarted)
            {
                ToggleInternal(false);
                return;
            }
            
            var showedViews = Context.ViewDefinition?.GetShowedViews();
        
            int count = 0;
            for (int i = 0; i < showedViews?.size; i++)
            {
                var showedView = showedViews[i];

                if (showedView != this)
                {
                    count++;
                }
            }

            var targetState = count <= 0;

            ToggleInternal(targetState);

            if (targetState)
            {
                bar.UpdateValue(life.GetProgress, life.GetProgressNext);
            }
        }
    }

    public override void OnViewInContainerToggle(UIBoardView view, bool state)
    {
        UpdateViewInternal();
        
        base.OnViewInContainerToggle(view, state);
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);
        
        if(canvas == null) return;
        
        canvas.overrideSorting = true;
        canvas.sortingOrder = GetLayerIndexBy(new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.UIUP1.Layer));
    }
}