using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICharacterViewController : IWBaseMonoBehaviour
{
    [Serializable]
    public class CharacterEmotionDef
    {
        public CharacterEmotion Emotion;
        public Image Image;
    }

    [Header("Emotions")]
    [SerializeField] private List<CharacterEmotionDef> emotions;

    [Header("Other settings")]
    [SerializeField] private CanvasGroup canvasGroup;

    public CanvasGroup GetCanvasGroup()
    {
        return canvasGroup;
    }
    
    private CharacterEmotion currentEmotion;
    private Image currentCharImage;

    public const float FADE_TIME = 0.3f;

    public string CharacterId { get; set; }

    private bool active = true;

    private CharacterSide side;
    public CharacterSide Side
    {
        get { return side; }

        set
        {
            // todo: refactor!
            if (value == CharacterSide.Right)
            {
                foreach (var item in emotions)
                {
                    var scale = item.Image.transform.localScale;
                    if (scale.x > 0)
                    {
                        scale.x *= -1;
                        item.Image.transform.localScale = scale;
                    }
                }
            }
            else if (value == CharacterSide.Left)
            {
                foreach (var item in emotions)
                {
                    var scale = item.Image.transform.localScale;
                    if (scale.x < 0)
                    {
                        scale.x *= -1;
                        item.Image.transform.localScale = scale;
                    }
                }  
            }

            side = value;
        }
    }

    public CharacterEmotion Emotion
    {
        get { return currentEmotion; }
        set
        {
            Debug.Log($"[UICharacterViewController] => SetEmotion({value})");
        
            if (currentEmotion == value)
            {
                return;
            }

            bool found = false;
            foreach (var item in emotions)
            {
                if (item.Emotion == value)
                {
                    item.Image.gameObject.SetActive(true);
                    currentEmotion = value;
                    currentCharImage = item.Image;
                    found = true;
                }
                else
                {
                    item.Image.gameObject.SetActive(false); 
                }
            }

            if (!found)
            {
                var item = emotions[0];
                item.Image.gameObject.SetActive(true);
                currentEmotion = CharacterEmotion.Normal;
                currentCharImage = item.Image;
                Debug.LogError($"[UICharacterViewController] => SetEmotion({value}: Not defined!");
            }
        
            SyncPivotAndSize();
        }
    }

    public void OnEnable()
    {
        if (currentEmotion == CharacterEmotion.Unknown)
        {
            Emotion = CharacterEmotion.Normal;
            return;
        }
        
        if (currentCharImage == null)
        {
            return;
        }

        SyncPivotAndSize();
    }

    private void SyncPivotAndSize()
    {
        currentCharImage.SetNativeSize();
        
        // Sync sprite and rect transform pivots
        RectTransform rectTransform = currentCharImage.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size *= currentCharImage.pixelsPerUnit;
        Vector2 pixelPivot = currentCharImage.sprite.pivot;
        Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
        rectTransform.pivot = percentPivot;
    }

    public void ToggleActive(bool active, CharacterEmotion emotion, bool animated, Action onComplete = null)
    {
        Debug.Log($"[UICharacterViewController] => ToggleActive(active: {active}, animated: {animated})");

        bool emotionChanged = false;
        if (Emotion != emotion)
        {
            Emotion = emotion;
            emotionChanged = true;
        }
        
        if (this.active == active && !emotionChanged)
        {
            onComplete?.Invoke();
            return;
        }

        this.active = active;

        Color color = active ? Color.white : Color.gray;
        Vector3 scale = active ? Vector3.one : Vector3.one * 0.9f;
        if (animated)
        {
            currentCharImage.DOColor(color, FADE_TIME)
                          .SetEase(Ease.OutSine)
                          .OnComplete(() =>
                           {
                               onComplete?.Invoke();
                           });

            transform.DOScale(scale, FADE_TIME)
                     .SetEase(Ease.OutSine);
        }
        else
        {
            currentCharImage.color = color;
            transform.localScale = scale;
            onComplete?.Invoke();
        }
    }
    
    public void ToForeground(bool animated, CharacterEmotion emotion, Action onComplete = null)
    {
        ToggleActive(true, emotion, animated, onComplete);
    }

    public void ToBackground(bool animated, CharacterEmotion emotion, Action onComplete = null)
    {
        ToggleActive(false, emotion, animated, onComplete);
    }
}