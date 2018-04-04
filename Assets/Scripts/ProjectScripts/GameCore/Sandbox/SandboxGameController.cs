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
                .RegisterBuilder(new MulticellularPieceMatchActionBuilder())) // creates match action for
            .RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()))); 
        
        boardController.RegisterComponent(new BoardRandomComponent()); // random
        boardController.RegisterComponent(new ObstaclesLogicComponent());
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
            Width = 28,
            Height = 28,
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

        var centerPosition = boardController.BoardDef.GetSectorCenterWorldPosition(10, 20, boardController.BoardDef.PieceLayer);

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
            At = new BoardPosition(8, 13),
            PieceTypeId = PieceType.Tavern1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(8, 17),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(12, 17),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new FillBoardAction{Piece = PieceType.O2.Id, Positions = new List<BoardPosition>
        {
            new BoardPosition(12, 10),
            new BoardPosition(12, 11),
            new BoardPosition(12, 12),
            new BoardPosition(12, 13),
            new BoardPosition(12, 14),
            new BoardPosition(12, 15),
            new BoardPosition(13, 10),
            new BoardPosition(13, 11),
            new BoardPosition(13, 12),
            new BoardPosition(13, 13),
            new BoardPosition(13, 14),
            new BoardPosition(13, 15),
            new BoardPosition(14, 10),
            new BoardPosition(14, 11),
            new BoardPosition(14, 12),
            new BoardPosition(14, 13),
            new BoardPosition(14, 14),
            new BoardPosition(14, 15),
            new BoardPosition(15, 10),
            new BoardPosition(15, 11),
            new BoardPosition(15, 12),
            new BoardPosition(15, 13),
            new BoardPosition(15, 14),
            new BoardPosition(15, 15),
        }});
        
        boardController.ActionExecutor.PerformAction(new FillBoardAction{Piece = PieceType.O1.Id, Positions = new List<BoardPosition>
        {
            new BoardPosition(16, 10),
            new BoardPosition(16, 11),
            new BoardPosition(16, 12),
            new BoardPosition(16, 13),
            new BoardPosition(16, 14),
            new BoardPosition(16, 15),
        }});
        
        /*boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(0, 0),
            PieceTypeId = PieceType.Fog.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(5, 0),
            PieceTypeId = PieceType.Fog.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(5, 5),
            PieceTypeId = PieceType.Fog.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(0, 5),
            PieceTypeId = PieceType.Fog.Id
        });*/
        
        /*boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(12, 14),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new FillBoardAction{Piece = PieceType.O3.Id, Positions = new List<BoardPosition>
        {
            new BoardPosition(10, 12),
            new BoardPosition(10, 13),
            new BoardPosition(10, 14),
            new BoardPosition(10, 15),
            new BoardPosition(10, 16),
            new BoardPosition(10, 17),
            new BoardPosition(11, 16),
            new BoardPosition(12, 16),
            new BoardPosition(13, 16),
            new BoardPosition(14, 16),
            new BoardPosition(15, 16),
        }});
        
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 11),
            PieceTypeId = PieceType.A2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 12),
            PieceTypeId = PieceType.A3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 13),
            PieceTypeId = PieceType.A4.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 14),
            PieceTypeId = PieceType.A5.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 15),
            PieceTypeId = PieceType.A6.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 16),
            PieceTypeId = PieceType.A7.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 17),
            PieceTypeId = PieceType.A8.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 10),
            PieceTypeId = PieceType.B1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 11),
            PieceTypeId = PieceType.B2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 12),
            PieceTypeId = PieceType.B3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 13),
            PieceTypeId = PieceType.B4.Id
        });*/
        
        //register board
        BoardService.Current.RegisterBoard(boardController, 0);
    }

}