using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public struct BoardPosition : IEquatable<BoardPosition>
{
    public int X;
    public int Y;
    public int Z;

    public BoardPosition(int x, int y, int z = 0)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    [JsonIgnore]
    public bool IsValid => X >= 0 && Y >= 0;

    public bool IsValidFor(int w, int h)
    {
        return IsValid && X < w && Y < h;
    }

    public static BoardPosition operator +(BoardPosition first, BoardPosition second)
    {
        return new BoardPosition(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
    }

    public static BoardPosition operator -(BoardPosition first, BoardPosition second)
    {
        return new BoardPosition(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
    }

    public static BoardPosition operator *(BoardPosition pos, int multiply)
    {
        return new BoardPosition(pos.X * multiply, pos.Y * multiply, pos.Z * multiply);
    }
    
    public bool IsNeighbor(BoardPosition other)
    {
        if (Equals(other)) return false;
        
        return ((X - 1 == other.X || X + 1 == other.X) && Y == other.Y) ||
               ((Y - 1 == other.Y || Y + 1 == other.Y) && X == other.X);
    }
    
    public bool IsDiagonalNeighbor(BoardPosition other)
    {
        if (Equals(other)) return false;
        
        return other.X >= X - 1 && other.X <= X + 1 && other.Y >= Y - 1 && other.Y <= Y + 1;
    }

    public BoardPosition NextTo(BoardPosition to)
    {
        return new BoardPosition((int)Mathf.Clamp(to.X - this.X, -1, 1) + this.X, (int)Mathf.Clamp(to.Y - this.Y, -1, 1) + this.Y, (int)Mathf.Clamp(to.Z - this.Z, -1, 1) + this.Z);
    }

    public static BoardPosition Direction(BoardPosition from, BoardPosition to)
    {
        return new BoardPosition((int)Mathf.Clamp(to.X - from.X, -1, 1), (int)Mathf.Clamp(to.Y - from.Y, -1, 1), (int)Mathf.Clamp(to.Z - from.Z, -1, 1));
    }

    public static BoardPosition GetRandomPosition(BoardPosition from, BoardPosition to, int seed = 0)
    {
        var rand = new System.Random(seed);
        int x = rand.Next(from.X, to.X);
        int y = rand.Next(from.Y, to.Y);
        int z = rand.Next(from.Z, to.Z);

        return new BoardPosition(x, y, z);
    }

    public override bool Equals(object other)
    {
        bool result;
        if (!(other is BoardPosition))
        {
            result = false;
        }
        else
        {
            var vector = (BoardPosition)other;
            result = (X.Equals(vector.X) && Y.Equals(vector.Y) && Z.Equals(vector.Z));
        }
        return result;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;
    }

    public Vector3 ToVector()
    {
        return new Vector3(X, Y, Z);
    }

    public override string ToString()
    {
        return $"[{X},{Y},{Z}]";
    }

    public string ToSaveString()
    {
        return $"{X},{Y},{Z}";
    }

    public static BoardPosition Default()
    {
        return new BoardPosition(-1, -1);
    }

    public static BoardPosition Zero()
    {
        return new BoardPosition(0, 0);
    }

    public static BoardPosition Parse(string value)
    {
        var valueArray = value.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var result = new int[] {0, 0, 0};

        for (var i = 0; i < result.Length; i++)
        {
            if(i == valueArray.Length) break;
            
            int j;
            
            if(int.TryParse(valueArray[i], out j) == false) break;

            result[i] = j;
        }
        
        return new BoardPosition(result[0], result[1], result[2]);
    }
    
    public static List<BoardPosition> GetLine(BoardPosition dot, int length)
    {
        var line = new List<BoardPosition> {dot};
        
        for (var i = 1; i < length; i++)
        {
            dot = dot.Up;
            line.Add(dot);
        }

        return line;
    }
    
    public static List<BoardPosition> GetLineInCenter(BoardPosition dot, int length)
    {
        dot = dot.DownAtDistance((int) (length * 0.5f));
        
        var line = new List<BoardPosition> {dot};
        
        for (var i = 1; i < length; i++)
        {
            dot = dot.Up;
            line.Add(dot);
        }

        return line;
    }

    public static List<BoardPosition> GetRect(BoardPosition dot, int width, int height)
    {
        var rect = new List<BoardPosition>();
        
        rect.AddRange(GetLine(dot, height));

        for (var i = 1; i < width; i++)
        {
            dot = dot.Right;
            rect.AddRange(GetLine(dot, height));
        }

        return rect;
    }
    
    public static List<BoardPosition> GetRectInCenter(BoardPosition dot, int width, int height)
    {
        var rect = new List<BoardPosition>();

        dot = dot.LeftAtDistance((int) (width * 0.5f));
        
        rect.AddRange(GetLineInCenter(dot, height));

        for (var i = 1; i < width; i++)
        {
            dot = dot.Right;
            rect.AddRange(GetLineInCenter(dot, height));
        }

        return rect;
    }
    
    public static List<BoardPosition> GetRectInCenter(BoardPosition center, int width, int height, bool includeCenter)
    {
        var rect = new List<BoardPosition>();

        var dot = center.LeftAtDistance((int) (width * 0.5f));

        var line = GetLineInCenter(dot, height);
        for (var i = 0; i < line.Count; i++)
        {
            var point = line[i];
            if (includeCenter || !point.Equals(center))
            {
                rect.Add(point);
            }
        }

        for (var i = 1; i < width; i++)
        {
            dot = dot.Right;
            line = GetLineInCenter(dot, height);
            for (var index = 0; index < line.Count; index++)
            {
                var point = line[index];
                if (includeCenter || !point.Equals(center))
                {
                    rect.Add(point);
                }
            }
        }
        
        return rect;
    }
    
    public static List<BoardPosition> GetRectInCenterForArea(BoardPosition center, int width, int height, int areaW, int areaH, bool includeCenter)
    {
        var rect = new List<BoardPosition>();

        var dot = center.LeftAtDistance((int) (width * 0.5f));

        var line = GetLineInCenter(dot, height);
        for (var i = 0; i < line.Count; i++)
        {
            var point = line[i];
            if (point.IsValidFor(areaW, areaH) && (includeCenter || !point.Equals(center)))
            {
                rect.Add(point);
            }
        }

        for (var i = 1; i < width; i++)
        {
            dot = dot.Right;
            line = GetLineInCenter(dot, height);
            for (var index = 0; index < line.Count; index++)
            {
                var point = line[index];
                if (point.IsValidFor(areaW, areaH) && (includeCenter || !point.Equals(center)))
                {
                    rect.Add(point);
                }
            }
        }
        
        return rect;
    }
    
    public static float SqrMagnitude(BoardPosition from, BoardPosition to)
    {
        return (to.X - from.X) * (to.X - from.X) + (to.Y - from.Y) * (to.Y - from.Y);
    }

    public static BoardPosition GetCenter(List<BoardPosition> positions)
    {
        var center = new Vector3(0, 0, 0);

        foreach (var position in positions)
        {
            center += position.ToVector();
        }
        
        center = center / positions.Count;
        
        return new BoardPosition(Mathf.RoundToInt(center.x), Mathf.RoundToInt(center.y), Mathf.RoundToInt(center.z));
    }

    public static void GetAABB(List<BoardPosition> positions, out BoardPosition topLeft, out BoardPosition topRight, out BoardPosition bottomRight, out BoardPosition bottomLeft)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;        
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        foreach (var pos in positions)
        {
            if (minX > pos.X) {minX = pos.X;}
            if (maxX < pos.X) {maxX = pos.X;}
            if (minY > pos.Y) {minY = pos.Y;}
            if (maxY < pos.Y) {maxY = pos.Y;}
        }
        
        topLeft     = new BoardPosition(minX, maxY);
        topRight    = new BoardPosition(maxX, maxY);
        bottomRight = new BoardPosition(maxX, minY);
        bottomLeft  = new BoardPosition(minX, minY);
    }

    public bool Equals(BoardPosition other)
    {
        return (this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z));
    }

    public List<BoardPosition> Neighbors()
    {
        return new List<BoardPosition>
        {
            Left,
            Up,
            Right,
            Down
        };
    }

    public List<BoardPosition> NeighborsWithDiagonal()
    {
        return new List<BoardPosition>
        {
            Left,
            Up,
            Right,
            Down,
            TopLeft,
            BottomLeft,
            TopRight,
            BottomRight
        };
    }
    
    public List<BoardPosition> NeighborsFor(int w, int h)
    {
        var neighbors = new List<BoardPosition>();
        var neighbor = Left;
        
        if(neighbor.IsValid) neighbors.Add(neighbor);
        
        neighbor = Down;
        
        if(neighbor.IsValid) neighbors.Add(neighbor);
        
        neighbor = Right;
        
        if(neighbor.X < w && neighbor.Y < h) neighbors.Add(neighbor);
        
        neighbor = Up;
        
        if(neighbor.X < w && neighbor.Y < h) neighbors.Add(neighbor);

        return neighbors;
    }
    
    public BoardPosition UpAtDistance(int distance)
    {
        return new BoardPosition(X, Y + distance, Z);
    }

    public BoardPosition DownAtDistance(int distance)
    {
        return new BoardPosition(X, Y - distance, Z);
    }
    
    public BoardPosition LeftAtDistance(int distance)
    {
        return new BoardPosition(X - distance, Y, Z);
    }
    
    public BoardPosition RightAtDistance(int distance)
    {
        return new BoardPosition(X + distance, Y, Z);
    }
    
    public BoardPosition TopRightAtDistance(int distance)
    {
        return new BoardPosition(X + distance, Y + distance, Z);
    }
    
    public BoardPosition BottomRightAtDistance(int distance)
    {
        return new BoardPosition(X + distance, Y - distance, Z);
    }
    
    public BoardPosition TopLeftAtDistance(int distance)
    {
        return new BoardPosition(X - distance, Y + distance, Z);
    }
    
    public BoardPosition BottomLeftAtDistance(int distance)
    {
        return new BoardPosition(X - distance, Y - distance, Z);
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Up => new BoardPosition(X, Y + 1, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Down => new BoardPosition(X, Y - 1, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Left => new BoardPosition(X - 1, Y, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Right => new BoardPosition(X + 1, Y, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition TopRight => new BoardPosition(X + 1, Y + 1, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition BottomRight => new BoardPosition(X + 1, Y - 1, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition TopLeft => new BoardPosition(X - 1, Y + 1, Z);

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition BottomLeft => new BoardPosition(X - 1, Y - 1, Z);
}
