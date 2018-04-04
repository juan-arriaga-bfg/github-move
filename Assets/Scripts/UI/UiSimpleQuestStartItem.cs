using UnityEngine;
using UnityEngine.UI;

public class UiSimpleQuestStartItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    
    [SerializeField] private NSText progressLabel;
    
    [SerializeField] private RectTransform progress;
    
    [SerializeField] private GameObject btn;
    [SerializeField] private GameObject check;

    private bool isComplete;
    
    public void Init(Quest quest)
    {
        var current = quest.CurrentAmount;
        var target = quest.TargetAmount;
        
        isComplete = quest.Check();
        
        background.sprite = IconService.Current.GetSpriteById(string.Format("back_is_{0}", isComplete.ToString().ToLower()));
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
        
        progressLabel.Text = string.Format("{0}/{1}", current, target);
        progress.sizeDelta = new Vector2(Mathf.Clamp(145 * current / (float) target, 0, 145), progress.sizeDelta.y);
        
        btn.SetActive(!isComplete);
        check.SetActive(isComplete);
    }

    public void OnClick()
    {
        if(isComplete) return;
        
        UIMessageWindowController.CreateDefaultMessage("Find!!!");
    }
}