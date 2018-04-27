using System.Collections.Generic;

public class UICharactersWindowModel : IWWindowModel 
{
    public string WakeUpText
    {
        get
        {
            var price = WakeUpPrice;

            return price.Amount == 0 ? "Wake Up\nBand" : string.Format("Wake Up\nBand {0}<sprite name={1}>", price.Amount, price.Currency);
        }
    }

    public CurrencyPair WakeUpPrice
    {
        get { return new CurrencyPair {Currency = "Crystals", Amount = Heroes.FindAll(hero => hero.IsSleep).Count}; }
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
