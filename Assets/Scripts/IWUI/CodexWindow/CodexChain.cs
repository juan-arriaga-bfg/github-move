using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexChain : MonoBehaviour
{
    [SerializeField] private Transform itemsHost;
    [SerializeField] private TextMeshProUGUI caption;
   
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
        ReturnContentToPool();
        
        foreach (var item in items)
        {
            codexItems.Add(item);
            item.transform.SetParent(itemsHost, false);
        }

        FixItemsSize();
    }

    public void ReturnContentToPool()
    {
        if (codexItems == null)
        {
            return;
        }

        foreach (var item in codexItems)
        {
            UIService.Get.PoolContainer.Return(item.gameObject);
        }
        
        codexItems.Clear();
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

            var index = i;
            
            // Force min size for Ingredients
            if (item.Def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient))
            {
                index = 0;
            } 
            // Fors max size for multicell
            else if (item.Def.PieceTypeDef.Filter.Has(PieceTypeFilter.Multicellular))
            {
                index = (int)itemsCount;
            }
            
            float w = Mathf.Lerp(CodexItem.MIN_ITEM_IMAGE_SIZE, CodexItem.MAX_ITEM_IMAGE_SIZE, index / itemsCount);
            float h = Mathf.Lerp(CodexItem.MIN_ITEM_IMAGE_SIZE, CodexItem.MAX_ITEM_IMAGE_SIZE, index / itemsCount);

            item.PieceImageRectTransform.sizeDelta = new Vector2(w,h);
            item.SetCaption($"{(int)w}x{(int)h}");
        }
    }
}