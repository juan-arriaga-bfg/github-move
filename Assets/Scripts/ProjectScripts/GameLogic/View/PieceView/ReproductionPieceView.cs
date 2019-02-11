public class ReproductionPieceView : CooldownPieceView
{
    protected override string coolDownParticle => R.ProductionProcessParticle;
    protected override string readyParticleName => R.ProductionReadyParticle;

    public override void ToggleEffectsByState(bool isProcessing)
    {
        var isTutorialEnd = Context.Context.TutorialLogic.CheckLockPR();
        if (isTutorialEnd == false)
        {
            ClearParticle(ref processParticle);
            ClearParticle(ref readyParticle);
            return;
        }
        
        base.ToggleEffectsByState(isProcessing);
    }

    public void OnTutorialEnd()
    {
        base.ToggleEffectsByState(false);
    }
}