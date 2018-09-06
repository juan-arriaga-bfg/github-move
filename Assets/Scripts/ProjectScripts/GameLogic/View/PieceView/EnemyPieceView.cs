using System.Collections.Generic;
using UnityEngine;

public class EnemyPieceView: PieceBoardElementView
{
    [SerializeField] private GameObject lockItem;
    private List<GameObject> locks;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        AreaLockComponent areaLock = piece.GetComponent<AreaLockComponent>(AreaLockComponent.ComponentGuid);
    }
}