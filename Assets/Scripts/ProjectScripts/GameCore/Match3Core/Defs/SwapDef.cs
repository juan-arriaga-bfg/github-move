using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapDef
{
	public BoardPosition From { get; set; }

	public BoardPosition To { get; set; }

	public MatchDef MatchDef { get; set; }

	public virtual int GetPriority()
	{
		return MatchDef.MatchPoints.Count;
	}
}
