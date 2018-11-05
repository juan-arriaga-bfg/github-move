using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterBubbleMessageViewController : UICharacterBubbleView
{
    [SerializeField] private Transform back;
    [SerializeField] private Image headerBack;
    [SerializeField] private NSText header;
    [SerializeField] private NSText message;
    [SerializeField] private CanvasGroup canvasGroup;

#region Teletype
    
    NSTeleTypeEffect cachedTeleTypeEffect;
    
    public virtual NSTeleTypeEffect GetTeleTypeEffect()
    {       
        if (cachedTeleTypeEffect == null)
        {
            cachedTeleTypeEffect = message.GetComponent<NSTeleTypeEffect>();
        }
        
        return cachedTeleTypeEffect;
    }
    
    public virtual void PlayTeleTypeEffect()
    {
        var teleTypeEffect = GetTeleTypeEffect();
        if (teleTypeEffect == null) return;
        
        teleTypeEffect.Play();
    }
    
    public virtual void StopTeleTypeEffect()
    {
        var teleTypeEffect = GetTeleTypeEffect();
        if (teleTypeEffect == null) return;
        
        teleTypeEffect.Stop();
    } 
    
    public virtual bool IsPlayingTeleTypeEffect()
    {
        var teleTypeEffect = GetTeleTypeEffect();
        if (teleTypeEffect == null) return false;

        return teleTypeEffect.IsPlaying;
    }

#endregion
    
    public override void Show(UICharacterBubbleDef def, Action onComplete)
    {
        UiCharacterBubbleDefMessage data = def as UiCharacterBubbleDefMessage;

        SetCharRelatedData(data);

        SetSide(data.Side);
        
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1);
        
        message.Text = data.Message;
        PlayTeleTypeEffect();

        DOTween.Sequence()
               .AppendInterval(1)
               .AppendCallback(() =>
                {
                    onComplete?.Invoke();
                });
    }

    private void SetSide(CharacterSide side)
    {
        Vector3 scale = side == CharacterSide.Left ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        back.localScale = scale;
        message.transform.localScale = scale;
        header.transform.localScale = scale;
    }

    private void SetCharRelatedData(UiCharacterBubbleDefMessage data)
    {
        UICharacterDef charDef = UiCharacterData.GetDef(data.CharacterId);
        headerBack.color = charDef.Color;
        header.Text = charDef.Name;
    }

    public override void Hide(bool animated, Action onComplete)
    {
        if (!animated)
        {
            StopTeleTypeEffect();
            canvasGroup.alpha = 0;
            onComplete?.Invoke();
            return;
        }
        
        canvasGroup.DOFade(0, 1);
        DOTween.Sequence()
               .AppendInterval(1)
               .AppendCallback(() =>
                {
                    StopTeleTypeEffect();
                    onComplete?.Invoke();
                });
    }
}