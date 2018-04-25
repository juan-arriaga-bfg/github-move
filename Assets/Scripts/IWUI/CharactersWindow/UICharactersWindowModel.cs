using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharactersWindowModel : IWWindowModel 
{
    public string WakeUpText
    {
        get { return string.Format("Wake Up\nBand {0}<sprite name=Crystals>", 15); }
    }

    public List<Hero> Heroes
    {
        get
        {
            var heroes = GameDataService.Current.HeroesManager.Heroes.FindAll(hero => hero.IsCollect);
            
            return heroes;
        }
    }
}
