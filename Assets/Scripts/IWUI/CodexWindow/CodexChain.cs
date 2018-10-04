using Boo.Lang;
using TMPro;
using UnityEngine;

public class CodexChain : MonoBehaviour
{
    [SerializeField] private Transform itemsHost;
    [SerializeField] private TextMeshProUGUI caption;
    
    private readonly List<CodexItem> codexItems = new List<CodexItem>();

    public int ChainId { get; private set; }
    
    public Transform ItemsHost
    {
        get { return itemsHost; }
    }

    public void Init(CodexChainDef def)
    {
        ChainId = def.ItemDefs[0].PieceTypeDef.Id;
        
        if (caption != null)
        {
            caption.text = def.Name;
        }
    }
    
    public void AddItem(CodexItem item)
    {
        codexItems.Add(item);
        item.transform.SetParent(itemsHost, false);
    }
}