﻿using UnityEngine;

public class ChangeFogStateView : UIBoardView, IBoardEventListener
{
	[SerializeField] private NSText label;
	
	[SerializeField] private RectTransform line;
	
	private FogDef def;
	
	public override Vector3 Ofset
	{
		get { return new Vector3(0, 1.5f); }
	}
	
	public override void Init(Piece piece)
	{
		base.Init(piece);
		
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
	}

	protected override void SetOfset()
	{
		def = GameDataService.Current.FogsManager.GetDef(new BoardPosition(Context.CachedPosition.X,
			Context.CachedPosition.Y));
		
		if(def == null) return;
		
		CachedTransform.position = def.GetCenter(Context.Context) + Ofset;
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
        
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
	}

	public override void UpdateVisibility(bool isVisible)
	{
		base.UpdateVisibility(isVisible);
		
		if(IsShow == false || def == null) return;
		
		var power = GameDataService.Current.HeroesManager.CurrentPower();
		
		label.Text = string.Format("{0}/{1}", power, def.Condition.Value);

		line.sizeDelta = new Vector2(Mathf.Clamp(10 + 153 * (float) power / def.Condition.Value, 10, 163), line.sizeDelta.y);
	}
	
	public void OnBoardEvent(int code, object context)
	{
		if (code != GameEventsCodes.ClosePieceMenu || context is FogDef && ((FogDef) context).Uid == def.Uid) return;
		
		Change(false);
	}
}