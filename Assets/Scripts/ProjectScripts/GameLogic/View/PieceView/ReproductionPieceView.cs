public class ReproductionPieceView : CooldownPieceView
{
    protected override string coolDownParticle => R.ProductionProcessParticle;
    protected override string readyParticleName => R.ProductionReadyParticle;

    public override void ToggleEffectsByState(bool isProcessing)
    {
        if (life == null)
            life = Piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
        if (life?.Locker.IsLocked == true)
        {
            ClearParticle(ref processParticle);
            ClearParticle(ref readyParticle);
            return;
        }
        
        base.ToggleEffectsByState(isProcessing);
    }
}