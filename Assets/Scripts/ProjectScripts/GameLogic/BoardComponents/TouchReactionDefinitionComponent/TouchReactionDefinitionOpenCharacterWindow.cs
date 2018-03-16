public class TouchReactionDefinitionOpenCharacterWindow : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var model = UIService.Get.GetCachedModel<UICharacterWindowModel>(UIWindowType.CharacterWindow);

        model.Hero = GameDataService.Current.HeroesManager.GetHero(piece.PieceType == PieceType.H1.Id ? "Robin" : "John"); 
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
        return true;
    }
}