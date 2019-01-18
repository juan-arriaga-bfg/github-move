using System;
using DG.Tweening;
using UnityEngine;

public class DummyIapValidator : IIapValidator
{
    public float Delay { get; set; } = 1f;
    public IapValidationResult Result { get; set; } = IapValidationResult.Genuine;
    
    public void Validate(string productId, string receipt, Action<IapValidationResult> onComplete)
    {
        DOTween.Sequence()
               .AppendInterval(Delay)
               .AppendCallback(() =>
                {
                    onComplete.Invoke(Result);
                });
    }
}