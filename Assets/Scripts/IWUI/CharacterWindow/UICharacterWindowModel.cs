using System;
using UnityEngine;

// TODO: как придет время и будет заведены типы карточек переписать на них
public enum CharacterWindowCardTupe
{
    Build,
    Rare,
    Resources
}

public class UICharacterWindowModel : IWWindowModel
{
    public Hero Hero { get; set; }
    
    public bool IsDone
    {
        get { return Hero.CurrentProgress >= Hero.TotalProgress; }
    }
    
    public string Title
    {
        get { return string.Format("{0} Character:", Hero.Def.Uid); }
    }
    
    public string Message
    {
        get { return "Next level:"; }
    }

    public string AbilityValue
    {
        get
        {
            return string.Format("{0} <color=#00FF00>+{1}</color>", Hero.CurrentAbilityValue, Hero.NextAbilityValue - Hero.CurrentAbilityValue);
        }
    }

    public string ButtonText
    {
        get { return string.Format("{0}", Hero.Price); }
    }

    public string ProgressText
    {
        get { return string.Format("{0}/{1}", Hero.CurrentProgress, Hero.TotalProgress); }
    }

    public float ProgressLenght
    {
        get { return Mathf.Clamp(320 * Hero.CurrentProgress / (float) Hero.TotalProgress, 0, 320); }
    }
    
    public string LevelText
    {
        get { return string.Format("<size=45>{0}</size> <size=35>Lvl</size>", Hero.Level + 1); }
    }
    
    public Sprite IconSprite
    {
        get { return IconService.Current.GetSpriteById("face_" + Hero.Def.Uid); }
    }
    
    public Sprite SkillSprite
    {
        get { return IconService.Current.GetSpriteById(Hero.CurrentAbility.ToString()); }
    }
    
    public string CardTupeText
    {
        get
        {
            var color = "FFFFFF";
            var cardTupe = CharacterWindowCardTupe.Rare;

            switch (cardTupe)
            {
                case CharacterWindowCardTupe.Build:
                    color = "347E13";
                    break;
                case CharacterWindowCardTupe.Rare:
                    color = "3D7AA4";
                    break;
                case CharacterWindowCardTupe.Resources:
                    color = "D75100";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return string.Format("<color=#{0}>{1} card</color>", color, cardTupe);
        }
    }
}