using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField] private TabGroup tabGroup;
    
    [SerializeField] private RectTransform head;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject captionActive;
    [SerializeField] private GameObject captionDisabled;

    [SerializeField] private int index;

    private float headerStartPos;
    
    private void Start()
    {
        headerStartPos = head.anchoredPosition.x;
    }

    public void Toggle(bool enabled)
    {
        content.SetActive(enabled);
        captionActive.SetActive(enabled);
        captionDisabled.SetActive(!enabled);

        var offsetX = index * head.sizeDelta.x;
        head.anchoredPosition = new Vector2(headerStartPos + offsetX, head.anchoredPosition.y);
    }

    public void OnClick()
    {
        Debug.Log("Tab click: " + index);
        
        tabGroup.Click(index);
    }
}