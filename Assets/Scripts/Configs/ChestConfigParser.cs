using System.Linq;
using System.Text;

public class ChestsConfigqParser : IConfigParser
{
    public string Parse(string text)
    {
        return "OK!!!!!!!!!!!";
        
        var result = new StringBuilder("[");
        var textToParse = text.Replace("\r", string.Empty);
        
        var columns = textToParse.Split('\t').ToList();
        
        foreach (var column in columns)
        {
            var lines = column.Split('\n').ToList();
            foreach (var line in lines)
            {
                result.Append(line);
                result.Append(",");
            }
        }
        
        result.Append("]");
        
        return result.ToString();
    }
}