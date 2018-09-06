using UnityEngine;
using UnityEngine.UI;

public class EnemyLockView : UIBoardView
{
    [SerializeField] private Image Image;
    
    protected override ViewType Id => ViewType.Lock;

    public override void Init(Piece piece)
    {
        base.Init(piece);

        Priority = defaultPriority = 0;

        Change(true);
    }
}
