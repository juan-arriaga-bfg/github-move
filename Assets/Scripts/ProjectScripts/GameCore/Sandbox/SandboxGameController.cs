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
                {PieceType.A1.Id, R.A1Piece},

            }
        };

        var pieceBuilderDef = new Dictionary<int, IPieceBuilder>
        {
            {PieceType.Generic.Id, new GenericPieceBuilder()},
            {PieceType.Empty.Id, new EmptyPieceBuilder()},
            {PieceType.A1.Id, new A1PieceBuilder()},
        };

        boardController.RegisterComponent(new ActionExecuteComponent()
            .RegisterComponent(new ActionHistoryComponent())); // action loop
        boardController.RegisterComponent(new BoardEventsComponent()); // external event system
        boardController.RegisterComponent(new BoardLoggerComponent()); // logger
        boardController.RegisterComponent(new BoardLogicComponent()); // core logic
        boardController.RegisterComponent(new BoardRandomComponent()); // random
        boardController.RegisterComponent(new BoardRenderer().Init(gameBoardResourcesDef, gameBoardRendererView.transform)); // renderer context
        boardController.RegisterComponent(new BoardManipulatorComponent()
            .RegisterComponent(new LockerComponent())); // user manipualtor
        boardController.RegisterComponent(new BoardDefinitionComponent
        {
            CellWidth = 1,
            CellHeight = 1,
            UnitSize = 1f,
            GlobalPieceScale = 1f,
            ViewCamera = Camera.main,
            Width = 10,
            Height = 10,
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

        boardController.ActionExecutor.PerformAction(new CreateBoardAction());
        
        boardController.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(9,9,1),
            PieceTypeId = PieceType.A1.Id
        });
        
        boardController.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(0,0,1),
            PieceTypeId = PieceType.Generic.Id
        });
        
        boardController.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(1,0,1),
            PieceTypeId = PieceType.Generic.Id
        });
        
        boardController.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(9,0,1),
            PieceTypeId = PieceType.Generic.Id
        });
        
        boardController.ActionExecutor.AddAction(new SpawnPieceAtAction
        {
            At = new BoardPosition(0,9,1),
            PieceTypeId = PieceType.Generic.Id
        });

//        boardController.ActionExecutor.PerformAction(new FillBoardAction {LevelConfig = level});
//        boardController.ActionExecutor.PerformAction(new StartSessionBoardAction());
    }
}