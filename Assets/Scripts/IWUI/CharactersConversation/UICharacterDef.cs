using UnityEngine;

public class UICharacterDef
{
    public string Name;
    public string ColorHex;
    public Color Color => GetColorByHex(ColorHex);

    private static Color GetColorByHex(string hex)
    {
        Color color = Color.white;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }

        return color;
    }
}