using TMPro;
using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField] private RectTransform head;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject headActive;
    [SerializeField] private GameObject headDisabled;

    public TabGroup TabGroup;
    public int Index;

    private void Start()
    {
        var headerStartPos = head.anchoredPosition.x;
        var offsetX = Index * head.sizeDelta.x;
        head.anchoredPosition = new Vector2(headerStartPos + offsetX, head.anchoredPosition.y);
    }

    public void Toggle(bool enabled)
    {
        // Debug.Log($"Tab {Index} toggle to {enabled}");
        
        content.SetActive(enabled);
        headActive.SetActive(enabled);
        headDisabled.SetActive(!enabled);
    }

    public void OnClick()
    {
        // Debug.Log("Tab click: " + Index);
        
        TabGroup.Click(Index);
    }
}