using DG.Tweening;

public abstract class TaskHighlightUsingArrow : ITaskHighlight
{
     public const float DELAY_BEFORE_SHOW_ARROW = 0.5f;
     
     public virtual void Highlight(TaskEntity task)
     {
         var hintCooldown = BoardService.Current.FirstBoard.HintCooldown;
 
          hintCooldown.Pause(this);
 
          DOTween.Sequence()
                 .AppendInterval(HintArrowView.DURATION + DELAY_BEFORE_SHOW_ARROW)
                 .AppendCallback(() =>
                  {
                      hintCooldown.Resume(this);
                  });
         
         DOTween.Sequence()
                .AppendInterval(DELAY_BEFORE_SHOW_ARROW)
                .AppendCallback(() =>
                 {
                     ShowArrow(task);
                 });
     }

     protected abstract void ShowArrow(TaskEntity task);
 }