using System.Collections.Generic;
using UnityEngine;

public class SandboxGameController : MonoBehaviour
{
    protected virtual void OnDestroy()
    {
        List<IECSSystem> ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);

        for (int i = 0; i < ecsSystems.Count; i++)
        {
            if (ecsSystems[i].IsPersistence == false)
            {
                ECSService.Current.SystemProcessor.UnRegisterSystem(ecsSystems[i]);
            }
        }
    }
    
    public virtual void Run()
    {
        BoardController boardController = new BoardController();
        
        var gameBoardRendererView = new GameObject("_BoardRenderer");
        
        var gameBoardResourcesDef = new BoardResourcesDef
        {
            ElementsResourcesDef = new Dictionary<int, string>
            {
                {PieceType.Generic.Id, R.GenericPiece},
                
                {PieceType.M1.Id, R.M1Piece},
                {PieceType.S1.Id, R.S1Piece},
                
                {PieceType.E1.Id, R.E1Piece},
                {PieceType.E2.Id, R.E2Piece},
                {PieceType.E3.Id, R.E3Piece},
                
                {PieceType.O1.Id, R.O1Piece},
                {PieceType.C1.Id, R.C1Piece},
                
                {PieceType.A1.Id, R.A1Piece},
                {PieceType.A2.Id, R.A2Piece},
                {PieceType.A3.Id, R.A3Piece},
                {PieceType.A4.Id, R.A4Piece},
                {PieceType.A5.Id, R.A5Piece},
                {PieceType.A6.Id, R.A6Piece},
                
                {PieceType.B1.Id, R.B1Piece},
                {PieceType.B2.Id, R.B2Piece},
                {PieceType.B3.Id, R.B3Piece},
                {PieceType.B4.Id, R.B4Piece},
                {PieceType.B5.Id, R.B5Piece},
            }
        };
        
        var pieceBuilderDef = new Dictionary<int, IPieceBuilder>
        {
            {PieceType.Generic.Id, new GenericPieceBuilder()},
            {PieceType.Empty.Id, new EmptyPieceBuilder()},
            
            {PieceType.M1.Id, new MulticellularSpawnPieceBuilder
                {
                    Mask = new List<BoardPosition>
                    {
                        BoardPosition.Zero().Up,
                    }
                }
            },
            
            {PieceType.S1.Id, new MulticellularSpawnPieceBuilder
                {
                    Mask = new List<BoardPosition>
                    {
                        BoardPosition.Zero().Right,
                    }
                }
            },
            
            {PieceType.E1.Id, new EnemyPieceBuilder()},
            {PieceType.E2.Id, new EnemyPieceBuilder()},
            {PieceType.E3.Id, new EnemyPieceBuilder()},
            
            {PieceType.O1.Id, new GenericPieceBuilder()},
            {PieceType.C1.Id, new ResourcePieceBuilder()},
            
            {PieceType.A1.Id, new SimplePieceBuilder()},
            {PieceType.A2.Id, new SimplePieceBuilder()},
            {PieceType.A3.Id, new SpawnPieceBuilder()},
            {PieceType.A4.Id, new SpawnPieceBuilder()},
            {PieceType.A5.Id, new SpawnPieceBuilder()},
            {PieceType.A6.Id, new SpawnPieceBuilder()},
            
            {PieceType.B1.Id, new SimplePieceBuilder()},
            {PieceType.B2.Id, new SimplePieceBuilder()},
            {PieceType.B3.Id, new SpawnPieceBuilder()},
            {PieceType.B4.Id, new SpawnPieceBuilder()},
            {PieceType.B5.Id, new SpawnPieceBuilder()},
        };
        
        boardController.RegisterComponent(new ActionExecuteComponent()
            .RegisterComponent(new ActionHistoryComponent())); // action loop
        boardController.RegisterComponent(new BoardEventsComponent()); // external event system
        boardController.RegisterComponent(new BoardLoggerComponent()); // logger
        
        boardController.RegisterComponent(new BoardLogicComponent() // core logic
            .RegisterComponent(new FieldFinderComponent())
            .RegisterComponent(new EmptyCellsFinderComponent()) // finds empty cells
            .RegisterComponent(new MatchActionBuilderComponent() // creates match action
                .RegisterDefaultBuilder(new DefaultMatchActionBuilder())) // creates default match action
            .RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()))); 
        
        boardController.RegisterComponent(new BoardRandomComponent()); // random
        boardController.RegisterComponent(new BoardRenderer().Init(gameBoardResourcesDef,
            gameBoardRendererView.transform)); // renderer context
        boardController.RegisterComponent(new BoardManipulatorComponent()
            .RegisterComponent(new LockerComponent())); // user manipualtor
        boardController.RegisterComponent(new BoardDefinitionComponent
        {
            CellWidth = 1,
            CellHeight = 1,
            UnitSize = 1.8f,
            GlobalPieceScale = 1f,
            ViewCamera = Camera.main,
            Width = 30,
            Height = 30,
            Depth = 3,
            PieceLayer = 1
        }); // board settings
        
        boardController.RegisterComponent(new BoardStatesComponent()
            .RegisterState(new SessionBoardStateComponent(SessionBoardStateType.Processing))
        ); // states
        
        boardController.States.AddState(SessionBoardStateComponent.ComponentGuid);
        
        boardController.Init(pieceBuilderDef);
        
        boardController.BoardDef.SectorsGridView = boardController.RendererContext.GenerateField
        (
            boardController.BoardDef.Width,
            boardController.BoardDef.Height,
            boardController.BoardDef.UnitSize,
            new List<string>
            {
                "tile_grass_1",
                "tile_grass_2",
                "tile_grass_3"
            });

        var leftPoint = boardController.BoardDef.GetSectorCenterWorldPosition(0, 0, 0);
        var rightPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width, boardController.BoardDef.Height, 0);
        var topPoint = boardController.BoardDef.GetSectorCenterWorldPosition(0, boardController.BoardDef.Height, 0);
        var bottomPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width, 0, 0);

        var centerPosition = boardController.BoardDef.GetSectorCenterWorldPosition
            (
                (int)(boardController.BoardDef.Width * 0.5f), 
                (int)(boardController.BoardDef.Height * 0.5f), 
                0
            );

        boardController.Manipulator.CameraManipulator.CurrentCameraSettings.CameraClampRegion = new Rect
        (
            leftPoint.x - boardController.BoardDef.UnitSize, 
            -(topPoint - bottomPoint).magnitude * 0.5f,
            (leftPoint - rightPoint).magnitude + boardController.BoardDef.UnitSize,
            (topPoint - bottomPoint).magnitude + boardController.BoardDef.UnitSize * 2f
        );
       
        boardController.Manipulator.CameraManipulator.CachedCameraTransform.localPosition = new Vector3
        (
            centerPosition.x,
            centerPosition.y,
            boardController.Manipulator.CameraManipulator.CachedCameraTransform.localPosition.z
        );
        
        
        boardController.ActionExecutor.PerformAction(new CreateBoardAction());
        
        var position = new BoardPosition(15, 15, boardController.BoardDef.PieceLayer);
        var positions = new List<BoardPosition>();

        for (int i = 8; i < 16; i++)
        {
            boardController.BoardLogic.EmptyCellsFinder.FindRingWithPointInCenter(position, positions, 1000, i);
        }
        
        boardController.ActionExecutor.PerformAction(new FillBoardAction{Piece = PieceType.O1.Id, Positions = positions});
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(14, 14),
            PieceTypeId = PieceType.S1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(10, 14),
            PieceTypeId = PieceType.M1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(16, 14),
            PieceTypeId = PieceType.A1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(17, 15),
            PieceTypeId = PieceType.A2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(18, 16),
            PieceTypeId = PieceType.A3.Id
        });
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(19, 17),
            PieceTypeId = PieceType.A4.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(20, 18),
            PieceTypeId = PieceType.A5.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(21, 19),
            PieceTypeId = PieceType.A6.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(16, 18),
            PieceTypeId = PieceType.B1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(17, 19),
            PieceTypeId = PieceType.B2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(18, 20),
            PieceTypeId = PieceType.B3.Id
        });
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(19, 21),
            PieceTypeId = PieceType.B4.Id
        });
        boardController.ActionExecutor.PerformAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(20, 22),
            PieceTypeId = PieceType.B5.Id
        });
//        boardController.ActionExecutor.PerformAction(new StartSessionBoardAction());
        
        //register board
        BoardService.Current.RegisterBoard(boardController, 0);


    }

}