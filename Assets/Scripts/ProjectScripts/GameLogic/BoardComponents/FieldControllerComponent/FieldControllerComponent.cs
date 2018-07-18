﻿using System.Collections.Generic;
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
        var pieces = GameDataService.Current.FieldManager.Pieces;

        foreach (var piece in pieces)
        {
            context.ActionExecutor.AddAction(new FillBoardAction
            {
                Piece = piece.Key,
                Positions = piece.Value
            });
        }
    }

    private void TestField()
    {
        /*context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 8),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(21, 4),
            PieceTypeId = PieceType.MineX.Id
        });*/
        
        AddPieces(new BoardPosition(4, 10), PieceType.O1.Id, PieceType.O5.Id);
        AddPieces(new BoardPosition(6, 10), PieceType.OX1.Id, PieceType.OX5.Id);
        
        AddPieces(new BoardPosition(8, 10), PieceType.A1.Id, PieceType.A9.Id);
        AddPieces(new BoardPosition(10, 10), PieceType.B1.Id, PieceType.B5.Id);
        AddPieces(new BoardPosition(12, 10), PieceType.C1.Id, PieceType.C9.Id);
        AddPieces(new BoardPosition(14, 10), PieceType.D1.Id, PieceType.D4.Id);
        AddPieces(new BoardPosition(16, 10), PieceType.E1.Id, PieceType.E5.Id);
        AddPieces(new BoardPosition(18, 10), PieceType.F1.Id, PieceType.F5.Id);
        AddPieces(new BoardPosition(20, 10), PieceType.G1.Id, PieceType.G4.Id);
        AddPieces(new BoardPosition(22, 10), PieceType.H1.Id, PieceType.H4.Id);
        AddPieces(new BoardPosition(24, 10), PieceType.I1.Id, PieceType.I5.Id);
        AddPieces(new BoardPosition(26, 10), PieceType.J1.Id, PieceType.J5.Id);
        
        AddPieces(new BoardPosition(28, 10), PieceType.ChestA1.Id, PieceType.ChestA3.Id);
        AddPieces(new BoardPosition(28, 14), PieceType.ChestX1.Id, PieceType.ChestX3.Id);
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
        /*var positions = new List<BoardPosition>();

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
        */
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
        
//        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadRight, new BoardPosition(12, 7));
//        context.RendererContext.CreateBoardElementAt<BoardElementView>(R.RoadLeft, new BoardPosition(11, 7));
        
//        AddBoardElement(R.BushRight, new BoardPosition(16, 14, 2), new Vector3(-0.8f, 0.4f));
//        AddBoardElement(R.BushLeft, new BoardPosition(17, 11, 2), new Vector3(-0.1f, 0.5f));
        
        /*AddBoardElement(R.Tree, new BoardPosition(19, 7, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(20, 7, 2), new Vector3(-0.6f, -0.1f));
        AddBoardElement(R.Tree, new BoardPosition(12, 7, 2), new Vector3(0f, 0.1f));
        AddBoardElement(R.Tree, new BoardPosition(10, 8, 2), new Vector3(0f, 0.1f));*/
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