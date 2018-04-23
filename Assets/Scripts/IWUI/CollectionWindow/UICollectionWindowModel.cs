using System;
using System.Collections.Generic;

public class UICollectionWindowModel : IWWindowModel
{
    public Action OnOpen;
    
    private Hero hero;

    public string Element
    {
        set
        {
            hero = GameDataService.Current.HeroesManager.Heroes.Find(h => h.IsElementOfHero(value));
        }
    }
    
    public string Hero
    {
        get { return hero.Def.Uid; }
    }

    public bool IsCollectAll
    {
        get { return hero.IsCollectCollections; }
    }
    
    public List<string> GetCollection()
    {
        var collection = new List<string>();

        foreach (var pair in hero.Collection)
        {
            collection.Add(pair.Currency);
        }

        return collection;
    }
}