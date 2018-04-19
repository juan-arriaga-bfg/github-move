using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiChestSlot : MonoBehaviour
{
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    [SerializeField] private Image button;
    
    [SerializeField] private GameObject currencyIcon;
    [SerializeField] private GameObject timerGo;
    [SerializeField] private GameObject chestGo;
    [SerializeField] private GameObject buttonGo;
    [SerializeField] private GameObject shineGo;
    
    private Chest chest;

    private bool isTimerWork;

    private UIHintArrowViewController cachedHintArrow;
    
    public Action<UiChestSlot, ChestState> OnChestStateChanged { get; set; }

    public Chest Chest
    {
        get { return chest; }
        set { chest = value; }
    }

    public virtual void ToggleHintArrow(bool state)
    {
        if (cachedHintArrow != null)
        {
            UIService.Get.ReturnCachedObject(cachedHintArrow.gameObject);
        }
        
        if (state == false) return;
        
        cachedHintArrow= UIService.Get.GetCachedObject<UIHintArrowViewController>(R.UIHintArrow);
        cachedHintArrow.CachedTransform.SetParentAndReset(transform);
        // set arrow offset
        cachedHintArrow.CachedTransform.localPosition = new Vector3(0f, 56.9f, 0f);
        cachedHintArrow.FadeInOut(state);
    }
    
    public void Init(Chest chest)
    {
        isTimerWork = false;
        
        if (chest == null)
        {
            timerGo.SetActive(false);
            chestGo.SetActive(false);
            buttonGo.SetActive(false);
            
            return;
        }

        this.chest = chest;
        
        var state = chest.State;
        
        if (OnChestStateChanged != null)
        {
            OnChestStateChanged(this, chest.State);
        }
        
        chestGo.SetActive(true);
        timerGo.SetActive(state == ChestState.InProgress);
        buttonGo.SetActive(true);
        currencyIcon.SetActive(state == ChestState.InProgress);
        shineGo.SetActive(state == ChestState.Open);
        
        top.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, state == ChestState.Open ? 45 : 15);
        
        var id = "";
        var color = "FAF9ADFF";
        
        top.sprite = IconService.Current.GetSpriteById(id + "_2");
        bottom.sprite = IconService.Current.GetSpriteById(id + "_1");

        switch (state)
        {
            case ChestState.Close:
                buttonLabel.Text = string.Format("<color=#{1}>UNLOCK IN </color><size=35>{0}</size>", chest.GetTimeText(), color);
                button.sprite = IconService.Current.GetSpriteById("btn_orange_norm");
                break;
            case ChestState.InProgress:
                buttonLabel.Text = string.Format("<size=40>{0}</size> <color=#{1}>OPEN</color>", this.chest.Def.Price.Amount, color);
                button.sprite = IconService.Current.GetSpriteById("btn_blue_norm");
                break;
            case ChestState.Open:
                buttonLabel.Text = string.Format("<color=#{0}>TAP TO OPEN</color>", color);
                button.sprite = IconService.Current.GetSpriteById("btn_green_norm");
                break;
            case ChestState.Finished:
                break;
        }
    }

    private void Update()
    {
        if (chest == null || chest.State != ChestState.InProgress)
        {
            if (isTimerWork)
            {
                isTimerWork = false;
                Init(chest);
            }
            
            return;
        }

        timerLabel.Text = chest.GetTimeLeftText();
        isTimerWork = true;
    }
    
    public void OnClick()
    {
        if (chest == null)
        {
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
            model.Title = "Need chests?";
            model.Message = null;
            model.Image = "Chest_UI";
            model.AcceptLabel = "Ok";
            
            model.OnCancel = null;
            model.OnAccept = () =>
            {
                var cache = BoardService.Current.GetBoardById(0).BoardLogic.PositionsCache;
                var pos = cache.GetRandomPositions(PieceType.O1.Id, 1);

                if (pos.Count == 0)
                {
                    pos = cache.GetRandomPositions(PieceType.O2.Id, 1);
                }

                if (pos.Count == 0)
                {
                    return;
                }
                
                HintArrowView.Show(pos[0]);
            };
        
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            return;
        }
        
        switch (chest.State)
        {
            case ChestState.Close:
                StartTimer();
                break;
            case ChestState.InProgress:
                var model2 = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                model2.Title = "Message";
                model2.Message = string.Format("Are you sure you want to spend {0} crystals to Speed Up the chest?", chest.Def.Price.Amount);
                model2.AcceptLabel = "Ok";
                model2.CancelLabel = "Cancel";

                model2.OnCancel = () => {};
                model2.OnAccept = () =>
                {
                    var shopItem = new ShopItem
                    {
                        Uid = string.Format("purchase.test.{0}.10", chest.Currency), 
                        ItemUid = chest.Currency, 
                        Amount = 1,
                        CurrentPrices = new List<Price>
                        {
                            new Price{Currency = chest.Def.Price.Currency, DefaultPriceAmount = chest.Def.Price.Amount}
                        }
                    };
        
                    ShopService.Current.PurchaseItem
                    (
                        shopItem,
                        (item, s) =>
                        {
                            // on purchase success
                            chest.State = ChestState.Open;
                            Init(chest);
                        },
                        item =>
                        {
                            // on purchase failed (not enough cash)
                            UIMessageWindowController.CreateDefaultMessage("Not enough crystals!");
                        }
                    );
                };
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
                break;
            case ChestState.Open:
                chest.State = ChestState.Finished;
                if (OnChestStateChanged != null)
                {
                    OnChestStateChanged(this, chest.State);
                }
                GameDataService.Current.ChestsManager.RemoveActiveChest(chest);
                
                var model = UIService.Get.GetCachedModel<UIChestRewardWindowModel>(UIWindowType.ChestRewardWindow);

                model.Chest = GameDataService.Current.ChestsManager.GetChest(chest.Piece);
        
                UIService.Get.ShowWindow(UIWindowType.ChestRewardWindow);
                
                var main = UIService.Get.GetShowedWindowByName(UIWindowType.MainWindow).CurrentView as UIMainWindowView;
				
                main.UpdateSlots();
                break;
            case ChestState.Finished:
                break;
        }
    }

    private void StartTimer()
    {
        foreach (var def in GameDataService.Current.ChestsManager.ActiveChests)
        {
            if (def.State != ChestState.InProgress) continue;
            
            var model1 = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
            model1.Title = "Another chest unlocking";
            model1.Message = "Finish unlock another chest first";
            model1.AcceptLabel = "Ok";
                
            model1.OnAccept = () => {};
        
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            
            return;
        }
        
        var model2 = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model2.Title = string.Format("{0} chest", chest.Currency);
        model2.Message = string.Format("Unlock time: {0}", chest.GetTimeText());
        model2.AcceptLabel = "Start unlock";
                
        model2.OnAccept = () =>
        {
            chest.State = ChestState.InProgress;
            Init(chest);
        };
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
}