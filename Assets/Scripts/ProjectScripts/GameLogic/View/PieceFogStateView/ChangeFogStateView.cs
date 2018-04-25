using DG.Tweening;
using UnityEngine;

public class ChangeFogStateView : UIBoardView, IBoardEventListener
{
	[SerializeField] private NSText label;
	
	[SerializeField] private RectTransform line;
	
	private FogDef def;

	private TouchReactionDefinitionFog definition;
	
	public override Vector3 Ofset
	{
		get { return new Vector3(0, 4f); }
	}
	public override void Init(Piece piece)
	{
		base.Init(piece);

		var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
		
		if(reaction == null) return;
		
		definition = reaction.GetComponent<TouchReactionDefinitionFog>(TouchReactionDefinitionFog.ComponentGuid);
		
		if(definition == null) return;
		
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
	}
	
	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
        
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
	}
	
	private void Update()
	{
		if (def == null)
		{
			var pos = new BoardPosition(Context.CachedPosition.X, Context.CachedPosition.Y);
            
			if (GameDataService.Current.FogsManager.Fogs.TryGetValue(pos, out def) == false) return;
		}

		var power = GameDataService.Current.HeroesManager.CurrentPower();
		var isComplete = power >= def.Condition.Value;
		
		Change(definition.IsOpen || isComplete);
		
		label.Text = string.Format("{0}/{1}", power, def.Condition.Value);

		line.sizeDelta = new Vector2(Mathf.Clamp(163 * (float) power / def.Condition.Value, 10, 163), line.sizeDelta.y);
	}
	
	public void OnBoardEvent(int code, object context)
	{
		if (code != GameEventsCodes.ClosePieceMenu) return;
		
		if(definition == null || definition == context) return;
		
		definition.IsOpen = false;
	}
}