using System;

public class UIPiecesCheatSheetElementEntity : IUITabElementEntity 
{
    public string Uid { get; set; }
    public Action<UITabElementViewController> OnSelectEvent { get; set; }
    public Action<UITabElementViewController> OnDeselectEvent { get; set; }

    public int PieceId { get; set; }

    public override string ToString()
    {
        return string.Format("PieceId:{0}", PieceId);
    }
}
