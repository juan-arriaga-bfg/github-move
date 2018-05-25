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
        
//        CreateTown();

//        if (fieldDef.Pieces == null)
        {
            StartField();
            TestField();
//            CreateFog();
            return;
        }
        
        foreach (var item in fieldDef.Pieces)
        {
            context.ActionExecutor.AddAction(new FillBoardAction
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
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private void StartField()
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(19, 8),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(16, 8),
            PieceTypeId = PieceType.Market1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(16, 10),
            PieceTypeId = PieceType.Storage1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(26, 3),
            PieceTypeId = PieceType.Factory1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(14, 10),
            PieceTypeId = PieceType.Factory1.Id
        });
        
        //----------------------------------------
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 4),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 0),
            PieceTypeId = PieceType.King.Id
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.O1.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(24, 1),
                new BoardPosition(24, 3),
                new BoardPosition(24, 6),
                new BoardPosition(25, 0),
                new BoardPosition(26, 1),
                new BoardPosition(29, 6)
            }
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.A1.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(23, 5),
                new BoardPosition(23, 6),
                new BoardPosition(25, 1),
                new BoardPosition(25, 2),
                new BoardPosition(26, 6),
                new BoardPosition(27, 0),
                new BoardPosition(27, 5),
                new BoardPosition(28, 2),
                new BoardPosition(29, 1),
            }
        });
    }

    private void TestField()
    {
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.F4.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(6, 10),
                new BoardPosition(6, 11),
                new BoardPosition(6, 12),
                new BoardPosition(6, 13),
            }
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.G4.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(8, 10),
                new BoardPosition(8, 11),
                new BoardPosition(8, 12),
                new BoardPosition(8, 13),
            }
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.H4.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(10, 10),
                new BoardPosition(10, 11),
                new BoardPosition(10, 12),
                new BoardPosition(10, 13),
            }
        });
        
        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.I4.Id,
            Positions = new List<BoardPosition>
            {
                new BoardPosition(12, 10),
                new BoardPosition(12, 11),
                new BoardPosition(12, 12),
                new BoardPosition(12, 13),
            }
        });
        
        return;
        
        AddPieces(new BoardPosition(6, 10), PieceType.O1.Id, PieceType.O5.Id);
        AddPieces(new BoardPosition(8, 10), PieceType.OX1.Id, PieceType.OX5.Id);
        
        AddPieces(new BoardPosition(10, 10), PieceType.A1.Id, PieceType.A9.Id);
        AddPieces(new BoardPosition(12, 10), PieceType.B1.Id, PieceType.B5.Id);
        AddPieces(new BoardPosition(14, 10), PieceType.C1.Id, PieceType.C9.Id);
        AddPieces(new BoardPosition(16, 10), PieceType.D1.Id, PieceType.D5.Id);
        AddPieces(new BoardPosition(18, 10), PieceType.E1.Id, PieceType.E5.Id);
        AddPieces(new BoardPosition(20, 10), PieceType.F1.Id, PieceType.F4.Id);
        AddPieces(new BoardPosition(22, 10), PieceType.G1.Id, PieceType.G4.Id);
        AddPieces(new BoardPosition(24, 10), PieceType.H1.Id, PieceType.H4.Id);
        AddPieces(new BoardPosition(26, 10), PieceType.I1.Id, PieceType.I4.Id);
        
        AddPieces(new BoardPosition(28, 10), PieceType.Chest1.Id, PieceType.Chest6.Id);
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
        for (int i = 0; i < 22; i++)
        {
            context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverLeft, new BoardPosition(i, 7));
            context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverRight, new BoardPosition(22, 29-i));
            
            context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(i, 8));
            context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(29 - 8, 29 - i));
        }
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RiverAngle, new BoardPosition(22, 7));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(13, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(13, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(13, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(13, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(15, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(15, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(15, 11));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(15, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(15, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(17, 8));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(17, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(17, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(17, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(17, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 13));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 15));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(19, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 17));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(20, 18));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(10, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(11, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(12, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 9));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(11, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(12, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(13, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 10));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(14, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(15, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(21, 12));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(16, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(17, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(18, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(19, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(20, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(21, 14));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(18, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(19, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(20, 16));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(21, 16));
        
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.House, new BoardPosition(16, 12, 1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.House, new BoardPosition(18, 12, 1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.House, new BoardPosition(20, 12, 1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.House, new BoardPosition(18, 14, 1));
        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.House, new BoardPosition(20, 14, 1));

        AddBoardElement(R.BushLeft, new BoardPosition(14, 10, 2), new Vector3(0.5f, 0.3f));
        AddBoardElement(R.BushLeft, new BoardPosition(16, 10, 2), new Vector3(0.5f, 0.5f));
        AddBoardElement(R.BushRight, new BoardPosition(17, 13, 2), new Vector3(-0.8f, 0.4f));
        AddBoardElement(R.BushLeft, new BoardPosition(18, 8, 2), new Vector3(-0.1f, 0.5f));
        AddBoardElement(R.BushLeft, new BoardPosition(18, 14, 2), new Vector3(0.5f, 0.3f));
        
        AddBoardElement(R.Tree, new BoardPosition(11, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(13, 8, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(11, 9, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 9, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(13, 9, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(13, 10, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(15, 12, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(17, 14, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(19, 15, 2), new Vector3(0.3f, 0.2f));
        AddBoardElement(R.Tree, new BoardPosition(20, 16, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(21, 16, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(20, 17, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(21, 17, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(21, 13, 2), new Vector3(0.4f, 0.2f));
        
        AddBoardElement(R.BrigeLeft, new BoardPosition(19, 7, 1), new Vector3(0.6f, -0.25f));
        AddBoardElement(R.BrigeRight, new BoardPosition(22, 10, 1), new Vector3(-0.6f, -0.25f));
        
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