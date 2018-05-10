using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CharacterSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    private List<HeroSaveItem> heroes;
    
    [JsonProperty]
    public List<HeroSaveItem> Heroes
    {
        get { return heroes; }
        set { heroes = value; }
    }

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if(GameDataService.Current == null) return;
        
        heroes = new List<HeroSaveItem>();
        
        var save = GameDataService.Current.HeroesManager.Heroes;

        foreach (var hero in save)
        {
            if(hero.IsUnlock == false || hero.IsCollect == false || hero.IsSleep == false) continue;
            
            heroes.Add(new HeroSaveItem{Id = hero.Def.SaveIndex(), StartTime = hero.SleepTime.Value});
        }
    }
}