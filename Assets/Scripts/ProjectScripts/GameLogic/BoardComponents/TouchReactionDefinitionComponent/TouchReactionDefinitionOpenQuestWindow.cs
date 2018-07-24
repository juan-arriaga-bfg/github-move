public class TouchReactionDefinitionOpenQuestWindow : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		var model = UIService.Get.GetCachedModel<UIQuestWindowModel>(UIWindowType.QuestWindow);
        
		model.Quest = GameDataService.Current.QuestsManager.ActiveQuests[0];
        
		UIService.Get.ShowWindow(UIWindowType.QuestWindow);
		return true;
	}
}