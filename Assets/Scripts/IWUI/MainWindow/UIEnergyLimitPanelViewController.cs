using UnityEngine;

public class UIEnergyLimitPanelViewController: UILimitPanelViewController
{
    [SerializeField] private UIButtonViewController button;
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

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        button.OnClick(OpenShop);
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
