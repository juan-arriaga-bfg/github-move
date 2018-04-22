public class TouchReactionDefinitionOpenHeroesWindow : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        UIService.Get.ShowWindow(UIWindowType.HeroesWindow);
        return true;
    }
}