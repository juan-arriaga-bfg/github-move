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
    [SerializeField] private Image pieceImage;
    [SerializeField] private TextMeshProUGUI caption;
    [SerializeField] private GameObject questionMark;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject shine;
    [SerializeField] private CodexItemDropPanel dropPanel;
    [SerializeField] private GameObject exclamationMark;

    private CodexItemState state;

    private CodexItemDef def;
    
    public void Init(CodexItemDef itemDef)
    {
        def = itemDef;
        state = itemDef.State;

        arrow.SetActive(def.ShowArrow);
        
        Reset();

        Sprite sprite = null;
        string captionText = string.Empty;

        switch (state)
        {
            case CodexItemState.FullLock:
                questionMark.SetActive(true);
                pieceImage.gameObject.SetActive(false);
                break;
            
            case CodexItemState.PartLock:
                questionMark.SetActive(true);
                sprite = GetPieecSprite();
                captionText = GetCaption();
                pieceImage.material = lockedMaterial;
                pieceImage.color = lockedColor;
                break;
            
            case CodexItemState.PendingReward:
                sprite = GetPieecSprite();
                captionText = GetCaption();
                EnableShine();
                exclamationMark.SetActive(true);
                break;
            
            case CodexItemState.Unlocked:
                sprite = GetPieecSprite();
                captionText = GetCaption();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        pieceImage.sprite = sprite;
        caption.text = captionText;
        
        dropPanel.Init(itemDef);
        
        Debug.Log($"[CodexItem] => Init {itemDef.PieceTypeDef.Abbreviations[0]} as {state}");
    }

    private void Reset()
    {
        questionMark.SetActive(false);
        shine.SetActive(false);
        exclamationMark.SetActive(false);

        pieceImage.gameObject.SetActive(true);
        pieceImage.material = unlokedMaterial;
        pieceImage.color = unlockedColor;
    }

    private void EnableShine()
    {
        shine.SetActive(true);

        DOTween.Kill(shine);
        
        shine.transform.DORotate(new Vector3(0, 0, 360), 5.5f, RotateMode.LocalAxisAdd)
             .SetId(shine)
             .SetLoops(-1);
    }

    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(def.PieceTypeDef.Id));
    }
    
    private string GetCaption()
    {
        return def.PieceDef.Name;
    }
}