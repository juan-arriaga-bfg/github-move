using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchClaster 
{
	private List<MatchDef> matchDefs;
    private List<BoardPosition> intersectionPoints;

    public List<MatchDef> MatchDefs { get { return matchDefs; } }
    public List<BoardPosition> IntersectionPoints { get { return intersectionPoints; } }

	public MatchClaster()
	{
		this.matchDefs = new List<MatchDef>();
		this.intersectionPoints = new List<BoardPosition>();
	}

    public MatchClaster AddMatchDef(MatchDef def)
    {
        if (matchDefs.Contains(def) == false)
        {
            matchDefs.Add(def);
        }

        return this;
    }

    public MatchClaster AddIntersection(BoardPosition intersection)
    {
        if (intersectionPoints.Contains(intersection) == false)
        {
            intersectionPoints.Add(intersection);
        }

        return this;
    }
}
