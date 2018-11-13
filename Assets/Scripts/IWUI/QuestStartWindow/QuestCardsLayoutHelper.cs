using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestCardsLayoutHelper : MonoBehaviour
{
    [SerializeField] private RectTransform card;
    [SerializeField] private RectTransform canvas;
    
    [SerializeField] private HorizontalLayoutGroup horLayout;
    
    [SerializeField] private float topPadding;
    [SerializeField] private float bottomPadding;
    
    private void Start()
    {
        
    }
    
    private IEnumerator UpdateLayoutCoroutine()
    {
        yield return new WaitForEndOfFrame();
        FixLayout();
    }

    public void FixLayoutAtTheNextFrame()
    {
        StartCoroutine(UpdateLayoutCoroutine()); 
    }
    
    public void FixLayout()
    {
        // Canvas.ForceUpdateCanvases();
        // LayoutRebuilder.ForceRebuildLayoutImmediate(canvas);
        
        float screenH = canvas.sizeDelta.y;
        float cardH = card.sizeDelta.y;

        horLayout.padding.bottom = (int) (bottomPadding);

        var scale = Vector3.one;
        if (screenH - bottomPadding - topPadding - cardH < 0)
        {
            float freeSpace = screenH - bottomPadding - topPadding;
            float ratio = Mathf.Abs(freeSpace / cardH);
            scale *= ratio;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localScale = scale;
        }
        
        // Canvas.ForceUpdateCanvases();
        // LayoutRebuilder.ForceRebuildLayoutImmediate(canvas);
        
        // horLayout.CalculateLayoutInputHorizontal();
        // horLayout.CalculateLayoutInputVertical();
    }

    private void Update()
    {
        //DoIt();
    }
}
