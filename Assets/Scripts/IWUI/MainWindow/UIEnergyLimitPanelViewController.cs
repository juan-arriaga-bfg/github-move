using UnityEngine;

public class UIEnergyLimitPanelViewController: UILimitPanelViewController
{
    [SerializeField] protected GameObject notifyIcon;
    private TimerComponent timer;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        timer = BoardService.Current.FirstBoard.MarketLogic.ResetEnergyTimer;
        timer.OnStart += UpdateNotifyIcon;
        timer.OnComplete += UpdateNotifyIcon;
        UpdateNotifyIcon();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        UpdateNotifyIcon();
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        timer.OnStart -= UpdateNotifyIcon;
        timer.OnComplete -= UpdateNotifyIcon;
    }

    private void UpdateNotifyIcon()
    {
        if (timer == null)
        {
            return;
        }
        
        notifyIcon.SetActive(timer.IsExecuteable() == false);
    }
}
