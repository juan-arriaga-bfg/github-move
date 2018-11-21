using System;

public abstract class UICharacterBubbleView : IWBaseMonoBehaviour
{
    public abstract void Hide(bool animated, Action onComplete);
    public abstract void Show(ConversationActionBubbleEntity def, Action onComplete);
}