using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductionItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonClaim;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject targets;
    [SerializeField] private GameObject pattern;
    
    [SerializeField] private HorizontalLayoutGroup group;

    private List<ProductionViewItem> items = new List<ProductionViewItem>();

    private RectTransform body;
    private ProductionComponent production;

    public void Init(ProductionComponent data)
    {
        RemoveTimerAction();
        
        production = data;
        body = GetComponent<RectTransform>();
        
        production.Timer.OnExecute += UpdateTimer;
        production.Timer.OnStart += Decoration;
        production.Timer.OnComplete += Decoration;
        
        icon.sprite = IconService.Current.GetSpriteById(production.Target);
        button.SetActive(production.State == ProductionState.Full);
        timer.SetActive(production.State == ProductionState.Waiting);
        shine.SetActive(production.State == ProductionState.Completed);
        buttonClaim.SetActive(production.State == ProductionState.Completed);
        targets.SetActive(production.State != ProductionState.Waiting && production.State != ProductionState.Completed);
        
        while (items.Count > production.Storage.Count)
        {
            var i = items.Count - 1;
            var item = items[i];
            
            items.RemoveAt(i);
            Destroy(item.gameObject);
        }
        
        var index = 0;
        
        foreach (var pair in production.Storage)
        {
            if (index == items.Count)
            {
                pattern.SetActive(true);
                var item = Instantiate(pattern, pattern.transform.parent).GetComponent<ProductionViewItem>();
                items.Add(item);
            }
            
            items[index].Init(pair.Key, pair.Value.Value, pair.Value.Key, 26);
            index++;
        }
        
        pattern.SetActive(false);
        group.spacing = items.Count == 2 ? 20 : 5;
    }

    private void OnDestroy()
    {
        RemoveTimerAction();
    }

    private void UpdateTimer()
    {
        timerLabel.Text = production.Timer.CompleteTime.GetTimeLeftText();
    }

    private void Decoration()
    {
        if(production == null) return;
        
        Init(production);
    }

    private void RemoveTimerAction()
    {
        if(production == null) return;

        production.Timer.OnExecute -= UpdateTimer;
        production.Timer.OnStart -= Decoration;
        production.Timer.OnComplete -= Decoration;
    }

    public bool Check(int resource, Vector3 pos)
    {
        var v = new Vector3[4];
        body.GetWorldCorners(v);
        
        var rect = new Rect(v[0], v[2]-v[0]);
        var isAdd = rect.Contains(pos) && production.AddViaUI(resource);

        if (isAdd && production.State != ProductionState.Full)
        {
            UIService.Get.GetShowedView<UIProductionWindowView>(UIWindowType.ProductionWindow).Change(false);
        }
        else
        {
            Decoration();
        }
        
        return isAdd;
    }

    public void OnClick()
    {
        switch (production.State)
        {
            case ProductionState.Full:
                production.Change();
                production.Start();
                break;
            case ProductionState.Waiting:
                UIMessageWindowController.CreateTimerCompleteMessage(
                    "Complete now!",
                    "Would you like to build the item right now for crystals?",
                    "Complete now ",
                    production.Timer,
                    () => CurrencyHellper.Purchase(Currency.Product.Name, 1, production.Timer.GetPrise(), success =>
                    {
                        if(success == false) return;
                        production.Fast();
                    }));
                break;
            case ProductionState.Completed:
                UIService.Get.GetShowedView<UIProductionWindowView>(UIWindowType.ProductionWindow).Change(false);
                production.CompleteViaUI();
                break;
            default:
                return;
        }
    }
}