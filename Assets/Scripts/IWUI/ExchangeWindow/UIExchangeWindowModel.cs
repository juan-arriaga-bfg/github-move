using System;
using System.Collections.Generic;

public class UIExchangeWindowModel : IWWindowModel
{
    public string Title;
    public string Message;
    public string Button => string.Format(LocalizationService.Instance.Manager.GetTextByUid("common.button.buyAll", "Buy all {0}"), Price.ToStringIcon(false));

    public Action OnClick;
    
    public CurrencyPair Price;

    public List<CurrencyPair> Products;
}
