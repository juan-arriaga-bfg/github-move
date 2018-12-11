using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexItem : IWUIWindowViewController
{
    [Header("Materials")] 
    [SerializeField] private Material unlokedMaterial;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color lockedColor;
    
    [Header("Image params")]
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private Vector3 rewardScale;
    [SerializeField] private Vector2 maxSize;

    [IWUIBinding("#Caption")] private TextMeshProUGUI caption;
    [IWUIBinding("#(?)")]     private GameObject questionMark;
    [IWUIBinding("#Arrow")]   private GameObject arrow;
    [IWUIBinding("#Shine")]   private GameObject shine;
    [IWUIBinding("#Basket")]  private GameObject basket;
    [IWUIBinding("#Hand")]    private GameObject hand;
    [IWUIBinding("#Piece")]   private Image pieceImage;
    
    private CodexItemState state;

    private CodexItemDef def;
    
    public void UpdateUI(CodexItemDef itemDef, bool forceHideArrow)
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

                pieceImage.transform.localScale = rewardScale;
                
                break;
            
            case CodexItemState.Unlocked:
                sprite = GetPieecSprite();
                // captionText = GetCaption();

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

        pieceImage.gameObject.SetActive(true);
        pieceImage.material = unlokedMaterial;
        pieceImage.color = unlockedColor;

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