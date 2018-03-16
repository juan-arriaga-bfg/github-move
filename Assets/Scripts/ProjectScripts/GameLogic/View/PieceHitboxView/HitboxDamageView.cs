using UnityEngine;
using DG.Tweening;

public class HitboxDamageView : BoardElementView 
{
    [SerializeField] private NSText damageLabel;

    public virtual void ApplyDamage(int damage, Vector3 targetPos)
    {
        damageLabel.Text = string.Format("-{0}", damage);

        float duration = 1f;
        DOTween.Kill(animationUid);
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Append(CachedTransform.DOMove(targetPos, duration));
        
        FadeAlpha(0f, 1f, (_) => { });

        DestroyOnBoard(duration);
    }
}
