using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnMatchPattern : IMatchPattern
{
    private int fromType = 100;
    private int toType = 199;
    private int minCountForMatch = 3;

    public List<MatchDef> GetMatches(int[,,] matrix, int[,] defMatrix, int w, int h, int layer)
    {
        var matchDefs = new List<MatchDef>();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h - 2; y++)
            {
                int matchCount = GetMatchesFromPoint(matrix, defMatrix, h, x, y, layer);
                if (matchCount >= minCountForMatch)
                {
                    var matchDef = new MatchDef(this);
                    for (int i = y; i < (y + matchCount); i++)
                    {
                        var point = new BoardPosition(x, i, layer);
                        matchDef.AddPoint(point);
                    }
                    matchDefs.Add(matchDef);
                }
                y = y + matchCount - 1;
            }
        }
        return matchDefs;
    }

    public int GetMatchesFromPoint(int[,,] matrix, int[,] defMatrix, int h, int x, int y, int z, int count = 1)
    {
        if (y + 1 < h)
        {
            var type = matrix[x, y, z];
            var nextType = matrix[x, y + 1, z];

            if (type == nextType
                && type >= fromType
                && type <= toType
                && defMatrix[x, y] == 1
                && defMatrix[x, y + 1] == 1)
            {
                count++;
                return GetMatchesFromPoint(matrix, defMatrix, h, x, y + 1, z, count);
            }
        }
        return count;
    }
}
