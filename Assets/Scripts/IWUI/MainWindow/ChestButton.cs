using UnityEngine;

public class ChestButton : UIGenericResourcePanelViewController
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;

    private TimerComponent timer;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        timer = BoardService.Current.FirstBoard.FreeChestLogic.Timer;
        
        timer.OnStart += UpdateTimerState;
        timer.OnComplete += UpdateTimerState;

        UpdateTimerState();
    }
    
    private void OnDestroy()
    {
        timer.OnStart -= UpdateTimerState;
        timer.OnComplete -= UpdateTimerState;
    }

    public override void UpdateResource(int offset)
    {
        base.UpdateResource(offset);
        UpdateTimerState();
    }

    private void UpdateTimerState()
    {
        var isActive = !timer?.IsExecuteable() ?? false;
        
        shine.SetActive(isActive);
        exclamationMark.SetActive(isActive);
    }
}