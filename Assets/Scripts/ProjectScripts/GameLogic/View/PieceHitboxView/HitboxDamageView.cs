using UnityEngine;
using DG.Tweening;

public class HitboxDamageView : BoardElementView 
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private NSText damageLabel;

    private void Show(int damage)
    {
        damageLabel.Text = string.Format("<color=#EE4444>-{0}</color>", damage);

        float duration = 1f;
		
        DOTween.Kill(animationUid);
		
        var sequence = DOTween.Sequence().SetId(animationUid);
		
        sequence.Insert(0, CachedTransform.DOMove(CachedTransform.position + Vector3.up, duration));
        sequence.Insert(duration*0.5f, icon.DOFade(0f, duration));
        sequence.Insert(duration*0.5f, damageLabel.TextLabel.DOFade(0f, duration));
		
        DestroyOnBoard(duration);
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
		
        icon.color = Color.white;

        var tColor = damageLabel.TextLabel.color;
		
        damageLabel.TextLabel.color = new Color(tColor.r, tColor.g, tColor.b, 1);
    }
    
    public static void Show(BoardPosition position, int damage)
    {
        var board = BoardService.Current.GetBoardById(0);
        var view = board.RendererContext.CreateBoardElementAt<HitboxDamageView>(R.HitboxDamageView, position);
		
        view.CachedTransform.localPosition = view.CachedTransform.localPosition + Vector3.up;
        view.Show(damage);
    }
}
