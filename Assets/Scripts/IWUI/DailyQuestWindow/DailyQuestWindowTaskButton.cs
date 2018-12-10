using Quests;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestWindowTaskButton : IWBaseMonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private NSText label;
    
    [SerializeField] private Sprite find;
    [SerializeField] private Sprite claim;

    public void Init(TaskEntity task)
    {
        button.interactable = !task.IsClaimed();
        label.Text = GetTextForButton(task);

        image.sprite = task.IsCompleted() ? claim : find;
        
        gameObject.SetActive(task.State != TaskState.Claimed);
    }
    
    private string GetTextForButton(TaskEntity task)
    {
        if (task is TaskCompleteDailyTaskEntity && !GameDataService.Current.QuestsManager.DailyQuest.IsAllTasksClaimed(true))
        {
             return LocalizationService.Get("window.daily.quest.task.button.help", "window.daily.quest.task.button.help");
        }
        
        string text;
        
        switch (task.State)
        {
            case TaskState.Completed:
                text = LocalizationService.Get("window.daily.quest.task.button.claim", "window.daily.quest.task.button.claim");
                break;
            
            case TaskState.Claimed:
                text = LocalizationService.Get("window.daily.quest.task.button.done", "window.daily.quest.task.button.done");
                break;
            
            default:
                text = LocalizationService.Get("window.daily.quest.task.button.help", "window.daily.quest.task.button.help");
                break;
        }

        return text;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}