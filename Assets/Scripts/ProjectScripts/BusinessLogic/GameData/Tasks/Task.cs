using System.Collections.Generic;
using System.Linq;

public class Task
{
    public string Character { get; set; }
    public CurrencyPair Result { get; set; }
    public List<CurrencyPair> Prices { get; set; }
    public Dictionary<int, int> Rewards { get; set; }
    
    public CurrencyPair GetHardPrice()
    {
        List<CurrencyPair> diff;

        if (CurrencyHellper.IsCanPurchase(Prices, out diff)) return null;
        
        var amount = diff.Sum(pair => pair.Amount);
        
        return new CurrencyPair{Currency = Currency.Crystals.Name, Amount = amount};
    }

    public bool IsComplete
    {
        get { return CurrencyHellper.IsCanPurchase(Prices); }
    }
    
    public bool Exchange()
    {
        var isSuccess = false;
        List<CurrencyPair> diff;
        
        if (CurrencyHellper.IsCanPurchase(Prices, out diff))
        {
            CurrencyHellper.Purchase(Currency.Task.Name, 1, Prices, success =>
            {
                isSuccess = success;
            });
            
            return isSuccess;
        }

        var amount = diff.Sum(pair => pair.Amount);
        
        CurrencyHellper.Purchase(Currency.Task.Name, 1, new CurrencyPair{Currency = Currency.Crystals.Name, Amount = amount}, success =>
        {
            isSuccess = success;
            if(success == false) return;

            foreach (var pair in diff)
            {
                CurrencyHellper.Purchase(pair);
            }
            
            CurrencyHellper.Purchase(Currency.Task.Name, 1, Prices);
        });
        
        return isSuccess;
    }

    public void Ejection()
    {
        BoardService.Current.GetBoardById(0).ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = GameDataService.Current.PiecesManager.MatketPosition,
            Pieces = Rewards
        });
    }
}