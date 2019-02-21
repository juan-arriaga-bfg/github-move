public class ReproductionPieceView : CooldownPieceView
{
    protected override string coolDownParticle => R.ProductionProcessParticle;
    protected override string readyParticleName => R.ProductionReadyParticle;
}