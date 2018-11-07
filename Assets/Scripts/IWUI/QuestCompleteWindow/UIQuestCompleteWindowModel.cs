using UnityEngine;
using System.Collections;

public class UIQuestCompleteWindowModel : IWWindowModel
{
    public string Message { get; private set; }
    public string CharacterId { get; private set; }
    public CharacterEmotion Emotion { get; private set; } = CharacterEmotion.Happy;
    
    private QuestEntity quest;
    public QuestEntity Quest
    {
        get { return quest; }
        set
        {
            quest = value;
            FillData(quest.Id);
        }
    }

    private void FillData(string questId)
    {
        switch (questId)
        {
            case "1" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "2" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "Great! That fog is really annoying.";
                break;
            
            case "3" :
                CharacterId = UiCharacterData.CharRapunzel;
                Emotion = CharacterEmotion.Normal;
                Message = "There are much better without those rusted trees.";
                break;
            
            case "4" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "5" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "6" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "7" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "8" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            case "9" :
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "What a beautiful tree!";
                break;
            
            default:
                CharacterId = UiCharacterData.CharRapunzel;
                Message = "Cool quest completed message here!";
                break;
        }
    }
}
