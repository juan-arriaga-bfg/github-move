using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class RiverData
{
    public GameObject LeftTop;
    public GameObject Left;
    public GameObject LeftBottom;
    public GameObject Bottom;
    public GameObject RightBottom;
    public GameObject Right;
    public GameObject RightTop;
    public GameObject Top;

    public GameObject BridgeLeft;
    public GameObject BridgeRight;
    
    public GameObject GetRiverPrefabByShift(int xShift, int yShift)
    {
        if (xShift < 0 && yShift > 0)
            return LeftTop;
        if (xShift < 0 && yShift == 0)
            return Left;
        if (xShift < 0 && yShift < 0)
            return LeftBottom;
        if (xShift == 0 && yShift < 0)
            return Bottom;
        if (xShift > 0 && yShift < 0)
            return RightBottom;
        if (xShift > 0 && yShift == 0)
            return Right;
        if (xShift > 0 && yShift > 0)
            return RightTop;
        if (xShift == 0 && yShift > 0)
            return Top;

        //only [0;0]
        return null;
    }
}

public class CastlePieceView : PieceBoardElementView
{   
    [SerializeField] private GameObject chest;
    [SerializeField] private Transform cap;
    
    [SerializeField] private GameObject shine;
    
    [SerializeField] private float open;
    [SerializeField] private float close;

    [SerializeField] private RiverData riverData;    

    private List<GameObject> views = new List<GameObject>();
    
    private StorageComponent storage;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        storage.Timer.OnStart += OnStart;
        storage.Timer.OnComplete += Change;
        InitRiver(riverData);
        Change();
    }

    private void SpawnRiverElements(BoardPosition maskPosition, MulticellularPieceBoardObserver observer, int width, int height)
    {
        var xShift = 0;
        if (maskPosition.X == 0)
            xShift = -1;      
        else if (maskPosition.X == width - 1)
            xShift = 1;

        var yShift = 0;
        if (maskPosition.Y == 0)
            yShift = -1;
        else if (maskPosition.Y == height - 1)
            yShift = 1;

        maskPosition.X += xShift;
        maskPosition.Y += yShift;

        if (yShift != 0 && xShift != 0)
        {
            SpawnRiverElement(new BoardPosition(maskPosition.X, maskPosition.Y - yShift), observer, xShift, 0);
            SpawnRiverElement(new BoardPosition(maskPosition.X - xShift, maskPosition.Y), observer, 0, yShift);
        }
        
        SpawnRiverElement(maskPosition, observer, xShift, yShift);
    }

    private void SpawnRiverElement(BoardPosition maskPosition, MulticellularPieceBoardObserver observer,  int xShift, int yShift)
    {
        var targetPoint = observer.GetPointInMask(Piece.CachedPosition, maskPosition);

        var riverPrefab = riverData.GetRiverPrefabByShift(xShift, yShift);
        if (riverPrefab == null)
            return;
        
        var riverInstanse = Instantiate(riverPrefab, transform);
        var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(targetPoint.X, targetPoint.Y, 0);
        riverInstanse.transform.position = position;
        views.Add(riverInstanse);
    }
    
    private void InitRiver(RiverData river)
    {
        var observer = Piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
        var mask = observer.Mask;
        var maxMaskPoint = mask[mask.Count - 1];
        var width = maxMaskPoint.X + 1;
        var height = maxMaskPoint.Y + 1;
        for (int i = 0; i < mask.Count; i++)
        {
            var targetMaskPoint = mask[i];
            SpawnRiverElements(targetMaskPoint, observer, width, height);  
        }

        if (riverData.BridgeLeft != null)
        {
            var bridgePrefab = riverData.BridgeLeft;
            var riverInstanse = Instantiate(bridgePrefab, transform);
            var targetPoint = observer.GetPointInMask(Piece.CachedPosition, new BoardPosition(width / 2, -1));
            var shift = new Vector3(0.8f, -0.4f);
            var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(targetPoint.X, targetPoint.Y, 0) + shift;
            riverInstanse.transform.position = position;

            //var sprite = riverInstanse.GetComponent<BoardElementView>();
            //sprite.ClearCacheLayers();
                //sprite.CacheLayers();//= targetPoint.X * Context.Context.BoardDef.Width - targetPoint.Y + targetPoint.Z * 100 + targetPoint.X  - targetPoint.Y;
            
            views.Add(riverInstanse);
        }

        if (riverData.BridgeRight != null)
        {
            var bridgePrefab = riverData.BridgeRight;
            var riverInstanse = Instantiate(bridgePrefab, transform);
            var targetPoint = observer.GetPointInMask(Piece.CachedPosition, new BoardPosition(width, height / 2));
            var shift = new Vector3(-0.7f, -0.4f);
            var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(targetPoint.X, targetPoint.Y, 0) + shift;
            riverInstanse.transform.position = position;
            
            //var sprite = riverInstanse.GetComponent<SpriteRenderer>();
            //sprite.sortingOrder = targetPoint.X * Context.Context.BoardDef.Width - targetPoint.Y + targetPoint.Z * 100 + targetPoint.X  - targetPoint.Y;
            
            views.Add(riverInstanse);
        }
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        if (storage == null) return;
        
        storage.Timer.OnStart -= OnStart;
        storage.Timer.OnComplete -= Change;
        storage.Timer.OnExecute -= OnExecute;

        foreach (var view in views)
        {
            Destroy(view);
        }
        views = new List<GameObject>();
    }

    private void OnStart()
    {
        Change();
        
        if (storage.Filling > 0) return;
        
        chest.SetActive(false);
        storage.Timer.OnExecute += OnExecute;
    }

    private void OnExecute()
    {
        if(storage.Timer.GetTime().TotalSeconds < 3) return;
        storage.Timer.OnExecute -= OnExecute;
        Change();
    }

    private void Change()
    {
        var isOpen = storage.Filling > 0;
        
        shine.SetActive(isOpen);
        cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? open : close);
        chest.SetActive(true);
    }

    public Vector3 GetSpawnPosition()
    {
        return chest.transform.position;
    }
}