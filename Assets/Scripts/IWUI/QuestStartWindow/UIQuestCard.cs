using TMPro;
using UnityEngine;

public class UIQuestCard : MonoBehaviour
{
    [SerializeField] private UiQuestButton questButton;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private CanvasGroup canvasGroup;

    public CanvasGroup GetCanvasGroup() { return canvasGroup;}

    public void Init(QuestEntity quest)
    {
        questButton.Init(quest, false);
        title.text = LocalizationService.Get("quest.card.new", "quest.card.new");
        message.text = quest?.Tasks[0]?.GetLocalizedTitle() ?? "";
    }
}