using UnityEngine;
using UnityEngine.UI;

public class CastleUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText progress;
    [SerializeField] private GameObject check;

    private Quest quest;

    public void Init(Quest quest)
    {
        this.quest = quest;
        icon.sprite = IconService.Current.GetSpriteById(quest.WantedIcon);
        check.SetActive(false);
        
        Decoration();
    }
    
    private void Decoration()
    {
        if (quest.Check())
        {
            check.SetActive(true);
            progress.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            return;
        }
        
        progress.Text = string.Format("<color=#FE4704><size=40>{0}</size></color>/{1}", quest.CurrentAmount, quest.TargetAmount);
    }
    
    public bool IsComplete
    {
        get { return quest.Check(); }
    }
}