using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrdersSelectItem : UISimpleScrollItem
{
    [SerializeField] private List<UISimpleScrollItem> priceItems;
    
    [SerializeField] protected Image iconOrder;
    [SerializeField] protected NSText timer;
    
    [SerializeField] protected GameObject btnComplete;
    [SerializeField] protected GameObject btnBuy;

    public void Init(OrderDef order)
    {
        
    }
    
}