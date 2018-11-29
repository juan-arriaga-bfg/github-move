using System;

public class UIPiecesCheatSheetElementEntity : IUIContainerElementEntity 
{
    public string Uid { get; set; }
    public Action<UIContainerElementViewController> OnSelectEvent { get; set; }
    public Action<UIContainerElementViewController> OnDeselectEvent { get; set; }

    public int PieceId { get; set; }

    public override string ToString()
    {
        return string.Format("PieceId:{0}", PieceId);
    }
}