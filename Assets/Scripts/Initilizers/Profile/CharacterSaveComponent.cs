using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CharacterSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid { get { return ComponentGuid; } }
    
    private List<HeroSaveItem> heroes;
    private List<EnemySaveItem> enemies;

    private List<int> deadEnemies;

    [JsonProperty]
    public List<HeroSaveItem> Heroes
    {
        get { return heroes; }
        set { heroes = value; }
    }
    
    [JsonProperty]
    public List<EnemySaveItem> Enemies
    {
        get { return enemies; }
        set { enemies = value; }
    }
    
    [JsonProperty]
    public List<int> DeadEnemies
    {
        get { return deadEnemies; }
        set { deadEnemies = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if(GameDataService.Current == null || BoardService.Current == null) return;
        
        var board = BoardService.Current.GetBoardById(0);
        var logic = board.EnemiesLogic;
        var save = GameDataService.Current.HeroesManager.Heroes;
        
        heroes = new List<HeroSaveItem>();
        enemies = new List<EnemySaveItem>();
        
        foreach (var hero in save)
        {
            if(hero.IsUnlock == false || hero.IsCollect == false || hero.IsSleep == false) continue;
            
            heroes.Add(new HeroSaveItem{Id = hero.Def.SaveIndex(), StartTime = hero.SleepTime.Value});
        }

        deadEnemies = GameDataService.Current.EnemiesManager.DeadEnemies;
        
        if(logic.Enemy == null) return;
        
        enemies.Add(new EnemySaveItem{Damage = logic.Enemy.Damage, Reward = logic.Enemy.ActiveReward, Step = logic.Enemy.Step});
    }
}