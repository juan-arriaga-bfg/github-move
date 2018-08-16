using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CodexItem : MonoBehaviour
{
    [Header("Materials")] 
    [SerializeField] private Material unlokedMaterial;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color lockedColor;
    
    [Header("References")]
    [SerializeField] private Transform pieceHost;
    [SerializeField] private TextMeshProUGUI caption;
    [SerializeField] private GameObject questionMark;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject shine;
    [SerializeField] private CodexItemDropPanel dropPanel;
    [SerializeField] private GameObject exclamationMark;
    
    [Header("Image")]
    [SerializeField] private Image pieceImage;
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private Vector3 rewardScale;
    [SerializeField] private Vector2 maxSize;
    
    private CodexItemState state;

    private CodexItemDef def;
    
    public void Init(CodexItemDef itemDef, bool forceHideArrow)
    {
        def = itemDef;
        state = itemDef.State;

        arrow.SetActive(def.ShowArrow && !forceHideArrow);
        
        Reset();

        Sprite sprite = null;
        string captionText = GetCaption();

        switch (state)
        {
            case CodexItemState.FullLock:
                questionMark.SetActive(true);
                
                pieceImage.gameObject.SetActive(false);
                               
                break;
            
            case CodexItemState.PartLock:
                questionMark.SetActive(true);
                sprite = GetPieecSprite();
                
                pieceImage.material = lockedMaterial;
                pieceImage.color = lockedColor;
                
                break;
            
            case CodexItemState.PendingReward:
                sprite = GetPieecSprite();
                // captionText = GetCaption();
                shine.SetActive(true);
                exclamationMark.SetActive(true);
                
                dropPanel.gameObject.SetActive(true);
                dropPanel.Init(itemDef);

                pieceImage.transform.localScale = rewardScale;
                
                break;
            
            case CodexItemState.Unlocked:
                sprite = GetPieecSprite();
                // captionText = GetCaption();
                
                dropPanel.gameObject.SetActive(true);
                dropPanel.Init(itemDef);
                break;
            
            case CodexItemState.Highlighted:
                sprite = GetPieecSprite();
                shine.SetActive(true);

                pieceImage.transform.localScale = rewardScale;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        pieceImage.sprite = sprite;
        caption.text = captionText;
        
        pieceImage.SetNativeSize();

        Vector2 size = pieceImage.rectTransform.sizeDelta;
        size.x = Mathf.Min(maxSize.x, size.x);
        size.y = Mathf.Min(maxSize.y, size.y);
        pieceImage.rectTransform.sizeDelta = size;
        
        // Debug.Log($"[CodexItem] => Init {itemDef.PieceTypeDef.Abbreviations[0]} as {state}");
    }

    private void Reset()
    {
        questionMark.SetActive(false);
        shine.SetActive(false);
        exclamationMark.SetActive(false);

        pieceImage.gameObject.SetActive(true);
        pieceImage.material = unlokedMaterial;
        pieceImage.color = unlockedColor;
        
        dropPanel.gameObject.SetActive(false);

        pieceImage.transform.localScale = defaultScale;
    }

    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(def.PieceTypeDef.Id));
    }
    
    private string GetCaption()
    {
        return def?.PieceDef?.Name;
    }
}