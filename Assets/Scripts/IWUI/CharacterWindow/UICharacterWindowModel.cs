using System;

public enum CharacterWindowTupe
{
    Robin,
    John
}

// TODO: как придет время и будет заведены типы карточек переписать на них
public enum CharacterWindowCardTupe
{
    Build,
    Rare,
    Resources
}

public class UICharacterWindowModel : IWWindowModel
{
    public CharacterWindowTupe WindowTupe { get; set; }

    public int CurrentProgress
    {
        get { return ProfileService.Current.GetStorageItem(Currency.RobinCards.Name).Amount; }
    }

    public int TotalProgress
    {
        get { return GameDataService.Current.GetHero("Robin").Prices[GameDataService.Current.HeroLevel].Amount; }
    }

    public bool IsDone
    {
        get { return CurrentProgress >= TotalProgress; }
    }
    
    public string Title
    {
        get { return string.Format("{0} Caracter:", WindowTupe); }
    }
    
    public string Message
    {
        get { return "Next Level:"; }
    }

    public string ButtonText
    {
        get { return IsDone ? "Evolve" : "Ok"; }
    }

    public string ProgressText
    {
        get { return string.Format("{0}/{1}", CurrentProgress, TotalProgress); }
    }
    
    public string DamageText
    {
        get
        {
            var hero = GameDataService.Current.GetHero("Robin");
            var level = GameDataService.Current.HeroLevel;
            var nextDamage = level == hero.Damages.Count - 1 ? 0 : hero.Damages[level + 1];
            var heroDamage = hero.Damages[level];
            
            if(nextDamage == 0) return heroDamage.ToString();
            
            return string.Format("{0} <color=#00FF00>+{1}</color>", heroDamage, nextDamage);
        }
    }

    public string LevelText
    {
        get { return string.Format("Level {0}", GameDataService.Current.HeroLevel + 1); }
    }
    
    public string HeroName
    {
        get { return WindowTupe.ToString(); }
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