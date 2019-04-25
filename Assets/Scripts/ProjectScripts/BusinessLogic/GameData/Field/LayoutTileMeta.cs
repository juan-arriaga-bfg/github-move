public struct LayoutTileMeta
{
    // ReSharper disable InconsistentNaming
    public int tileId;
    public bool neighborR ;
    public bool neighborL ;
    public bool neighborT ;
    public bool neighborB ;
        
    public bool neighborBL;
    public bool neighborBR;        
    public bool neighborTL;
    public bool neighborTR;
        
    public int floorDiffR;
    public int floorDiffL;
    public int floorDiffT;
    public int floorDiffB;
        
    public int floorDiffBL;
    public int floorDiffBR;        
    public int floorDiffTL;
    public int floorDiffTR;
    // ReSharper restore InconsistentNaming
        
    private string BoolToString(bool v)
    {
        return v ? "1" : "0";
    }
        
    public override string ToString()
    {
        // return $"<mspace=1.5em>T:{BoolToString(neighborT)} R:{BoolToString(neighborR)}\nB:{BoolToString(neighborB)} L:{BoolToString(neighborL)}\nBL:{BoolToString(neighborBL)} TR:{BoolToString(neighborTR)}</mspace>";
        return $"<mspace=1.5em>T:{(floorDiffT)} R:{(floorDiffR)}\nB:{(floorDiffB)} L:{(floorDiffL)}\nBL:{(floorDiffBL)} TR:{(floorDiffTR)}\nBR:{(floorDiffBR)} TL:{(floorDiffTL)}</mspace>";
        // //return $"<mspace=1.5em>L:{floorDiffT}\nB:{floorDiffR}</mspace>";  
            
        // var str1 = $"<mspace=1.5em>T:{BoolToString(neighborT)} R:{BoolToString(neighborR)}\nB:{BoolToString(neighborB)} L:{BoolToString(neighborL)}\nBL:{BoolToString(neighborBL)} TR:{BoolToString(neighborTR)}</mspace>";
        // var str2 = $"<mspace=1.5em>id:{tileId} T:{(floorDiffT)} R:{(floorDiffR)}\nB:{(floorDiffB)} L:{(floorDiffL)}\nBL:{(floorDiffBL)} TR:{(floorDiffTR)}</mspace>";
        // return $"{str1}\n{str2}";
    }
}