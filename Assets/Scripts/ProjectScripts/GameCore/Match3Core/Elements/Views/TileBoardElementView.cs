using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoardElementView : BoardElementView
{
	[SerializeField] private List<string> lockers = new List<string>();

	public override void Init(BoardRenderer context)
    {
		var position = context.GetBoardPosition(this);
        if (context.Context.BoardLogic.IsEmpty(position.Left))
		{

		}
    }

	public override void SyncRendererLayers(BoardPosition boardPosition)
    {

	}
}
