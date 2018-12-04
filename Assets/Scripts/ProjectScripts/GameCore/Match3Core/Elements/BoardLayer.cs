
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public partial class BoardLayerDef
{
    public int Index;

    public int Layer
    {
        get { return BoardService.Current.FirstBoard.BoardDef.GetLayerFor(Index); }
    }

    public List<string> Names = new List<string>();
}

public partial class BoardLayer
{
    private static Dictionary<int, BoardLayerDef> cachedBoardLayerByLayer;

    protected static void CacheRuntime()
    {
        if (CheckIsInitilized() == false) return;
            
        if (cachedBoardLayerByLayer != null) return;
        
        cachedBoardLayerByLayer = new Dictionary<int, BoardLayerDef>();
        
        var t = typeof(BoardLayer);
        var fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.Static);
        
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            var fieldInfo = fieldInfos[i];
            if (fieldInfo.FieldType != typeof(BoardLayerDef)) continue;
            
            var fieldValue = (BoardLayerDef)fieldInfo.GetValue(null);
            
            cachedBoardLayerByLayer.Add(fieldValue.Layer, fieldValue);
        }
    }

    protected static bool CheckIsInitilized()
    {
        if (BoardService.Current == null || BoardService.Current.FirstBoard == null)
        {
            Debug.LogWarning($"[BoardLayer] can't use IsValidLayer before game board initialization");

            return false;
        }

        return true;
    }

    public static string LogValidLayers()
    {
        if (CheckIsInitilized() == false) return "none";
        
        CacheRuntime();

        var log = new StringBuilder();
        foreach (var cachedBoardLayerByLayerPair in cachedBoardLayerByLayer)
        {
            log.Append($"Layer:{cachedBoardLayerByLayerPair.Value.Layer} Names:{string.Join(";", cachedBoardLayerByLayerPair.Value.Names)} ");
        }

        return log.ToString();
    }

    public static bool IsValidLayer(int layer)
    {
        if (CheckIsInitilized() == false) return false;
        
        CacheRuntime();
        
        if (cachedBoardLayerByLayer.ContainsKey(layer))
        {
            return true;
        }
        
        Debug.LogError($"[BoardLayer] invalid layer (Z position): {layer} use one of valid layers:{LogValidLayers()}");

        return false;
    }
    
    public static int GetDefaultLayerIndexBy(BoardPosition boardPosition, int width, int height)
    {
        var layer = -boardPosition.Y + width + height * boardPosition.X + width * height * boardPosition.Z - 32767;
        if (layer > 32767 || layer < -32767)
        {
            Debug.LogError($"GetLayerIndexBy: invalid layer:{layer} for:{boardPosition}");
        }
        
        return layer;
    }

    public static readonly BoardLayerDef Default = new BoardLayerDef {Index = 0, Names = new List<string>{"Default"}};
    
    public static readonly BoardLayerDef Piece = new BoardLayerDef {Index = 1, Names = new List<string>{"Piece"}};
    
    public static readonly BoardLayerDef PieceUP1 = new BoardLayerDef {Index = 2, Names = new List<string>{"PieceUP1"}};
    
    public static readonly BoardLayerDef FX = new BoardLayerDef {Index = 3, Names = new List<string>{"FX"}};
    
    public static readonly BoardLayerDef UI = new BoardLayerDef {Index = 4, Names = new List<string>{"UI"}};
    
    public static readonly BoardLayerDef UIUP1 = new BoardLayerDef {Index = 5, Names = new List<string>{"UIUP1"}};
    
    public static readonly BoardLayerDef MAX = new BoardLayerDef {Index = 6, Names = new List<string>{"MAX"}};
    
}
