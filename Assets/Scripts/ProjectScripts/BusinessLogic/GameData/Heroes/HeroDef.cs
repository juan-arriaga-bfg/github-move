using System.Collections.Generic;

public enum AbilityType
{
    Strength,
    Dexterity
}

public class HeroAbility
{
    public int Value { get; set; }
    public AbilityType Ability { get; set; }
}

public class HeroLevelDef
{
    public List<HeroAbility> Abilities{ get; set; }
    public List<CurrencyPair> Prices { get; set; }
}

public class HeroDef
{
    public string Uid { get; set; }
    public List<HeroLevelDef> Levels { get; set; }
}