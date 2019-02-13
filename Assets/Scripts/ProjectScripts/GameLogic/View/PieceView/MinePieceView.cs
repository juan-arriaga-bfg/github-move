using DG.Tweening;

public class MinePieceView : CooldownPieceView
{
    protected override string coolDownParticle => R.MineProcessParticle;
    protected override string coolDownLeaveParticle => R.MineEndParticle;
    protected override string coolDownEnterParticle => R.MineEndParticle;

    protected override void OnComplete()
    {
        ParticleView.Show(coolDownLeaveParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));
        ToggleEffectsByState(false);

        var sequence = DOTween.Sequence();
        sequence.InsertCallback(0.2f, UpdateSate);
        
    }
}