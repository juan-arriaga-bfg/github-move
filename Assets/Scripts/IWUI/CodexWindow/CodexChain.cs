using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexChain : MonoBehaviour
{
    [SerializeField] private Transform itemsHost;
    [SerializeField] private TextMeshProUGUI caption;

    private const float minItemImageSize = 80;
    private const float maxItemImageSize = 155;
    
    private readonly List<CodexItem> codexItems = new List<CodexItem>();

    public int ChainId { get; private set; }
    
    public Transform ItemsHost => itemsHost;

    public void Init(CodexChainDef def)
    {
        ChainId = def.ItemDefs[0].PieceTypeDef.Id;
        
        if (caption != null)
        {
            caption.text = def.Name;
        }
    }
    
    public void AddItems(List<CodexItem> items)
    {
        foreach (var item in items)
        {
            codexItems.Add(item);
            item.transform.SetParent(itemsHost, false);
        }

        FixItemsSize();
    }

    public void FixItemsSize()
    {
        if (codexItems == null)
        {
            return;
        }

        float itemsCount = codexItems.Count;
        for (int i = 0; i < codexItems.Count; i++)
        {
            var item = codexItems[i];
            var image = item.PieceImage;

            float w = Mathf.Lerp(minItemImageSize, maxItemImageSize, i / itemsCount);
            float h = Mathf.Lerp(minItemImageSize, maxItemImageSize, i / itemsCount);
            
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(w,h);
        }
    }
}