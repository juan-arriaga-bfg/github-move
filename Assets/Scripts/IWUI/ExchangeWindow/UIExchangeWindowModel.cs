using System;
using System.Collections.Generic;

public class UIExchangeWindowModel : IWWindowModel
{
    public string Title;
    public string Message;
    public string Button => $"Buy all {Price.ToStringIcon(false)}";

    public Action OnClick;
    
    public CurrencyPair Price;

    public List<CurrencyPair> Products;
}
