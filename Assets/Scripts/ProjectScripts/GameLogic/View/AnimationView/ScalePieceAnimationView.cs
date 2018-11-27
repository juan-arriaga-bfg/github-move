using System.Web;
using DG.Tweening;
using UnityEngine;

public class ScalePieceAnimationView : AnimationView
{
    [SerializeField] private bool startFromCurrentScale = false; 
    [SerializeField] private Vector3 from = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 to = new Vector3(1, 1, 1);
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float timeoutDuration = 0.5f;

    protected PieceBoardElementView pieceView = null;
    
    public override void Play(PieceBoardElementView pieceView)
    {
        base.Play(pieceView);
        if(startFromCurrentScale == false)
            pieceView.transform.localScale = from;

        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);
        sequence.Insert(0, pieceView.transform.DOScale(to, duration));
        sequence.InsertCallback(timeoutDuration, () => OnComplete?.Invoke());

        this.pieceView = pieceView;
    }

    public override void Stop()
    {
        if (this.pieceView != null)
            DOTween.Kill(animationUid);
    }
}