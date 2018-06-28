public class TouchReactionDefinitionProduction : TouchReactionDefinitionComponent
{
    public ProductionComponent Production { get; set; }

    private ViewDefinitionComponent viewDef;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, Production);

        if (Production.IsActive == false)
        {
            UIErrorWindowController.AddError(string.Format("This building will be unlocked at Level {0}", Production.Level));
            return false;
        }
        
        switch (Production.State)
        {
            case ProductionState.Waiting:
                UIMessageWindowController.CreateTimerCompleteMessage(
                    "Complete now!",
                    "Would you like to build the item right now for crystals?",
                    "Complete now ",
                    Production.Timer,
                    () => CurrencyHellper.Purchase(Currency.Product.Name, 1, Production.Timer.GetPrise(), success =>
                    {
                        if(success == false) return;
                        Production.Fast();
                    }));
                break;
            case ProductionState.Completed:
                UIService.Get.GetShowedView<UIProductionWindowView>(UIWindowType.ProductionWindow).Change(false);
                Production.Complete();
                break;
            default:
                Production.Change();
                break;
        }
        
        return true;
    }
}