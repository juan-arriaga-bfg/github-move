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
            ElementsResourcesDef = new ElementsResourcesBuilder().Build()
        };
        
        boardController.RegisterComponent(new ActionExecuteComponent()
            .RegisterComponent(new ActionHistoryComponent())); // action loop
        boardController.RegisterComponent(new BoardEventsComponent()); // external event system
        boardController.RegisterComponent(new BoardLoggerComponent()); // logger
        
        boardController.RegisterComponent(new BoardLogicComponent() // core logic
            .RegisterComponent(new PiecePositionsCacheComponent())
            .RegisterComponent(new FieldFinderComponent())
            .RegisterComponent(new EmptyCellsFinderComponent()) // finds empty cells
            .RegisterComponent(new MatchActionBuilderComponent() // creates match action
                .RegisterDefaultBuilder(new DefaultMatchActionBuilder()) // creates default match action
                .RegisterBuilder(new MulticellularPieceMatchActionBuilder())) // creates match action for Multicellular
            .RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()))); 
        
        boardController.RegisterComponent(new BoardRandomComponent()); // random
        boardController.RegisterComponent(new ObstaclesLogicComponent());
        boardController.RegisterComponent(new EnemiesLogicComponent());
        boardController.RegisterComponent(new ReproductionLogicComponent());
        boardController.RegisterComponent(new HintCooldownComponent());
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
        
        boardController.Init(new PieceBuildersBuilder().Build());
        
        boardController.BoardDef.SectorsGridView = boardController.RendererContext.GenerateField
        (
            boardController.BoardDef.Width,
            boardController.BoardDef.Height,
            boardController.BoardDef.UnitSize,
            new List<string>
            {
                "tile_grass_1",
                "tile_grass_2"
            });

        var leftPoint = boardController.BoardDef.GetSectorCenterWorldPosition(0, 0, 0);
        var rightPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width, boardController.BoardDef.Height, 0);
        var topPoint = boardController.BoardDef.GetSectorCenterWorldPosition(0, boardController.BoardDef.Height, 0);
        var bottomPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width, 0, 0);

        var centerPosition = boardController.BoardDef.GetSectorCenterWorldPosition(25, 5, boardController.BoardDef.PieceLayer);

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
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 2),
            PieceTypeId = PieceType.King.Id
        });
        
        boardController.ActionExecutor.AddAction(new CreatePieceAtAction
        {
            At = new BoardPosition(27, 1),
            PieceTypeId = PieceType.Castle1.Id
        });
		boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 4),
            PieceTypeId = PieceType.Sawmill1.Id
        });
		
		boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(29, 6),
            PieceTypeId = PieceType.O1.Id
        });
		
		boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 3),
            PieceTypeId = PieceType.O1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 1),
            PieceTypeId = PieceType.O1.Id
        });
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(26, 1),
            PieceTypeId = PieceType.O1.Id
        });
		        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 6),
            PieceTypeId = PieceType.O1.Id
        });
		        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 0),
            PieceTypeId = PieceType.O1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(26, 7),
            PieceTypeId = PieceType.A1.Id
        });
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(27, 0),
            PieceTypeId = PieceType.A1.Id
        });
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(23, 5),
            PieceTypeId = PieceType.A1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 2),
            PieceTypeId = PieceType.A1.Id
        });
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 1),
            PieceTypeId = PieceType.A1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(23, 6),
            PieceTypeId = PieceType.A1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(24, 4),
            PieceTypeId = PieceType.A1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(28, 7),
            PieceTypeId = PieceType.A1.Id
        });

        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(25, 6),
            PieceTypeId = PieceType.A1.Id
        });  

        /*AddPieces(new BoardPosition(19, 10), PieceType.Chest1.Id, PieceType.Chest6.Id, boardController);
        AddPieces(new BoardPosition(21, 10), PieceType.Coin1.Id, PieceType.Coin5.Id, boardController);*/
        
        //register board
        BoardService.Current.RegisterBoard(boardController, 0);
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