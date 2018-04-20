using System;
using System.Collections.Generic;
using UnityEngine;

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
            
            return string.Format("Total quest time: {0} min", conditionsDelay.Count == 0 ? 0 : new TimeSpan(0, 0, conditionsDelay[0].Delay).TotalMinutes );
        }
    }

    public string GetChestSkin()
    {
        return "";
    }

    public List<ObstacleConditionHero> GetConditionHeroes()
    {
        return Obstacle.GetOpenConditions<ObstacleConditionHero>();
    }
}
