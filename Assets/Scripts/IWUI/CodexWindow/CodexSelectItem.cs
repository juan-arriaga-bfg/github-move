using UnityEngine;

public class CodexSelectItem : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private NSText itemName;
    [SerializeField] private NSText message;

    private Transform icon;
    
    public void SetItem(PieceDef def)
    {
        itemName.Text = def == null ? LocalizationService.Get("piece.name.NPC_Locked", "piece.name.NPC_Locked") : def.Name;
        message.Text = def == null ?  LocalizationService.Get("piece.description.NPC_Locked", "piece.description.NPC_Locked") : LocalizationService.Get($"piece.description.{def.Uid}", $"piece.description.{def.Uid}");
        
        CreateIcon(anchor, def == null ? "NoneIcon" : $"{def.Uid}Icon");
    }
    
    private void CreateIcon(Transform parent, string id)
    {
        if (icon != null) UIService.Get.PoolContainer.Return(icon.gameObject);
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(parent);
    }
}
