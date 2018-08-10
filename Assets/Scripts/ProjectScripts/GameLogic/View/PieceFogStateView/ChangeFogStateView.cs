using System.Collections.Generic;
using UnityEngine;

public class ChangeFogStateView : UIBoardView, IBoardEventListener
{
	[SerializeField] private NSText message;
	[SerializeField] private NSText price;
	
	private FogDef def;
	
	protected override ViewType Id
	{
		get { return ViewType.FogState; }
	}
	
	public override Vector3 Ofset
	{
		get { return new Vector3(0, 0.1f); }
	}
	
	public override void Init(Piece piece)
	{
		base.Init(piece);
		
		Priority = defaultPriority = 0;
		
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
	}

	public override void SetOfset()
	{
		def = GameDataService.Current.FogsManager.GetDef(new BoardPosition(Context.CachedPosition.X,
			Context.CachedPosition.Y));
		
		if(def == null) return;
		
		CachedTransform.position = def.GetCenter(Context.Context) + Ofset;
	}

	public override void ResetViewOnDestroy()
	{
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
		
		base.ResetViewOnDestroy();
	}

	protected override void UpdateView()
	{
		if(def == null) return;
		
		message.Text = "Clear fog";
		price.Text = def.Condition.ToStringIcon();
	}
	
	public void OnBoardEvent(int code, object context)
	{
		if (code != GameEventsCodes.ClosePieceUI || context is FogDef && ((FogDef) context).Uid == def.Uid) return;
		
		Change(false);
	}
	
	public void Clear()
	{
		CurrencyHellper.Purchase(Currency.Fog.Name, 1, def.Condition, success =>
		{
			if (success == false) return;
			
			var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

			if (views != null)
			{
				foreach (var view in views)
				{
					view.UpdateResource(0);
				}
			}
                
			Context.Context.ActionExecutor.AddAction(new CollapsePieceToAction
			{
				To = Context.CachedPosition,
				Positions = new List<BoardPosition>{Context.CachedPosition}
			});
		});
	}
}