using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowMatchPattern : IMatchPattern
{
    private int fromType = 100;
    private int toType = 199;
    private int minCountForMatch = 3;

    public List<MatchDef> GetMatches(int[,,] matrix, int[,] defMatrix, int w, int h, int layer)
    {
        var matchDefs = new List<MatchDef>();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w - 2; x++)
            {
                int matchCount = GetMatchesFromPoint(matrix, defMatrix, w, x, y, layer);
                if (matchCount >= minCountForMatch)
                {
                    var matchDef = new MatchDef(this);
                    for (int i = x; i < (x + matchCount); i++)
                    {
                        var point = new BoardPosition(i, y, layer);
                        matchDef.AddPoint(point);
                    }
                    matchDefs.Add(matchDef);
                }
                x = x + matchCount - 1;
            }
        }
        return matchDefs;
    }

    public int GetMatchesFromPoint(int[,,] matrix, int[,] defMatrix, int w, int x, int y, int z, int count = 1)
    {
        if (x + 1 < w)
        {
            var type = matrix[x, y, z];
            var nextType = matrix[x + 1, y, z];

            if (type == nextType 
                && type >= fromType
                && type <= toType
                && defMatrix[x, y] == 1
                && defMatrix[x + 1, y] == 1)
            {
                count++;
                return GetMatchesFromPoint(matrix, defMatrix, w, x + 1, y, z, count);
            }
        }
        return count;
    }
}
