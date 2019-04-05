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
        
        //GenerateBorder();

        LockWater();
        
        // var maxEdge = Math.Max(context.BoardDef.Width, context.BoardDef.Height);
        // CutTriangles(maxEdge / 2, Directions.All);
        
#if UNITY_EDITOR
//        TestFieldOleg(); return;
//        TestFieldAlex(); return;
//        TestFieldQA(); return;
#endif
        
        context.BoardLogic.PieceFlyer.Locker.Lock(context);
        context.Manipulator.CameraManipulator.CameraMove.Lock(context);
        
        if (fieldDef.Pieces == null)
        {
            CreateNewField();
        }
        else
        {
            LoadFieldFromSave(fieldDef);
        }

        AddLastAction();
    }

    private void LoadFieldFromSave(FieldDefComponent fieldDef)
    {
        fieldDef.Pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));

        foreach (var item in fieldDef.Pieces)
        {
            context.ActionExecutor.PerformAction(new FillBoardAction
            {
                Piece = item.Id,
                Positions = item.Positions
            });
        }

        var fogPos = CreateFog();

        context.ActionExecutor.AddAction(new FillBoardAction
        {
            Piece = PieceType.Fog.Id,
            Positions = fogPos
        });
    }

    private void CreateNewField()
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
                context.TutorialLogic.Update();
                controller.BoardLogic.PieceFlyer.Locker.Unlock(controller);
                
                controller.AreaAccessController?.FullRecalculate();

                var fogPositions = controller.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Fog.Id);
                foreach (var fog in fogPositions)
                {
                    var fogPiece = controller.BoardLogic.GetPieceAt(fog);
                    controller.PathfindLocker.RecalcCacheOnPieceAdded(controller.AreaAccessController.AvailiablePositions, fogPiece.CachedPosition, fogPiece);
                }
                
                controller.PathfindLocker.OnAddComplete(BoardPosition.GetRect(BoardPosition.Zero(), context.BoardDef.Width, context.BoardDef.Height));
                
                var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

                if (views == null)
                {
                    context.Manipulator.CameraManipulator.CameraMove.UnLock(context);
                    return;
                }
                
                foreach (var view in views)
                {
                    view.UpdateResource(0);
                }
                
                DevTools.UpdateFogSectorsDebug();
                context.Manipulator.CameraManipulator.CameraMove.UnLock(context);
            }
        });
    }

    private void TestFieldOleg()
    {
        AddPieces(new BoardPosition(24, 12), PieceType.OB1_TT.Id, PieceType.OB2_TT.Id);
        AddPieces(new BoardPosition(29, 16), PieceType.NPC_A.Id, PieceType.NPC_H.Id);
    }
    
    private void TestFieldAlex()
    {
        AddPieces(new BoardPosition(17, 16), PieceType.A1.Id, PieceType.A1.Id);
        AddPieces(new BoardPosition(19, 16), PieceType.B1.Id, PieceType.B11.Id);
        AddPieces(new BoardPosition(20, 16), PieceType.PR_A1.Id, PieceType.PR_A5.Id);
        AddPieces(new BoardPosition(21, 16), PieceType.NPC_A.Id, PieceType.NPC_H.Id);
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
        var fogManager = GameDataService.Current.FogsManager;
        var data = fogManager.Fogs;
        var positions = new List<BoardPosition>();

        foreach (var fog in data)
        {
            if (fogManager.IsFogCleared(fog.Uid))
            {
                continue;
            }
            
            var pos = fog.GetCenter();

            pos.Z = BoardLayer.Piece.Layer;
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
    
    private void LockWater()
    {
        var width = context.BoardDef.Width;
        var height = context.BoardDef.Height;
        var layout = GameDataService.Current.FieldManager.LayoutData;
        
        int layoutIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = layout[layoutIndex++];
                if (cell == 1)
                {
                    var point = new BoardPosition(x, y, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreateEmptyPiece());
                }
            }
        }
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
                {
                    var point = new BoardPosition(i, j, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreatePieceFromType(PieceType.Empty.Id));
                }

                if ((directions & Directions.Right) == Directions.Right)
                {
                    var point = new BoardPosition(width - 1 - i, height - 1 - j, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreatePieceFromType(PieceType.Empty.Id));
                }

                if ((directions & Directions.Top) == Directions.Top)
                {
                    var point = new BoardPosition(i, height - 1 - j, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreatePieceFromType(PieceType.Empty.Id));
                }

                if ((directions & Directions.Bottom) == Directions.Bottom)
                {
                    var point = new BoardPosition(width - 1 - i, j, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreatePieceFromType(PieceType.Empty.Id));
                }
            }    
        }
    }

    private void GenerateBorder()
    {
        
        var width = context.BoardDef.Width;
        var height = context.BoardDef.Height;

        var maxEdge = Math.Max(width, height);
        var minEdge = Math.Min(width, height);
        var cutSize = maxEdge / 2;
        
        var typeBottom = R.BorderBottom;
        var typeTop = R.BorderTop;
        var typeLeft = R.BorderLeft;
        var typeRight = R.BorderRight;

        var oddShift = (maxEdge) & 1;
        
        for (var currentPos = 0; currentPos < cutSize; currentPos++)
        {
            var cutDifference = cutSize - currentPos;
            
            var topPos = new BoardPosition(currentPos, height - 1 - cutDifference, BoardLayer.Default.Layer);
            var bottomPos = new BoardPosition(width - 1 - currentPos, cutDifference, BoardLayer.Default.Layer);
            var leftPos = new BoardPosition(currentPos, cutDifference, BoardLayer.Default.Layer);
            var rightPos = new BoardPosition(width - 1 - currentPos, height - 1 - cutDifference, BoardLayer.Default.Layer);
            
            
            
            if(topPos.X < minEdge / 2 && bottomPos.X < width - 1)
                context.RendererContext.CreateBoardElementAt<BoardElementView>(typeTop, topPos);
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

