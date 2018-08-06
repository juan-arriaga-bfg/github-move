using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField] private TabGroup tabGroup;
    
    [SerializeField] private RectTransform head;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject captionActive;
    [SerializeField] private GameObject captionDisabled;

    [SerializeField] public int Index { get; }

    private void Start()
    {
        head.anchoredPosition = new Vector2(head.anchoredPosition.x + Index * head.sizeDelta.x, head.anchoredPosition.y);
    }

    public void Toggle(bool enabled)
    {
        content.SetActive(enabled);
        captionActive.SetActive(enabled);
        captionDisabled.SetActive(!enabled);
    }

    public void OnClick()
    {
        tabGroup.Click(Index);
    }
}