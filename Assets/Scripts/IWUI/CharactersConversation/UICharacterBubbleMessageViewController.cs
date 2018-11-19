using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterBubbleMessageViewController : UICharacterBubbleView, ITeleTypedText
{
    [SerializeField] protected Transform back;
    [SerializeField] protected Transform bubbleHost;
    [SerializeField] protected Image headerBack;
    [SerializeField] protected NSText header;
    [SerializeField] protected NSText message;
    [SerializeField] protected CanvasGroup canvasGroup;

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
    
    public override void Show(ConversationActionBubbleEntity data, Action onComplete)
    {
        SetCharRelatedData(data);

        SetSide(data.Side);    

        message.Text = data.Message;

        if (data.AllowTeleType)
        {
            PlayTeleTypeEffect();
        }

        canvasGroup.alpha = 0;
        
        ShowAnimation(onComplete);
    }

    protected virtual void ShowAnimation(Action onComplete)
    {
        canvasGroup.DOFade(1, 0.5f);

        DOTween.Sequence()
               .AppendInterval(0.1f)
               .AppendCallback(() => { onComplete?.Invoke(); });
    }

    private void SetSide(CharacterSide side)
    {
        Vector3 scale = side == CharacterSide.Left ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        back.localScale = scale;
        message.transform.localScale = scale;
        header.transform.localScale = scale;
    }

    private void SetCharRelatedData(ConversationActionBubbleEntity data)
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

        HideAnimation(onComplete);
    }

    protected virtual void HideAnimation(Action onComplete)
    {
        var pool = UIService.Get.PoolContainer;

        float fadeTime = 1f;
        
        canvasGroup.DOFade(0, 1);
        
        DOTween.Sequence()
               .InsertCallback(0.5f, () =>
                {
                    StopTeleTypeEffect();
                    onComplete?.Invoke();
                })               
                .InsertCallback(fadeTime, () =>
                {
                    pool.Return(gameObject);
                });
    }
}