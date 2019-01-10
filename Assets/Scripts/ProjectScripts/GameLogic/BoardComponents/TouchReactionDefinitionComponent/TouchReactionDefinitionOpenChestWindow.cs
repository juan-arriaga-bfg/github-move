using UnityEngine;

public class TouchReactionDefinitionOpenChestWindow : TouchReactionDefinitionComponent
{
	private ChestPieceComponent chestComponent;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (chestComponent == null) chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
		
		if (chestComponent?.Def == null || chestComponent.Rewards.IsHightlight) return false;

		var model = UIService.Get.GetCachedModel<UIChestMessageWindowModel>(UIWindowType.ChestMessage);

		//TODO insert sound
		Debug.LogError("Not implemented sound #chest_open");
		
		model.ChestComponent = chestComponent;
				
		UIService.Get.ShowWindow(UIWindowType.ChestMessage);
		
		return true;
	}
}