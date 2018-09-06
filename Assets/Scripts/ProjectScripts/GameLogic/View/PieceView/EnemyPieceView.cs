using System.Collections.Generic;
using UnityEngine;

public class EnemyPieceView: PieceBoardElementView
{
    [SerializeField] private TouchRegion touchRegion;
    [SerializeField] private GameObject lockItem;
    [SerializeField] private GameObject touchItem;
    
    private List<GameObject> locks;
    private List<GameObject> views = new List<GameObject>();
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        AreaLockComponent areaLock = piece.GetComponent<AreaLockComponent>(AreaLockComponent.ComponentGuid);

        foreach (var position in areaLock.LockedCells)
        {
            var touch = Instantiate(touchItem, transform.parent);
            touchRegion.AddTouchRegion(touch.GetComponent<RectTransform>());
            
            var lockView = Instantiate(lockItem, transform.parent);
            // var sprite = fog.GetComponent<Spr			
            // sprite.sortingOrder = position.X * Context.Context.BoardDef.Width - position.Y + 101;
			
            var pos = piece.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
            lockView.transform.position = pos;
            touch.transform.position = pos;
			
            views.Add(lockView);
        }
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        foreach (var view in views)
        {
            Destroy(view);
        }
		
        views = new List<GameObject>();
		
        touchRegion.ClearTouchRegion();
    }
}