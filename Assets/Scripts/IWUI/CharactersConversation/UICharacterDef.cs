using UnityEngine;

public class UICharacterDef
{
    public string Id;
    public string Name;
    public string ColorHex;
    public Color Color => GetColorByHex(ColorHex);
    public int PieceId = PieceType.None.Id;

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