using UnityEngine;

public class ChestButton : IWUIWindowViewController
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;

    private TimerComponent timer;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        timer = BoardService.Current.GetBoardById(0).FreeChestLogic.Timer;
        
        timer.OnStart += UpdateState;
        timer.OnComplete += UpdateState;

        UpdateState();
    }
    
    private void OnDestroy()
    {
        timer.OnStart -= UpdateState;
        timer.OnComplete -= UpdateState;
    }

    private void UpdateState()
    {
        var isActive = !timer.IsExecuteable();
        
        shine.SetActive(isActive);
        exclamationMark.SetActive(isActive);
    }
}