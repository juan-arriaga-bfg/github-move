using System.Collections.Generic;

public class ConversationActionAddCharactersEntity : ConversationActionEntity
{
    public List<string> CharacterIds;
    public List<string> Positions;

    public bool Automatic; // if TRUE all chars specified on scenario will be added
}