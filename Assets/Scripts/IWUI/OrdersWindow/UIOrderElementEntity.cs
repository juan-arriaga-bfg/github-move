using System;

public class UIOrderElementEntity : UISimpleScrollElementEntity
{
    public Order Data;

    public Action<Order, OrderState, OrderState> OnOrderStageChangeFromTo;
}