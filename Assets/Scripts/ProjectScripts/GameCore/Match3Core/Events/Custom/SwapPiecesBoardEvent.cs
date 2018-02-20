using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPiecesBoardEvent : BoardEvent
{
	public BoardPosition From { get; set; }

	public BoardPosition To { get; set; }

	public bool IsSuccess { get; set; }
}
