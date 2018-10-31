using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterViewController : IWBaseMonoBehaviour   
{
    [SerializeField] private Image characterImage;

    private bool active;
    
    public void ToggleActive(bool active, bool animated, Action onComplete = null)
    {
        if (this.active == active)
        {
            onComplete?.Invoke();
            return;
        }

        this.active = active;

        Color color = active ? Color.white : Color.gray;
        
        if (animated)
        {
            const float TIME = 0.7f;
            characterImage.DOColor(color, TIME)
                           .OnComplete(() =>
                            {
                                onComplete?.Invoke();
                            });
        }
        else
        {
            characterImage.color = color;
            onComplete?.Invoke();
        }
    }
    
    public void ToForeground(bool animated, Action onComplete)
    {
        ToggleActive(true, animated, onComplete);
    }

    public void ToBackground(bool animated, Action onComplete)
    {
        ToggleActive(false, animated, onComplete);
    }
}