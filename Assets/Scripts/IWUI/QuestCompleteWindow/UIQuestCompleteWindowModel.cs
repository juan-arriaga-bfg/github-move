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
                CharacterId = UiCharacterData.CharSleepingBeauty;
                Message = "What a beautiful tree!";
                break;
            
            case "2" :
                CharacterId = UiCharacterData.CharSleepingBeauty;
                Message = "Great! That fog is really annoying.";
                break;
            
            case "3" :
                CharacterId = UiCharacterData.CharSleepingBeauty;
                Emotion = CharacterEmotion.Normal;
                Message = "There are much better without those rusted trees.";
                break;
            
            case "4" :
                CharacterId = UiCharacterData.CharGnomeWorker;
                Message = "We're chest hunters from now!";
                break;
            
            case "5" :
                CharacterId = UiCharacterData.CharSleepingBeauty;
                Message = "Well done! Let's continue work and build up a house.";
                break;

            default:
                CharacterId = UiCharacterData.CharSleepingBeauty;
                Message = "Cool quest completed message here!";
                break;
        }
    }
}
