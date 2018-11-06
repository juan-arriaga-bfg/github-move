using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldControllerComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private BoardController context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
        
        var fieldDef = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        
        GenerateBorder();
        var maxEdge = Math.Max(context.BoardDef.Width, context.BoardDef.Height);
        CutTriangles(maxEdge / 2, Directions.All);
        
#if UNITY_EDITOR
//        TestFieldOleg(); return;
//        TestFieldAlex(); return;
//        TestFieldQA(); return;
#endif
        
        context.BoardLogic.PieceFlyer.Locker.Lock(context);
        
        if (fieldDef.Pieces == null)
        {
            var pieces = new Dictionary<int, List<BoardPosition>>(GameDataService.Current.FieldManager.Pieces)
                {
                    {PieceType.Fog.Id, CreateFog()}
                };

           
            
            
            foreach (var piece in pieces)
            {
                context.ActionExecutor.AddAction(new FillBoardAction
                {
                    Piece = piece.Key,
                    Positions = piece.Value
                });
            }

            //AddPiece(new BoardPosition(18,18), PieceType.Char1.Id);
            //AddPiece(new BoardPosition(19,3), PieceType.MineK.Id);
            
            AddLastAction();
            
            return;
        }
        
        fieldDef.Pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));
        
        foreach (var item in fieldDef.Pieces)
        {
            context.ActionExecutor.PerformAction(new FillBoardAction
            {
                Piece = item.Id,
                Positions = item.Positions
            });
        }

        AddLastAction();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private void AddLastAction()
    {
        context.ActionExecutor.AddAction(new CallbackAction
        {
            Callback = controller =>
            {
                controller.BoardLogic.PieceFlyer.Locker.Unlock(controller);
                
                var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

                if (views == null) return;
                
                foreach (var view in views)
                {
                    view.UpdateResource(0);
                }
                
                controller.AreaAccessController?.FullRecalculate();
                PathfindLockObserver.LoadPathfindLock();
                controller.PathfindLocker.OnAddComplete();
                //controller.PathfindLocker.RecalcAll(controller.AreaAccessController.AvailiablePositions);
            }
        });
    }

    private void TestFieldOleg()
    {
        /*AddPieces(new BoardPosition(24, 12), PieceType.O1.Id,  PieceType.O9.Id);
        AddPieces(new BoardPosition(25, 12), PieceType.OX1.Id, PieceType.OX9.Id);
        AddPieces(new BoardPosition(6, 16), PieceType.OEpic1.Id, PieceType.OEpic9.Id);
        
        AddPieces(new BoardPosition(11, 12), PieceType.C1.Id, PieceType.C11.Id, true);
        AddPieces(new BoardPosition(12, 12), PieceType.A1.Id, PieceType.A9.Id, true);
        AddPieces(new BoardPosition(13, 12), PieceType.K1.Id, PieceType.K10.Id, true);
        AddPieces(new BoardPosition(14, 12), PieceType.L1.Id, PieceType.L9.Id, true);
        AddPieces(new BoardPosition(15, 12), PieceType.D1.Id, PieceType.D5.Id);
        AddPieces(new BoardPosition(16, 12), PieceType.E1.Id, PieceType.E5.Id);
        AddPieces(new BoardPosition(18, 12), PieceType.F1.Id, PieceType.F5.Id);
        AddPieces(new BoardPosition(20, 12), PieceType.G1.Id, PieceType.G5.Id);
        AddPieces(new BoardPosition(22, 12), PieceType.H1.Id, PieceType.H5.Id);
        AddPieces(new BoardPosition(24, 12), PieceType.I1.Id, PieceType.I5.Id);
        AddPieces(new BoardPosition(25, 12), PieceType.J1.Id, PieceType.J5.Id);
        
        AddPieces(new BoardPosition(26, 12), PieceType.ChestEpic1.Id, PieceType.ChestEpic3.Id);
        AddPieces(new BoardPosition(27, 16), PieceType.ChestA1.Id, PieceType.ChestA3.Id);
        
        AddPieces(new BoardPosition(28, 10), PieceType.ChestK1.Id, PieceType.ChestK3.Id);
        AddPieces(new BoardPosition(28, 14), PieceType.ChestL1.Id, PieceType.ChestL3.Id);
        AddPieces(new BoardPosition(28, 18), PieceType.ChestC1.Id, PieceType.ChestC3.Id);*/
        
        AddPieces(new BoardPosition(29, 16), PieceType.NPC_SleepingBeauty.Id, PieceType.NPC_9.Id);
    }
    
    private void TestFieldAlex()
    {
        AddPieces(new BoardPosition(17, 16), PieceType.A1.Id, PieceType.A1.Id);
        AddPieces(new BoardPosition(19, 16), PieceType.B1.Id, PieceType.B11.Id);
        AddPieces(new BoardPosition(20, 16), PieceType.PR_A1.Id, PieceType.PR_A5.Id);
        AddPieces(new BoardPosition(21, 16), PieceType.NPC_SleepingBeauty.Id, PieceType.NPC_9.Id);
        AddPieces(new BoardPosition(23, 16), PieceType.Boost_CR1.Id, PieceType.Boost_CR.Id);
        AddPieces(new BoardPosition(24, 16), PieceType.Boost_CR1.Id, PieceType.Boost_CR.Id);
        AddPieces(new BoardPosition(25, 16), PieceType.Boost_CR.Id, PieceType.Boost_CR.Id);
        AddPieces(new BoardPosition(27, 16), PieceType.CH1_A.Id, PieceType.CH3_A.Id);
        AddPieces(new BoardPosition(28, 16), PieceType.OB1_A.Id, PieceType.OB9_A.Id);
        AddPieces(new BoardPosition(29, 16), PieceType.PR_E1.Id, PieceType.PR_E5.Id);
    }
    
    private void TestFieldQA()
    {
        AddPieces(new BoardPosition(19, 16), PieceType.B1.Id, PieceType.B11.Id);
        AddPiece(10, 20, PieceType.B1.Id);
    }
    
    private List<BoardPosition> CreateFog()
    {
        var data = GameDataService.Current.FogsManager.Fogs;
        var positions = new List<BoardPosition>();

        foreach (var fog in data)
        {
            var pos = fog.GetCenter();

            pos.Z = context.BoardDef.PieceLayer;
            positions.Add(pos);
        }

        return positions;
    }
    
    private void AddPieces(BoardPosition position, int first, int last, bool includeFake = false)
    {
        for (var i = first; i < last + 1; i++)
        {
            if (includeFake == false && PieceType.GetDefById(i).Filter.Has(PieceTypeFilter.Fake))
            {
                continue;
            }
            
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
                    context.BoardLogic.LockCell(new BoardPosition(i, j, context.BoardDef.PieceLayer), this);

                if ((directions & Directions.Right) == Directions.Right)
                    context.BoardLogic.LockCell(new BoardPosition(width - 1 - i, height - 1 - j, context.BoardDef.PieceLayer), this);
            
                if ((directions & Directions.Top) == Directions.Top)
                    context.BoardLogic.LockCell(new BoardPosition(i, height - 1 - j, context.BoardDef.PieceLayer), this);
            
                if ((directions & Directions.Bottom) == Directions.Bottom)
                    context.BoardLogic.LockCell(new BoardPosition(width - 1 - i, j, context.BoardDef.PieceLayer), this);
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
    
    private void AddPiece(BoardPosition position, int piece)
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = position,
            PieceTypeId = piece
        });
    }
    
    private void AddPiece(int x, int y, int piece)
    {
        context.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(x, y),
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

