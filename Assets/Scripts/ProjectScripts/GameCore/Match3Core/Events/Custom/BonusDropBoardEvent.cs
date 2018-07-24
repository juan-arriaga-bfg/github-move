using UnityEngine;

public class BonusDropBoardEvent : BoardEvent 
{
	public string ItemId { get; set; }

	public int Amount { get; set; }

	public Vector2 ScreenPosition { get; set; }
}
