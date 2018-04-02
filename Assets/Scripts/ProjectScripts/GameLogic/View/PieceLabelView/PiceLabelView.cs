using UnityEngine;

public class PiceLabelView : UIBoardView
{
    [SerializeField] private NSText label;

    public override int Priority
    {
        get { return -1; }
    }

    public override Vector3 Ofset
    {
        get { return new Vector3(0, -0.3f); }
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

        if (def == null) return;

        label.Text = string.Format("{0} Level {1}", def.Uid.Substring(0, def.Uid.Length - 1), def.CurrentLevel());
        Change(true);
    }
}