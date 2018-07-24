﻿using UnityEngine;

public class BoardTimerView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private TimerComponent timer;
    
    protected override ViewType Id
    {
        get { return ViewType.BoardTimer; }
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Ofset = multiSize == 1 ? Ofset : new Vector3(0, 1.3f);
        SetOfset();

        Priority = defaultPriority = 10;
        timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);

        timer.OnExecute += UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        timer.OnExecute -= UpdateView;
        base.ResetViewOnDestroy();
    }

    protected override void UpdateView()
    {
        if(progressBar != null) progressBar.SetProgress((float)timer.GetTime().TotalSeconds / timer.Delay);
        label.Text = timer.GetTimeLeftText(null);
    }
}