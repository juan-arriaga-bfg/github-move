using System.Collections.Generic;
using System.Text;

public class UIDailyRewardElementEntity : UISimpleScrollElementEntity
{
    public DailyRewardState State;
    
    public List<CurrencyPair> Rewards;
    
    public string RewardsText
    {
        get
        {
            if (Rewards == null) return string.Empty;
            
            var str = new StringBuilder();
            
            foreach (var pair in Rewards)
            {
                if (PieceType.Parse(pair.Currency) != PieceType.None.Id)
                {
                    str.Append($"x{pair.Amount} ");
                    continue;
                }
                
                str.Append($"{pair.ToStringIcon()} ");
            }
            
            return str.ToString().TrimEnd();
        }
    }
}
