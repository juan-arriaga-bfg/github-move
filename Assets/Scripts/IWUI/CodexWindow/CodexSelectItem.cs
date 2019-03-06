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
        bool locked;
        if (def == null)
        {
            locked = true;
        }
        else if (state == CodexItemState.PendingReward || state == CodexItemState.Unlocked || GameDataService.Current.CodexManager.IsPieceUnlocked(def.Id))
        {
            locked = false;
        }
        else
        {
            locked = true;
        }

        itemName.Text = locked ? LocalizationService.Get("piece.name.NPC_Locked", "piece.name.NPC_Locked") : def.Name;
                
        CreateIcon(anchor, def == null ? "NoneIcon" : $"{def.Uid}Icon");

        if (ShowChain(def, state))
        {
            message.Text = "";
        }
        else
        {
            message.Text = locked ? LocalizationService.Get("piece.description.NPC_Locked", "piece.description.NPC_Locked") : LocalizationService.Get($"piece.description.{def.Uid}", $"piece.description.{def.Uid}");
        }
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

        if (GameDataService.Current.CodexManager.IsPieceUnlocked(def.Id) && PieceType.GetDefById(def.Id).Filter.Has(PieceTypeFilter.Character))
        {
            return false;
        }
        
        int targetId = def.Id;

        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainStartingFrom(targetId, 0, CHAIN_LENGTH, true, true, false);
        if (itemDefs == null)
        {
            return false;
        }
        
        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        
        UICodexWindowView.CreateItems(chain, chainDef, CHAIN_LENGTH);

        return true;
    }
}
