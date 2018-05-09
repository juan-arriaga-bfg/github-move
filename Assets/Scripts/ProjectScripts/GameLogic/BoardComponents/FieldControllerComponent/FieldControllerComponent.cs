using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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

        if (fieldDef.Pieces == null)
        {
            StartField();
//            TestField();
            CreateFog();
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
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private void StartField()
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 4),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(27, 1),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 0),
            PieceTypeId = PieceType.King.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 4),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(29, 6),
            PieceTypeId = PieceType.O1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 3),
            PieceTypeId = PieceType.O1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 1),
            PieceTypeId = PieceType.O1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(26, 1),
            PieceTypeId = PieceType.O1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 6),
            PieceTypeId = PieceType.O1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 0),
            PieceTypeId = PieceType.O1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(26, 7),
            PieceTypeId = PieceType.A1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(27, 0),
            PieceTypeId = PieceType.A1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(23, 5),
            PieceTypeId = PieceType.A1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 2),
            PieceTypeId = PieceType.A1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 1),
            PieceTypeId = PieceType.A1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(23, 6),
            PieceTypeId = PieceType.A1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 4),
            PieceTypeId = PieceType.A1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 7),
            PieceTypeId = PieceType.A1.Id
        });

        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 6),
            PieceTypeId = PieceType.A1.Id
        });
    }

    private void TestField()
    {
        AddPieces(new BoardPosition(10, 10), PieceType.A1.Id, PieceType.A9.Id, context);
        AddPieces(new BoardPosition(12, 10), PieceType.B1.Id, PieceType.B5.Id, context);
        AddPieces(new BoardPosition(14, 10), PieceType.C1.Id, PieceType.C9.Id, context);
        AddPieces(new BoardPosition(16, 10), PieceType.D1.Id, PieceType.D5.Id, context);
        AddPieces(new BoardPosition(18, 10), PieceType.E1.Id, PieceType.E6.Id, context);
        AddPieces(new BoardPosition(20, 10), PieceType.O1.Id, PieceType.O5.Id, context);
        AddPieces(new BoardPosition(22, 10), PieceType.OX1.Id, PieceType.OX5.Id, context);
        
        AddPieces(new BoardPosition(24, 10), PieceType.Chest1.Id, PieceType.Chest6.Id, context);
        AddPieces(new BoardPosition(26, 10), PieceType.Coin1.Id, PieceType.Coin5.Id, context);
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
    
    private void AddPieces(BoardPosition position, int first, int last, BoardController board)
    {
        for (int i = first; i < last + 1; i++)
        {
            board.ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = position,
                PieceTypeId = i
            });
            
            position = position.Up;
        }
    }
}