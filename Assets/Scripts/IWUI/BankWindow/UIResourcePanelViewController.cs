using DG.Tweening;
using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] private UIButtonViewController button;
    
    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelText(currentValueAnimated);
        }
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        button.OnClick(OpenShop);
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        SetLabelText(storageItem.Amount);
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }
    
    public override void UpdateLabel(int value)
    {
        if (amountLabel == null) return;
        
        DOTween.Kill(amountLabel);
        
        var sequence = DOTween.Sequence().SetId(amountLabel);
        sequence.Insert(0f, DOTween.To(() => CurrentValueAnimated, (v) => { CurrentValueAnimated = v; }, value, 0.5f ));
        sequence.Insert(0f, icon.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f)).SetEase(Ease.InSine);
        sequence.Insert(0.3f, icon.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f)).SetEase(Ease.OutSine);
    }

    public void OpenShop()
    {
        if (UIService.Get.GetShowedWindowsCount(UIWindowType.IgnoredWindows) > 0)
        {
            return;
        }
        
        if (UIService.Get.GetCachedModel<UIMainWindowModel>(UIWindowType.MainWindow).IsTutorial)
        {
            UIErrorWindowController.AddError(LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            return;
        }
        
        if (GameDataService.Current.QuestsManager.GetActiveQuestById("1_CreatePiece_PR_C4") != null)
        {
            UIMessageWindowController.CreateMessage(
                LocalizationService.Get("common.title.forbidden", "common.title.forbidden"),
                LocalizationService.Get("common.message.forbidden", "common.message.forbidden"));
            
            return;
        }
        
        CurrencyHelper.OpenShopWindow(itemUid);
    }

    private void SetLabelText(int value)
    {
        if (amountLabel == null) return;

        value = Mathf.Max(0, value);
        amountLabel.Text = $"<mspace=2.7em>{value}</mspace>";
    }
}