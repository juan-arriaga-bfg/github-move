using System;

public interface IIapValidator
{
    void Validate(string productId, string receipt, Action<IapValidationResult> onComplete);
}