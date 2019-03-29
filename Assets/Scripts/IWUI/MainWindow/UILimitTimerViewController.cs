using DG.Tweening;
using UnityEngine;

public class UILimitTimerViewController : IWUIWindowViewController
{
    [SerializeField] private RectTransform body;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private NSText label;

    private EnergyCurrencyLogicComponent energyLogic;
    private bool isShow;

    public override void OnViewInit(IWUIWindowView context)
    {
        base.OnViewInit(context);
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);
        
        var board = BoardService.Current.FirstBoard;
        
        energyLogic = board.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        energyLogic.OnExecute += UpdateView;
        energyLogic.Timer.OnTimeChanged += UpdateView;
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
        
        energyLogic.OnExecute -= UpdateView;
        energyLogic.Timer.OnTimeChanged -= UpdateView;
    }

    private void UpdateView()
    {
        label.Text = energyLogic.Timer.CompleteTime.GetTimeLeftText(true, true);
        Change(energyLogic.CheckIsNeed());
    }

    private void Change(bool isShow)
    {
        if(this.isShow == isShow) return;
        
        DOTween.Kill(body);
        
        var sequence = DOTween.Sequence().SetId(body);

        sequence.Insert(0, body.DOAnchorPosY(isShow ? -58 : -18, 0.5f).SetEase(Ease.OutExpo));
        sequence.Insert(isShow ? 0.1f : 0, canvas.DOFade(isShow ? 1 : 0, 0.3f));

        this.isShow = isShow;
    }
}