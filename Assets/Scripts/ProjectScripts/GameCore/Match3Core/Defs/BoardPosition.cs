using System.Collections;
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
    public bool IsValid
    {
        get
        {
            return X >= 0 && Y >= 0;
        }
    }

    public bool IsValidFor(int w, int h)
    {
        if (IsValid && X < w && Y < h)
        {
            return true;
        }

        return false;
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
            BoardPosition vector = (BoardPosition)other;
            result = (this.X.Equals(vector.X) && this.Y.Equals(vector.Y) && this.Z.Equals(vector.Z));
        }
        return result;
    }

    public override int GetHashCode()
    {
        return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
    }

    public override string ToString()
    {
        return string.Format("[{0},{1},{2}]", X, Y, Z);
    }

    public static BoardPosition Default()
    {
        return new BoardPosition(-1, -1);
    }

    public static BoardPosition Zero()
    {
        return new BoardPosition(0, 0);
    }

    public static float SqrMagnitude(BoardPosition from, BoardPosition to)
    {
        return (to.X - from.X) * (to.X - from.X) + (to.Y - from.Y) * (to.Y - from.Y);
    }

    public bool Equals(BoardPosition other)
    {
        return (this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z));
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
    public BoardPosition Up
    {
        get
        {
            return new BoardPosition(X, Y + 1, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Down
    {
        get
        {
            return new BoardPosition(X, Y - 1, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Left
    {
        get
        {
            return new BoardPosition(X - 1, Y, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition Right
    {
        get
        {
            return new BoardPosition(X + 1, Y, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition TopRight
    {
        get
        {
            return new BoardPosition(X + 1, Y + 1, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition BottomRight
    {
        get
        {
            return new BoardPosition(X + 1, Y - 1, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition TopLeft
    {
        get
        {
            return new BoardPosition(X - 1, Y + 1, Z);
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public BoardPosition BottomLeft
    {
        get
        {
            return new BoardPosition(X - 1, Y - 1, Z);
        }
    }

}
