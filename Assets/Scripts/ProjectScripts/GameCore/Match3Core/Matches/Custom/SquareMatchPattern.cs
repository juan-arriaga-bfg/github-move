using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMatchPattern : IMatchPattern
{
    private int fromType = 100;
    private int toType = 199;
    private int minCountForMatch = 3;

    public List<MatchDef> GetMatches(int[,,] matrix, int[,] defMatrix, int w, int h, int layer)
    {
        var matchDefs = new List<MatchDef>();

        var processedPoints = new List<BoardPosition>();

        for (int y = 0; y < h - 1; y++)
        {
            for (int x = 0; x < w - 1; x++)
            {
                var point = new BoardPosition(x, y, layer);
                var pointTop = new BoardPosition(x, y + 1, layer);
                var pointRight = new BoardPosition(x + 1, y, layer);
                var pointTopRight = new BoardPosition(x + 1, y + 1, layer);

                var type = matrix[x, y, layer];
                var nextTypeTop = matrix[x, y + 1, layer];
                var nextTypeRight = matrix[x + 1, y, layer];
                var nextTypeTopRight = matrix[x + 1, y + 1, layer];

                if (processedPoints.Contains(point) == false
                    && processedPoints.Contains(pointTop) == false
                    && processedPoints.Contains(pointRight) == false
                    && processedPoints.Contains(pointTopRight) == false
                    && type == nextTypeTop
                    && type == nextTypeRight
                    && type == nextTypeTopRight
                    && type >= fromType
                    && type <= toType
                    && defMatrix[x, y] == 1
                    && defMatrix[x + 1, y] == 1
                    && defMatrix[x, y + 1] == 1
                    && defMatrix[x + 1, y + 1] == 1)
                {
                    var matchDef = new MatchDef(this);

                    matchDef.AddPoint(point)
                            .AddPoint(pointTop)
                            .AddPoint(pointRight)
                            .AddPoint(pointTopRight);

                    processedPoints.AddRange(matchDef.MatchPoints);

                    matchDefs.Add(matchDef);
                }
            }
        }
        return matchDefs;
    }
}
