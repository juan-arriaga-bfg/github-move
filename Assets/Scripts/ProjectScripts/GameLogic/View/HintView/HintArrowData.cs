public static class HintArrowData
{
    public static IHintArrow CurrentHintArrow;

    public static void SetNewArrow(IHintArrow arrow)
    {
        CurrentHintArrow?.Remove(0.2f);
        CurrentHintArrow = arrow;
    }

    public static void ClearCurrentArrow(IHintArrow arrow)
    {
        if(arrow == CurrentHintArrow)
            CurrentHintArrow = null;
    }
}
