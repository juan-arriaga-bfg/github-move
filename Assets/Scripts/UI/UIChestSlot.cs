using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIChestSlot : MonoBehaviour
{
    [SerializeField] private NSText timerLabel;
    [SerializeField] private NSText slotLabel;
    [SerializeField] private NSText btnLabel;
    
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject currensy;
    
    [SerializeField] private Image icon;
    [SerializeField] private Image iconOpenTop;
    [SerializeField] private Image iconOpenDown;
    
    [SerializeField] private Image btnImage;
    [SerializeField] private Image slotImage;

    private ChestDef chest;
    
    public void Initialize(ChestDef chest = null)
    {
        this.chest = chest;
        
        slotLabel.Text = string.Format("<color=#FFFEAF>{0}</color>", "CHEST SLOT");

        bool chestIconState = icon.gameObject.activeSelf;
        
        slotImage.color = chest == null ? new Color(74/255f, 74/255f, 74/255f, 1f) : Color.white;
        button.SetActive(chest != null);
        icon.gameObject.SetActive(chest != null);
        timer.SetActive(chest != null);
        iconOpenTop.gameObject.SetActive(chest != null);
        iconOpenDown.gameObject.SetActive(chest != null);
        icon.gameObject.SetActive(chest != null);
        
        if (chest == null) return;
        
        var state = chest.State;
        
        timer.SetActive(state == ChestState.InProgres);
        currensy.SetActive(state == ChestState.InProgres);
        iconOpenTop.gameObject.SetActive(state == ChestState.Open);
        iconOpenDown.gameObject.SetActive(state == ChestState.Open);
        icon.gameObject.SetActive(state != ChestState.Open);

        if (chestIconState == false && icon.gameObject.activeSelf)
        {
            DOTween.Kill(icon);
            icon.transform.localScale = Vector3.zero;
            icon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetId(icon);
        }

        switch (state)
        {
            case ChestState.Lock:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_orange_norm");
                btnLabel.Text = string.Format("<color=#FFFEAF>UNLOCK IN</color> <size=60>{0}m</size>", new TimeSpan(0, 0, chest.Time).TotalMinutes);
                break;
            case ChestState.InProgres:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_blue_norm");
                btnLabel.Text = string.Format("<size=60>{0}</size> <color=#FFFEAF>OPEN</color>", chest.Price.Amount);
                break;
            case ChestState.Open:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_green_norm");
                btnLabel.Text = "<color=#FFFEAF>TAP TO OPEN</color>";
                break;
        }

        var id = chest.GetSkin();
        
        icon.sprite = IconService.Current.GetSpriteById(id);
        iconOpenTop.sprite = IconService.Current.GetSpriteById(id + "_3");
        iconOpenDown.sprite = IconService.Current.GetSpriteById(id + "_1");
    }

    private void Update()
    {
        if(chest != null && chest.State == ChestState.Open) Initialize(chest);
        if(chest == null || chest.State != ChestState.InProgres) return;
        
        var text = chest.GetCurrentTime();
        timerLabel.Text = text;
        btnLabel.Text = string.Format("<size=60>{0}</size> <color=#FFFEAF>OPEN</color>", 5 * (chest.GetCurrentTimeInTimeSpan().Minutes + 1));
        
        if (chest.State != ChestState.InProgres) Initialize(chest);
    }

    public void OnClick()
    {
        if(chest == null || chest.State == ChestState.None) return;
        
        if (chest.State == ChestState.Open)
        {
            var chestRewardmodel = UIService.Get.GetCachedModel<UIChestRewardWindowModel>(UIWindowType.ChestRewardWindow);

            chestRewardmodel.Chest = chest;
            GameDataService.Current.ChestsManager.RemoveActiveChest(chest);
            Initialize();
            
            UIService.Get.ShowWindow(UIWindowType.ChestRewardWindow);
            return;
        }
        
        var chestWindowmodel = UIService.Get.GetCachedModel<UIChestWindowModel>(UIWindowType.ChestWindow);

        chestWindowmodel.Chest = chest;
        
        UIService.Get.ShowWindow(UIWindowType.ChestWindow);
    }
}