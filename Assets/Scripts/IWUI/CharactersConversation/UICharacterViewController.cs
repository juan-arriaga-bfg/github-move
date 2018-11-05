using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICharacterViewController : IWBaseMonoBehaviour   
{
    [SerializeField] private Image characterImage;

    public string CharacterId;
    
    private bool active = true;

    private CharacterSide side;
    public CharacterSide Side
    {
        get { return side; }

        set
        {
            if (value == CharacterSide.Right)
            {
                var scale = characterImage.transform.localScale;
                if (scale.x > 0)
                {
                    scale.x *= -1;
                    characterImage.transform.localScale = scale;
                }
            }

            side = value;
        }
    }

    public void OnEnable()
    {
        if (characterImage == null)
        {
            return;
        }

        // Sync sprite and rect transform pivots
        RectTransform rectTransform = characterImage.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size *= characterImage.pixelsPerUnit;
        Vector2 pixelPivot = characterImage.sprite.pivot;
        Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
        rectTransform.pivot = percentPivot;
    }
    
    public void ToggleActive(bool active, bool animated, Action onComplete = null)
    {
        if (this.active == active)
        {
            onComplete?.Invoke();
            return;
        }

        this.active = active;

        Color color = active ? Color.white : Color.gray;
        Vector3 scale = active ? Vector3.one : Vector3.one * 0.9f;
        if (animated)
        {
            
            const float TIME = 0.7f;

            characterImage.DOColor(color, TIME)
                          .SetEase(Ease.OutSine)
                          .OnComplete(() =>
                           {
                               onComplete?.Invoke();
                           });

            transform.DOScale(scale, TIME)
                     .SetEase(Ease.OutSine);
        }
        else
        {
            characterImage.color = color;
            transform.localScale = scale;
            onComplete?.Invoke();
        }
    }
    
    public void ToForeground(bool animated, Action onComplete = null)
    {
        ToggleActive(true, animated, onComplete);
    }

    public void ToBackground(bool animated, Action onComplete = null)
    {
        ToggleActive(false, animated, onComplete);
    }
}