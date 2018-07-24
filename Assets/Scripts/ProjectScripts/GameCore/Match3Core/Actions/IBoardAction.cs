using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardAction  
{
	bool PerformAction(BoardController gameBoardController);
	
	int Guid { get;  }
}
