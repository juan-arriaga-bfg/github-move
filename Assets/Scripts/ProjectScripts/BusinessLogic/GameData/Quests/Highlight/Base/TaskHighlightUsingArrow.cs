using DG.Tweening;

public abstract class TaskHighlightUsingArrow : ITaskHighlight
{
    public virtual void Highlight(TaskEntity task)
    {
        var hintCooldown = BoardService.Current.FirstBoard.HintCooldown;

         hintCooldown.IsPaused = true;

         DOTween.Sequence()
                .AppendInterval(HintArrowView.DURATION)
                .AppendCallback(() =>
                 {
                     hintCooldown.IsPaused = false;
                 });
                 
         ShowArrow(task);
    }

    protected virtual void ShowArrow(TaskEntity task)
    {
        throw new System.NotImplementedException("Override me!");
    }
}