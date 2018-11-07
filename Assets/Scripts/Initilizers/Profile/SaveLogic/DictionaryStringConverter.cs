using System;
using System.Collections.Generic;
using System.Text;

public static class DictionaryStringConverter
{
	public static string ToSaveString<TKey, TValue>(this Dictionary<TKey, TValue> target)
	{
		var strBuilder = new StringBuilder();
		
		if(target == null) return strBuilder.ToString();

		foreach (var elem in target)
		{
			strBuilder.Append(elem.Key);
			strBuilder.Append(",");
			strBuilder.Append(elem.Value);
			strBuilder.Append(",");
		}

		if (target.Count > 0) strBuilder.Remove(strBuilder.Length - 1, 1);
		
		return strBuilder.ToString();
	}

	public static Dictionary<TKey, TValue> FromDataArray<TKey, TValue>(string[] dataArray, Func<string, TKey> parseKey, Func<string, TValue> parseValue, int beginPosition, int endPosition = -1)
	{
		if (endPosition < 0) endPosition = dataArray.Length;
        
		var dict = new Dictionary<TKey, TValue>();
        
		for (var i = beginPosition; i < endPosition; i+=2)
		{
			dict.Add(parseKey(dataArray[i]), parseValue(dataArray[i+1]));
		}

		return dict;
	}
}