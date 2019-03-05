using UnityEngine;

public class CodexSelectItem : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private NSText itemName;
    [SerializeField] private NSText message;
    [SerializeField] private CodexChain chain;

    private Transform icon;
    
    private const int CHAIN_LENGTH = 6;
    
    public void SetItem(PieceDef def, CodexItemState state)
    {
        bool locked = state == CodexItemState.FullLock || state == CodexItemState.PartLock || def == null;
        
        itemName.Text = locked ? LocalizationService.Get("piece.name.NPC_Locked", "piece.name.NPC_Locked") : def.Name;
        message.Text  = locked ? LocalizationService.Get("piece.description.NPC_Locked", "piece.description.NPC_Locked") : LocalizationService.Get($"piece.description.{def.Uid}", $"piece.description.{def.Uid}");
        
        CreateIcon(anchor, def == null ? "NoneIcon" : $"{def.Uid}Icon");
        ShowChain(def, state);
    }
    
    private void CreateIcon(Transform parent, string id)
    {
        if (icon != null) UIService.Get.PoolContainer.Return(icon.gameObject);
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);
    }
    
    private bool ShowChain(PieceDef def, CodexItemState state)
    {
        chain.ReturnContentToPool();

        if (state == CodexItemState.FullLock)
        {
            return false;
        }

        if (def == null)
        {
            return false;
        }

        int targetId = def.Id;

        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainAndFocus(targetId, CHAIN_LENGTH, true, true, false);
        if (itemDefs == null)
        {
            return false;
        }
        
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        
        UICodexWindowView.CreateItems(chain, chainDef, CHAIN_LENGTH);

        return true;
    }
}
