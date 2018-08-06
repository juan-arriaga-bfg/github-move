using System;
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
        
        GenerateBorder();
        var maxEdge = Math.Max(context.BoardDef.Width, context.BoardDef.Height);
        CutTriangles(maxEdge / 2, Directions.All);
        
//        if (fieldDef.Pieces == null)
        {
//            StartField();
//            CreateFog();
            TestField();
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
        /*AddPieces(new BoardPosition(4, 10), PieceType.O1.Id, PieceType.O5.Id);
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
        AddPieces(new BoardPosition(26, 10), PieceType.J1.Id, PieceType.J5.Id);*/
        
        AddPieces(new BoardPosition(24, 10), PieceType.Zord1.Id, PieceType.Zord4.Id);
        AddPieces(new BoardPosition(26, 10), PieceType.Zord1.Id, PieceType.Zord4.Id);
        AddPieces(new BoardPosition(28, 10), PieceType.Zord1.Id, PieceType.Zord4.Id);
        /*AddPieces(new BoardPosition(28, 14), PieceType.ChestX1.Id, PieceType.ChestX3.Id);
        AddPieces(new BoardPosition(28, 18), PieceType.ChestC1.Id, PieceType.ChestC3.Id);
        AddPieces(new BoardPosition(28, 22), PieceType.Basket1.Id, PieceType.Basket3.Id);
        
        AddPieces(new BoardPosition(29, 10), PieceType.Coin1.Id, PieceType.Coin5.Id);*/
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
    
    private void CutTriangles(int count, Directions directions)
    {
        var width = context.BoardDef.Width;
        var height = context.BoardDef.Height;

        for (var i = 0; i < count; i++)
        {
            for (var j = 0; j < count - i; j++)
            {
                if ((directions & Directions.Left) == Directions.Left) 
                    BlockCell(new BoardPosition(i, j, context.BoardDef.PieceLayer));

                if ((directions & Directions.Right) == Directions.Right)
                    BlockCell(new BoardPosition(width - 1 - i, height - 1 - j, context.BoardDef.PieceLayer));
            
                if ((directions & Directions.Top) == Directions.Top)
                    BlockCell(new BoardPosition(i, height - 1 - j, context.BoardDef.PieceLayer));
            
                if ((directions & Directions.Bottom) == Directions.Bottom)
                    BlockCell(new BoardPosition(width - 1 - i, j, context.BoardDef.PieceLayer));
            }    
        }
        
    }

    private void GenerateBorder()
    {
        //TODO fix resource problem
        
        var width = context.BoardDef.Width;
        var height = context.BoardDef.Height;

        var maxEdge = Math.Max(width, height);
        var minEdge = Math.Min(width, height);
        var cutSize = maxEdge / 2;
        
        var typeBottom = cutSize % 2 == 0 ? R.BorderDark : R.BorderLight;
        var typeLeft = cutSize % 2 == 0 ? R.BorderLightLeft : R.BorderDarkLeft;
        var typeRight = cutSize % 2 == 0 ? R.BorderLightRight : R.BorderDarkRight;

        var oddShift = (maxEdge) & 1;
        
        for (var i = 0; i < cutSize; i++)
        {
            var j = cutSize - i;
            
            var bottomPos = new BoardPosition(width - 1 - i, j, -2);
            var leftPos = new BoardPosition(i, j, -2);
            var rightPos = new BoardPosition(width - 1 - i, height - 1 - j, -2);
            
            if(bottomPos.X > minEdge / 2 - 1 && bottomPos.X < width - 1)
                context.RendererContext.CreateBoardElementAt<BoardElementView>(typeBottom, bottomPos);
            if(leftPos.X < minEdge / 2 && leftPos.X > oddShift)
                context.RendererContext.CreateBoardElementAt<BoardElementView>(typeLeft, leftPos);
            if(rightPos.X > maxEdge/2 - 1 && rightPos.X < width - 1)
                context.RendererContext.CreateBoardElementAt<BoardElementView>(typeRight, rightPos);
        }
        
    }
    
    private void BlockCell(BoardPosition position)
    {   
        context.ActionExecutor.AddAction(new LockCellAction
        {
            Locker = this,
            Positions = new List<BoardPosition> {position}
        });
    }

    private void AddPiece(BoardPosition position, int piece)
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = position,
            PieceTypeId = piece
        });
    }
    
    [Flags]
    private enum Directions
    {
        Left = 0x01,
        Right = 0x02,
        Top = 0x04,
        Bottom = 0x08,
        LeftAndRight = Left | Right,
        TopAndBottom = Top | Bottom,
        All = LeftAndRight | TopAndBottom
    }
}

