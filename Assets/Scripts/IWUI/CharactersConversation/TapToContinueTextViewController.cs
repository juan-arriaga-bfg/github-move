using DG.Tweening;
using TMPro;
using UnityEngine;

public class TapToContinueTextViewController : IWBaseMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float fadeAmount = 0.4f;
    [SerializeField] private Ease FADE_EASE = Ease.OutSine;

    private bool visible;
    
    private void Start()
    {
        label.text = LocalizationService.Get("common.message.tapToContinue", "common.message.tapToContinue");
    }

    private void OnEnable()
    {
        Show(true);
    }

    private void OnDisable()
    {
        Hide(false);
    }

    public void Show(bool animated, float delay = 0)
    {
        if (visible)
        {
            return;
        }

        visible = true;
        
        label.color = new Color(1, 1, 1, 0);

        DOTween.Sequence()
               .SetId(this)
               .AppendInterval(delay)
               .AppendCallback(() =>
                {
                    DOTween.ToAlpha(() => label.color, x => label.color = x, 1, fadeTime)
                           .SetTarget(label)
                           .SetId(this)
                           .OnComplete(() =>
                            {
                                DOTween.Sequence()
                                       .SetId(this)
                                       .Append(DOTween.ToAlpha(() => label.color, x => label.color = x, fadeAmount, fadeTime)
                                                      .SetTarget(label)
                                                      .SetEase(FADE_EASE)
                                                      .SetId(this))
                                       .SetLoops(-1, LoopType.Yoyo);
                            });
                });
    }

    public void Hide(bool animated)
    {
        DOTween.Kill(this);
        
        if (!visible)
        {
            return;
        }

        visible = false;

        if (!animated)
        {
            label.color = new Color(1, 1, 1, 0);
            return;
        }

        DOTween.ToAlpha(() => label.color, x => label.color = x, 0, fadeTime)
               .SetTarget(label)
               .SetId(this);
    }
}
