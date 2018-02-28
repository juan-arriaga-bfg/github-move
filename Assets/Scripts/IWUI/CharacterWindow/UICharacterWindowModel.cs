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
    public CharacterWindowCardTupe CardTupe { get; set; }
    
    public int CurrentProgress { get; set; }
    public int TotalProgress { get; set; }
    
    public int HeroDamage { get; set; }
    public int TeamDamage { get; set; }
    
    public int HeroLevel { get; set; }
    
    public bool IsDone
    {
        get { return CurrentProgress == TotalProgress; }
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
        get { return string.Format("{0} <color=#00FF00>+{1}</color>", HeroDamage, TeamDamage); }
    }

    public string LevelText
    {
        get { return string.Format("Level {0}", HeroLevel); }
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

            switch (CardTupe)
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
            
            return string.Format("<color=#{0}>{1} card</color>", color, CardTupe);
        }
    }
}