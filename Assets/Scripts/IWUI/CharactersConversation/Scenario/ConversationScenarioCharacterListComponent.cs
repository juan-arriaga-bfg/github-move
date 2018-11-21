using System;
using System.Collections.Generic;

public class ConversationScenarioCharacterListComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public Dictionary<CharacterPosition, string> ConversationCharacters;

    public CharacterSide GetCharacterSide(string charId)
    {
        if (ConversationCharacters == null)
        {
            return CharacterSide.Unknown;
        }

        foreach (var item in ConversationCharacters)
        {
            if (item.Value != charId)
            {
                continue;
            }

            switch (item.Key)
            {
                case CharacterPosition.Unknown:
                    return CharacterSide.Unknown;
                
                case CharacterPosition.LeftOuter:
                case CharacterPosition.LeftInner:
                    return CharacterSide.Left;
                
                case CharacterPosition.RightInner:
                case CharacterPosition.RightOuter:
                    return CharacterSide.Right;
            }
        }
        
        return CharacterSide.Unknown; 
    }
}