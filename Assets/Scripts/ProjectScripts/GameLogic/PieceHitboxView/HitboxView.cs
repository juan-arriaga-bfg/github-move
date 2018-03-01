﻿using UnityEngine;

public class HitboxView : IWBaseMonoBehaviour 
{

	[SerializeField] private NSText maxHealthLabel;
	
	[SerializeField] private NSText currentHealthLabel;
	

	private PieceBoardElementView context;

	private LivePieceComponent livePieceComponent;

	private int prevHealth = -1;
	

	public virtual void Init(PieceBoardElementView context)
	{
		this.context = context;
	}

	private void OnDisable()
	{
		livePieceComponent = null;
		prevHealth = -1;
	}

	private void Update()
	{
		if (context == null || context.Piece == null) return;

		if (livePieceComponent == null)
		{
			livePieceComponent = context.Piece.GetComponent<LivePieceComponent>(LivePieceComponent.ComponentGuid);
		}
		
		if (livePieceComponent == null) return;
		
		if (prevHealth == livePieceComponent.HitPoints) return;

		if (prevHealth != -1)
		{
			int damageAmount = Mathf.Abs(prevHealth - livePieceComponent.HitPoints);

			var hitboxDamageView = context.Context.CreateBoardElementAt<HitboxDamageView>(R.HitboxDamageView, new BoardPosition(100,100,3));
			hitboxDamageView.CachedTransform.position = CachedTransform.position;
			hitboxDamageView.ApplyDamage(damageAmount, CachedTransform.position + Vector3.up);
		}

		prevHealth = livePieceComponent.HitPoints;

		maxHealthLabel.Text = livePieceComponent.MaxHitPoints.ToString();

		currentHealthLabel.Text = Mathf.Clamp(livePieceComponent.HitPoints, 0, int.MaxValue).ToString();

	}
}
