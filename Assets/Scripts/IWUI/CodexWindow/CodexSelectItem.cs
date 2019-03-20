using Debug = IW.Logger;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CodexSelectItem : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private NSText itemName;
    [SerializeField] private NSText message;
    [SerializeField] private CodexChain chain;
    [SerializeField] private GameObject questionMark;
    
    [Header("Materials")] 
    [SerializeField] protected Material unlokedMaterial;
    [SerializeField] protected Material lockedMaterial;
    [SerializeField] protected Color unlockedColor;
    [SerializeField] protected Color lockedColor;

    private Transform icon;

    public CodexChain GetChain()
    {
        return chain;
    }
    
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

        if (def != null)
        {
            CreateIcon(anchor, $"{def.Uid}Icon", def, locked);
        }

        if (ShowChain(def, state))
        {
            message.Text = "";
        }
        else
        {
            message.Text = locked ? LocalizationService.Get("piece.description.NPC_Locked", "piece.description.NPC_Locked") : LocalizationService.Get($"piece.description.{def.Uid}", $"piece.description.{def.Uid}");
        }
    }
    
    private void CreateIcon(Transform parent, string id, PieceDef pieceDef, bool locked)
    {        
        var charChain = GameDataService.Current.MatchDefinition.GetChain(pieceDef.Id);
        var isAnyUnlocked = GameDataService.Current.CodexManager.IsAnyPieceUnlockedInChain(charChain);
     
        Debug.Log($"id: {pieceDef.Id} locked: {locked}, isAnyUnlocked: {isAnyUnlocked}");
        questionMark.SetActive(!isAnyUnlocked);

        if (icon != null)
        {
            UIService.Get.PoolContainer.Return(icon.gameObject);
        }
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);

        var iconSprites = icon.GetComponentsInChildren<Image>().ToList();
        foreach (var sprite in iconSprites)
        {
            sprite.material = locked ? lockedMaterial : unlokedMaterial;
            sprite.color    = locked ? lockedColor : unlockedColor;
        }
    }
    
    private bool ShowChain(PieceDef def, CodexItemState state)
    {
        chain.ReturnContentToPool();

        if (def == null)
        {
            return false;
        }

        if (GameDataService.Current.CodexManager.IsPieceUnlocked(def.Id) && PieceType.GetDefById(def.Id).Filter.Has(PieceTypeFilter.Character))
        {
            return false;
        }
        
        int targetId = def.Id;
        var piecesChain = GameDataService.Current.MatchDefinition.GetChain(targetId);
        var codexManager = GameDataService.Current.CodexManager;
        
        bool anyItemUnlocked = false;
        foreach (var id in piecesChain)
        {
            if (codexManager.IsPieceUnlocked(id))
            {
                anyItemUnlocked = true;
                break;
            }
        }

        if (!anyItemUnlocked)
        {
            return false;
        }

        int startIndex = GameDataService.Current.CodexManager.GetCharChainStartIndex(def.Id);
        
        // Do not include the last one item (hide a char)
        var itemDefs = GameDataService.Current.CodexManager.GetCodexItemsForChainStartingFrom(targetId, startIndex, 1, 
            CodexDataManager.CHAR_CHAIN_VISIBLE_COUNT, true, true, false);
        if (itemDefs == null)
        {
            return false;
        }

        CodexChainDef chainDef = new CodexChainDef {ItemDefs = itemDefs};
        
        UICodexWindowView.CreateItems(chain, chainDef, CodexDataManager.CHAR_CHAIN_VISIBLE_COUNT);

        return true;
    }


}
