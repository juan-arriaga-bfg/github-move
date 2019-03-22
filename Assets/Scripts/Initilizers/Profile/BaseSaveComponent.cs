using System;
using System.Collections.Generic;
using System.Text;

public class BaseSaveComponent : ECSEntity
{
    public override int Guid { get; }
    
    protected string PositionsToString(List<BoardPosition> positions)
    {
        if (GameDataService.Current == null) return null;
		
        var str = new StringBuilder();

        foreach (var position in positions)
        {
            str.Append(position.ToSaveString());
            str.Append(";");
        }
		
        return str.ToString();
    }
    
    protected List<BoardPosition> StringToPositions(string value)
    {
        var positions = new List<BoardPosition>();
		
        if (string.IsNullOrEmpty(value)) return positions;
		
        var data = value.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var str in data)
        {
            positions.Add(BoardPosition.Parse(str));
        }
		
        return positions;
    }
    
    protected string IdsToString(List<string> ids)
    {
        return string.Join(";", ids);
    }

    protected List<string> StringToIds(string value)
    {
        var ret = new List<string>();

        if (string.IsNullOrEmpty(value))
        {
            return ret;
        }
		
        var data = value.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < data.Length; i++)
        {
            var str = data[i];
            ret.Add(str);
        }

        return ret; 
    }
}
