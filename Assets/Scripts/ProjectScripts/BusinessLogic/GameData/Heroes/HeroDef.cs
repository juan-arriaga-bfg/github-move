using System;
using System.Collections.Generic;

public enum AbilityType
{
    Strength,
    Dexterity,
    Power
}

public enum HeroType
{
    Robin = 0,
    John,
    Greenarrow
}

public class HeroAbility
{
    public int Value { get; set; }
    public AbilityType Ability { get; set; }
}

public class HeroDef
{
    public string Uid { get; set; }
    public int Level { get; set; }
    public int Cooldown { get; set; }
    public List<HeroAbility> Abilities { get; set; }
    public List<CurrencyPair> Collection { get; set; }
    public List<CurrencyPair> Prices { get; set; }

    public int SaveIndex()
    {
        var type = (HeroType)Enum.Parse(typeof(HeroType), Uid);
        
        return (int)type;
    }
}