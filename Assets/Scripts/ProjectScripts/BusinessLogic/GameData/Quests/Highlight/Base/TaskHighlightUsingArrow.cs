using DG.Tweening;

public abstract class TaskHighlightUsingArrow : ITaskHighlight
{
     public const float DELAY_BEFORE_SHOW_ARROW = 0.5f;
     
     public virtual bool Highlight(TaskEntity task)
     {
         var hintCooldown = BoardService.Current.FirstBoard.HintCooldown;

         if (ShowArrow(task, DELAY_BEFORE_SHOW_ARROW))
         {
             hintCooldown.Pause(this);
 
             DOTween.Sequence()
                    .AppendInterval(HintArrowView.DURATION + DELAY_BEFORE_SHOW_ARROW)
                    .AppendCallback(() =>
                     {
                         hintCooldown.Resume(this);
                     });

             return true;
         }

         return false;
     }

     protected abstract bool ShowArrow(TaskEntity task, float delay);
 }