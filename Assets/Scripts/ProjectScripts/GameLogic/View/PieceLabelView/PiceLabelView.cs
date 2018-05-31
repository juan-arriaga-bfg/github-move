using UnityEngine;

public class PiceLabelView : UIBoardView
{
    [SerializeField] private NSText label;
    
    protected override ViewType Id
    {
        get { return ViewType.LevelLabel; }
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);

        Priority = defaultPriority = -1;

        var uid = PieceType.Parse(piece.PieceType);
        
        label.Text = string.Format("{0} Level {1}", uid.Substring(0, uid.Length - 1), uid.Substring(uid.Length - 1, 1));
        Change(true);
    }
}