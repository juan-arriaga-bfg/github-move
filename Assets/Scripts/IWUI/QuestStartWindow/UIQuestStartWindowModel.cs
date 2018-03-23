using System;
using System.Collections.Generic;

public class UIQuestStartWindowModel : IWWindowModel
{
    public Obstacle Obstacle { get; set; }
    
    public string Title
    {
        get { return "Sherwood Quest"; }
    }
    
    public string SubTitle
    {
        get { return "Choose party for the quest:"; }
    }
    
    public string Message
    {
        get { return Obstacle.Def.Message; }
    }
    
    public string ButtonText
    {
        get { return "Start"; }
    }
    
    public string TimeText
    {
        get
        {
            var conditionsDelay = Obstacle.GetOpenConditions<ObstacleConditionDelay>();
            var conditionsHero = Obstacle.GetOpenConditions<ObstacleConditionHero>();
            
            var bonus = 0;
            
            for (var i = 0; i < conditionsHero.Count; i++)
            {
                var hero = GameDataService.Current.HeroesManager.GetHero(conditionsHero[i].HeroAbility.Ability);
                
                if(hero.InAdventure != Obstacle.GetUid()) continue;

                bonus += hero.CurrentAbilityValue;
            }
            
            return string.Format("Total quest time: {0} min", conditionsDelay.Count == 0 ? 0 : new TimeSpan(0, 0, conditionsDelay[0].Delay).TotalMinutes - bonus);
        }
    }

    public string GetChestSkin()
    {
        var chest = GameDataService.Current.ChestsManager.GetChest(Obstacle.Def.Reward);
        return chest.GetSkin();
    }

    public List<ObstacleConditionHero> GetConditionHeroes()
    {
        return Obstacle.GetOpenConditions<ObstacleConditionHero>();
    }
}
