using System;

public abstract class UICharacterBubbleView : IWBaseMonoBehaviour
{
    public abstract void Hide(bool animated, Action onComplete);
    public abstract void Show(UICharacterBubbleDef def, Action onComplete);
}