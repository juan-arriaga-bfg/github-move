using System.Collections.Generic;
using UnityEngine;

public class FieldControllerComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid
    {
        get { return ComponentGuid; }
    }

    private BoardController context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
        
        var fieldDef = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);

#if UNITY_EDITOR
        CreateDebug();
#endif
        
        if (fieldDef.Pieces == null)
        {
            StartField();
//            TestField();
            CreateFog();
            CreateTown();
            return;
        }
        
        foreach (var item in fieldDef.Pieces)
        {
            context.ActionExecutor.PerformAction(new FillBoardAction
            {
                Piece = item.Id,
                Positions = item.Positions
            });
        }
        
        if(fieldDef.Resources == null) return;
        
        foreach (var item in fieldDef.Resources)
        {
            foreach (var pair in item.Resources)
            {
                GameDataService.Current.CollectionManager.CastResourceOnBoard(item.Position, pair);
            }
        }
        
        CreateTown();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private void StartField()
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 8),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(21, 4),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(17, 0),
            PieceTypeId = PieceType.King.Id
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.O1.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(17, 1),
                new BoardPosition(17, 3),
                new BoardPosition(17, 6),
                new BoardPosition(18, 0),
                new BoardPosition(19, 1),
                new BoardPosition(22, 6)
            }
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.A1.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(16, 5),
                new BoardPosition(16, 6),
                new BoardPosition(18, 1),
                new BoardPosition(18, 2),
                new BoardPosition(19, 6),
                new BoardPosition(20, 0),
                new BoardPosition(20, 5),
                new BoardPosition(21, 2),
                new BoardPosition(22, 1),
            }
        });
    }

    private void TestField()
    {
        AddPieces(new BoardPosition(4, 10), PieceType.O1.Id, PieceType.O5.Id);
        AddPieces(new BoardPosition(6, 10), PieceType.OX1.Id, PieceType.OX5.Id);
        
        AddPieces(new BoardPosition(8, 10), PieceType.A1.Id, PieceType.A9.Id);
        AddPieces(new BoardPosition(10, 10), PieceType.B1.Id, PieceType.B5.Id);
        AddPieces(new BoardPosition(12, 10), PieceType.C1.Id, PieceType.C9.Id);
        AddPieces(new BoardPosition(14, 10), PieceType.D1.Id, PieceType.D5.Id);
        AddPieces(new BoardPosition(16, 10), PieceType.E1.Id, PieceType.E5.Id);
        AddPieces(new BoardPosition(18, 10), PieceType.F1.Id, PieceType.F5.Id);
        AddPieces(new BoardPosition(20, 10), PieceType.G1.Id, PieceType.G4.Id);
        AddPieces(new BoardPosition(22, 10), PieceType.H1.Id, PieceType.H4.Id);
        AddPieces(new BoardPosition(24, 10), PieceType.I1.Id, PieceType.I5.Id);
        AddPieces(new BoardPosition(26, 10), PieceType.J1.Id, PieceType.J5.Id);
        
        AddPieces(new BoardPosition(28, 10), PieceType.ChestA1.Id, PieceType.ChestA3.Id);
        AddPieces(new BoardPosition(28, 14), PieceType.ChestB1.Id, PieceType.ChestB3.Id);
        AddPieces(new BoardPosition(28, 18), PieceType.ChestC1.Id, PieceType.ChestC3.Id);
        
        AddPieces(new BoardPosition(29, 10), PieceType.Coin1.Id, PieceType.Coin5.Id);
    }
    
    private void CreateFog()
    {
        var data = GameDataService.Current.FogsManager.Fogs;
        var positions = new List<BoardPosition>();

        foreach (var fog in data)
        {
            var pos = fog.Position;

            pos.Z = context.BoardDef.PieceLayer;
            positions.Add(pos);
        }

        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.Fog.Id,
            Positions = positions
        });
    }

    private void CreateTown()
    {
        var positions = new List<BoardPosition>();

        for (int i = 0; i < 23; i++)
        {
            for (int j = 7; j < context.BoardDef.Height; j++)
            {
                positions.Add(new BoardPosition(i, j, context.BoardDef.PieceLayer));
            }
        }
        
        context.ActionExecutor.AddAction(new LockCellAction
        {
            Locker = this,
            Positions = positions
        });
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleBottom, new BoardPosition(22, 7, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleLeft, new BoardPosition(17, 7, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleRight, new BoardPosition(22, 12, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngleTop, new BoardPosition(17, 12, -1));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(18, 12, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(19, 12, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(20, 12, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(21, 12, -1));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(18, 7, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(19, 7, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(20, 7, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(21, 7, -1));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(17, 8, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(17, 9, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(17, 10, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(17, 11, -1));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(22, 8, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(22, 9, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(22, 10, -1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(22, 11, -1));
        
        AddBoardElement(R.BrigeLeft, new BoardPosition(19, 7, 1), new Vector3(0.8f, -0.4f));
        AddBoardElement(R.BrigeRight, new BoardPosition(22, 10, 1), new Vector3(-0.7f, -0.4f));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 12));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(14, 14));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(16, 16));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(18, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(18, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(18, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(18, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(18, 17));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 18));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(11, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(12, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 7));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 7));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(12, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 9));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 11));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(18, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(19, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(20, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(21, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(22, 13));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(18, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(19, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(20, 15));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(18, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(19, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(20, 17));
        
        AddBoardElement(R.BushLeft, new BoardPosition(13, 11, 2), new Vector3(0.5f, 0.3f));
        AddBoardElement(R.BushLeft, new BoardPosition(15, 13, 2), new Vector3(0.5f, 0.5f));
        
        AddBoardElement(R.BushRight, new BoardPosition(16, 14, 2), new Vector3(-0.8f, 0.4f));
        AddBoardElement(R.BushLeft, new BoardPosition(17, 11, 2), new Vector3(-0.1f, 0.5f));
        AddBoardElement(R.BushLeft, new BoardPosition(17, 15, 2), new Vector3(0.5f, 0.3f));
        
        AddBoardElement(R.Tree, new BoardPosition(10, 7, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(11, 7, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 7, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(10, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(11, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 9, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 11, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(14, 13, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(18, 16, 2), new Vector3(0.3f, 0.2f));
        AddBoardElement(R.Tree, new BoardPosition(19, 17, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(20, 17, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(19, 18, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(20, 18, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(20, 14, 2), new Vector3(0.4f, 0.2f));
    }

    private void CreateDebug()
    {
        for (int i = 0; i < context.BoardDef.Width; i++)
        {
            for (int j = 0; j < context.BoardDef.Height; j++)
            {
                var cell = context.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(i, j, 20));
                cell.SetIndex(i, j);
            }
        }
    }
    
    private void AddPieces(BoardPosition position, int first, int last)
    {
        for (int i = first; i < last + 1; i++)
        {
            context.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = position,
                PieceTypeId = i
            });
            
            position = position.Up;
        }
    }

    private void AddBoardElement(string pattern, BoardPosition position, Vector3 offset)
    {
        var view = context.RendererContext.CreateBoardElementAt<BoardElementView>(pattern, position);

        view.CachedTransform.localPosition += offset;
    }
}