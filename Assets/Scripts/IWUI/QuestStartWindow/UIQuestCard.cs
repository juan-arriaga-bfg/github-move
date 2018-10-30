using TMPro;
using UnityEngine;

public class UIQuestCard : MonoBehaviour
{
    [SerializeField] private UiQuestButton questButton;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;

    public void Init(QuestEntity quest)
    {
        questButton.Init(quest, false);
        title.text = "New quest";
    }
}