using UnityEngine;

public class HitboxView : IWBaseMonoBehaviour 
{

	[SerializeField] private NSText maxHealthLabel;
	
	[SerializeField] private NSText currentHealthLabel;
	

	private PieceBoardElementView context;

	private LivePieceComponent livePieceComponent;

	public virtual void Init(PieceBoardElementView context)
	{
		this.context = context;
	}

	private void OnDisable()
	{
		livePieceComponent = null;
	}

	private void Update()
	{
		if (context == null || context.Piece == null) return;

		if (livePieceComponent == null)
		{
			livePieceComponent = context.Piece.GetComponent<LivePieceComponent>(LivePieceComponent.ComponentGuid);
		}
		
		if (livePieceComponent == null) return;

		maxHealthLabel.Text = livePieceComponent.MaxHitPoints.ToString();

		currentHealthLabel.Text = Mathf.Clamp(livePieceComponent.HitPoints, 0, int.MaxValue).ToString();

	}
}
