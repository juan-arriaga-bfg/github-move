using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldControllerComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    private BoardController context;

    public bool IsCreateComplete;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;

        IsCreateComplete = false;
        
        var fieldDef = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        
        //GenerateBorder();

        LockCellsByLayout();
        
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
        var pieces = new Dictionary<int, List<BoardPosition>>(GameDataService.Current.FieldManager.BoardPieces)
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
                
                FogSectorsView.Rebuild(context.RendererContext);
                
                context.Manipulator.CameraManipulator.CameraMove.UnLock(context);
                controller.BoardLogic.VIPIslandLogic.Init();
                
                GameDataService.Current.FogsManager.UpdateUnlockedStates();
                
                IsCreateComplete = true;
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
        AddPieces(new BoardPosition(19, 16), PieceType.B1.Id, PieceType.BM.Id);
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
        AddPieces(new BoardPosition(19, 16), PieceType.B1.Id, PieceType.BM.Id);
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
    
    private void LockCellsByLayout()
    {
        var width = context.BoardDef.Width;
        var height = context.BoardDef.Height;
        var layout = GameDataService.Current.FieldManager.LayoutData;
        var tileDefs = BoardTiles.GetDefs();
        
        int layoutIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tileId = layout[layoutIndex++];
                var def = tileDefs[tileId];
                if (def.IsLock)
                {
                    var point = new BoardPosition(x, y, BoardLayer.Piece.Layer);
                    context.BoardLogic.AddPieceToBoard(point.X, point.Y, context.CreateEmptyPiece());
                }
            }
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

