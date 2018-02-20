using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMatchPattern
{
    List<MatchDef> GetMatches(int[,,] matrix, int[,] defMatrix, int w, int h, int layer);
}
