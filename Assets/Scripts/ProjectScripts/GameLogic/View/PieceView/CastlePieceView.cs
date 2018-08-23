using System.Collections.Generic;
using UnityEngine;

public class CastlePieceView : PieceBoardElementView
{   
    [SerializeField] private GameObject chest;
    [SerializeField] private Transform cap;
    
    [SerializeField] private GameObject shine;
    
    [SerializeField] private float open;
    [SerializeField] private float close;

    private List<BoardElementView> views = new List<BoardElementView>();
    private List<BoardPosition> lockedPositions = new List<BoardPosition>();
    private Piece king;
    
    private StorageComponent storage;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        storage.Timer.OnStart += OnStart;
        storage.Timer.OnComplete += Change;
        InitRiver();
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
    
    public BoardElementView CreateRiverElementByShift(BoardController board, BoardPosition targetPosition, int xShift, int yShift)
    {
        if (xShift < 0 && yShift > 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleTop, targetPosition); //return LeftTop;
        if (xShift < 0 && yShift == 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngle, targetPosition);
        if (xShift < 0 && yShift < 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleLeft, targetPosition);
        if (xShift == 0 && yShift < 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, targetPosition);
        if (xShift > 0 && yShift < 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleBottom, targetPosition);
        if (xShift > 0 && yShift == 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, targetPosition);
        if (xShift > 0 && yShift > 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleRight, targetPosition);
        if (xShift == 0 && yShift > 0)
            return board.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, targetPosition);
        return null;
    }

    private void SpawnRiverElement(BoardPosition maskPosition, MulticellularPieceBoardObserver observer,  int xShift, int yShift)
    {
        var targetPoint = observer.GetPointInMask(Piece.CachedPosition, maskPosition);
        var riverObject = CreateRiverElementByShift(Context.Context, targetPoint, xShift, yShift);
        if (riverObject == null)
            return;
        
        
        
        Context.Context.BoardLogic.LockCell(targetPoint, this);
        lockedPositions.Add(targetPoint);

        targetPoint.Z = -1;
        riverObject.SyncRendererLayers(targetPoint);
        
        views.Add(riverObject);
    }

    private BoardElementView CreateCustomElement(string resourceName, BoardPosition position, Vector3 shift)
    {
        var view = Context.Context.RendererContext.CreateBoardElementAt<BoardElementView>(resourceName, position);
        view.transform.localPosition += shift;
        
        views.Add(view);
        view.SyncRendererLayers(position);
        
        return view;
    }
    
    private void InitRiver()
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

        var kingPos = observer.GetPointInMask(Piece.CachedPosition, new BoardPosition(width / 2 - 1, -1));
        if (lockedPositions.Contains(kingPos))
        {
            lockedPositions.Remove(kingPos);
            Context.Context.BoardLogic.UnlockCell(kingPos, this);
        }

        var bridgeLeftPosition = new BoardPosition(width / 2 - 1, -1);
        var targetPosition = observer.GetPointInMask(Piece.CachedPosition, bridgeLeftPosition);
        targetPosition.Z = 0;
        CreateCustomElement(R.BrigeLeft, targetPosition, new Vector3(0.8f, -0.4f));
        
        targetPosition.Z = 0;
        targetPosition = observer.GetPointInMask(Piece.CachedPosition, new BoardPosition(width, height/2));
        CreateCustomElement(R.BrigeRight, targetPosition, new Vector3(-0.7f, -0.4f));

        var currentKingPos = GameDataService.Instance.Manager.PiecesManager.KingPosition;
        if (currentKingPos.Equals(BoardPosition.Default()))
        {
            Context.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
            {
                At = kingPos,
                IsCheckMatch = false,
#if UNITY_EDITOR
                OnFailedAction = (_) => Debug.LogError("[CastlePieceView] Fail on spawn king"),
#endif
                PieceTypeId = PieceType.King.Id
            });    
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
            Destroy(view.gameObject);
        }

        foreach (var lockPos in lockedPositions)
        {
            Context.Context.BoardLogic.UnlockCell(lockPos, this);
        }
        
        views = new List<BoardElementView>();
        lockedPositions = new List<BoardPosition>();
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
        if(storage.Timer.StartTime.GetTime().TotalSeconds < 3) return;
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