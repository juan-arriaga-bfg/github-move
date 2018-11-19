public class ConversationActionBubbleEntity : ConversationActionEntity
{
    public string BubbleView;
    public string Message;
    public CharacterSide Side;
    public CharacterEmotion Emotion = CharacterEmotion.Normal;
    public string CharacterId;
    public bool AllowTeleType = true;
}

public class ConversationActionBubbleQuestCompletedEntity : ConversationActionBubbleEntity
{
    public string QuestId;
}