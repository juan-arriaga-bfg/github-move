using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIRobberyWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnSendLabel;
    [SerializeField] private NSText btnClaimLabel;
    
    [SerializeField] private Button btnSend;
    [SerializeField] private Button btnClaim;
    
    [SerializeField] private GameObject round;
    [SerializeField] private GameObject dot;
    
    [SerializeField] private UIRobberyItem mainChest;
    
    [SerializeField] private Image chest;
    
    [SerializeField] private RectTransform line;

    private int reward;

    private bool isClaimReward;

    private List<UIRobberyItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();

        isClaimReward = false;
        
        var windowModel = Model as UIRobberyWindowModel;
        var steps = windowModel.Enemy.Def.Steps;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnSendLabel.Text = windowModel.SendText;
        btnClaimLabel.Text = windowModel.ClaimText;

        var max = steps[steps.Count - 1].Amount;
        
        mainChest.Decoration(max, max);

        for (var i = 0; i < steps.Count - 1; i++)
        {
            if(steps[i].Amount <= windowModel.Enemy.Damage) continue;
            
            var item = Instantiate(dot, dot.transform.parent);
            
            item.GetComponent<UIRobberyItem>().Decoration(steps[i].Amount, max);
        }
        
        dot.SetActive(false);

        items = GetComponentsInChildren<UIRobberyItem>().ToList();
        
        Decoration();
    }
    
    private void Decoration()
    {
        var windowModel = Model as UIRobberyWindowModel;
        
        line.sizeDelta = new Vector2(Mathf.Clamp(455 * windowModel.Enemy.Progress, 0, 455), line.sizeDelta.y);

        reward = windowModel.Enemy.GetReward();

        var isNone = reward == PieceType.None.Id;

        btnSend.interactable = isNone;
        btnClaim.interactable = !isNone;
        
        round.SetActive(windowModel.Enemy.IsComplete);
        chest.sprite = IconService.Current.GetSpriteById(string.Format("Chest_{0}active", windowModel.Enemy.IsComplete ? "" : "not_"));
        
        foreach (var item in items)
        {
            item.Check(windowModel.Enemy.Damage);
        }
    }
    
    public override void OnViewCloseCompleted()
    {
        if(isClaimReward == false) return;

        isClaimReward = false;
        
        var windowModel = Model as UIRobberyWindowModel;
        
        windowModel.View.SpawnReward(reward);
        
        windowModel.Enemy = null;

        foreach (var item in items)
        {
            if(item.gameObject == dot || item == mainChest) continue;
            
            Destroy(item.gameObject);
        }
        
        items = new List<UIRobberyItem>();
        
        dot.SetActive(true);
    }

    public void OnClickSend()
    {
        var windowModel = Model as UIRobberyWindowModel;
        
        windowModel.Enemy.SetDamage(3);
        windowModel.View.UpdateFill();
        Decoration();
    }
    
    public void OnClickClaim()
    {
        isClaimReward = true;
        Controller.CloseCurrentWindow();
    }
    
    public void OnClickQuestion()
    {
        
    }
}