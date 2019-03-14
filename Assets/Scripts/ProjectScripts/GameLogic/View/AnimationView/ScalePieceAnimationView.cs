using System.Web;
using DG.Tweening;
using UnityEngine;

public class ScalePieceAnimationView : AnimationView
{
    [SerializeField] private bool scaleOnlyView = false;
    [SerializeField] private bool startFromCurrentScale = false; 
    [SerializeField] private Vector3 from = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 to = new Vector3(1, 1, 1);
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float timeoutDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.Linear;
    
    public override void Play(PieceBoardElementView pieceView)
    {
        base.Play(pieceView);
        
        DOTween.Kill(pieceView.SelectedAnimationId);
        
        if(startFromCurrentScale == false)
            pieceView.transform.localScale = from;

        DOTween.Kill(pieceView);
        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);
        if (scaleOnlyView)
        {
            var viewObj = pieceView.transform.Find("View");
            sequence.Insert(0, viewObj.DOScale(to, duration)).SetEase(easeType);
        }
        else
        {
            sequence.Insert(0, pieceView.transform.DOScale(to, duration)).SetEase(easeType);    
        }

        sequence.InsertCallback(timeoutDuration, CompleteAnimation);

    }
}