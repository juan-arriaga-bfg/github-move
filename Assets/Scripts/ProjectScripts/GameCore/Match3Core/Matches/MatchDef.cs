using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MatchDef
{
    private IMatchPattern pattern;

    private List<BoardPosition> matchPoints;

    public List<BoardPosition> MatchPoints { get { return matchPoints; } }

    public IMatchPattern Pattern { get { return pattern; } }

    [JsonIgnore]
    public MatchClaster Claster { get; set; }

    public MatchDef(IMatchPattern pattern)
    {
        this.pattern = pattern;
    }

    public virtual int GetPriority()
    {
        return matchPoints == null ? 0 : matchPoints.Count;
    }

    public MatchDef AddPoints(List<BoardPosition> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            AddPoint(points[i]);
        }

        return this;
    }

    public MatchDef AddPoint(BoardPosition point)
    {
        if (matchPoints == null) matchPoints = new List<BoardPosition>();

        if (matchPoints.Contains(point) == false)
        {
            matchPoints.Add(point);
        }

        return this;
    }

    public MatchDef SetClaster(MatchClaster claster)
    {
        Claster = claster;

        return this;
    }

    public override string ToString()
    {
        var st = new System.Text.StringBuilder();

        st.Append("match => (");

        for (int i = 0; i < MatchPoints.Count; i++)
        {
            var point = MatchPoints[i];
            st.Append(string.Format("p{0}:[{1},{2},{3}] ", i, point.X, point.Y, point.Z));
        }

        st.Append(")");

        return st.ToString();
    }
}