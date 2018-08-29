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

        var board = BoardService.Current.GetBoardById(0);
        
        energyLogic = board.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        energyLogic.OnExecute += UpdateView;
        energyLogic.Timer.OnExecute += UpdateView;
    }

    private void UpdateView()
    {
        label.Text = energyLogic.Timer.CompleteTime.GetTimeLeftText(true);
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