using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexItem : MonoBehaviour
{
    [SerializeField] private Transform pieceHost;
    [SerializeField] private Image piece;
    [SerializeField] private TextMeshProUGUI caption;
    [SerializeField] private GameObject arrow;
    
    public void Init(CodexItemDef itemDef)
    {
        caption.text = itemDef.PieceTypeDef.Abbreviations[0];
        arrow.SetActive(!itemDef.IsLast);

        var sprite = IconService.Current.GetSpriteById(PieceType.Parse(itemDef.PieceTypeDef.Id));
        piece.sprite = sprite;
    }
}