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
                {PieceType.A2.Id, R.A2Piece},
                {PieceType.A3.Id, R.A3Piece},
                {PieceType.A4.Id, R.A4Piece},
                {PieceType.A5.Id, R.A5Piece},
                {PieceType.A6.Id, R.A6Piece},
                {PieceType.A7.Id, R.A7Piece},
                {PieceType.A8.Id, R.A8Piece},
                {PieceType.A9.Id, R.A9Piece},
                
                {PieceType.B1.Id, R.B1Piece},
                {PieceType.B2.Id, R.B2Piece},
                {PieceType.B3.Id, R.B3Piece},
                {PieceType.B4.Id, R.B4Piece},
                {PieceType.B5.Id, R.B5Piece},
                {PieceType.B6.Id, R.B6Piece},
                {PieceType.B7.Id, R.B7Piece},
                {PieceType.B8.Id, R.B8Piece},
                {PieceType.B9.Id, R.B9Piece},
            }
        };

        var pieceBuilderDef = new Dictionary<int, IPieceBuilder>
        {
            {PieceType.Generic.Id, new GenericPieceBuilder()},
            {PieceType.Empty.Id, new EmptyPieceBuilder()},
            
            {PieceType.A1.Id, new A1PieceBuilder()},
            {PieceType.A2.Id, new A1PieceBuilder()},
            {PieceType.A3.Id, new A1PieceBuilder()},
            {PieceType.A4.Id, new A1PieceBuilder()},
            {PieceType.A5.Id, new A1PieceBuilder()},
            {PieceType.A6.Id, new A1PieceBuilder()},
            {PieceType.A7.Id, new A1PieceBuilder()},
            {PieceType.A8.Id, new A1PieceBuilder()},
            {PieceType.A9.Id, new A1PieceBuilder()},
            
            {PieceType.B1.Id, new A1PieceBuilder()},
            {PieceType.B2.Id, new A1PieceBuilder()},
            {PieceType.B3.Id, new A1PieceBuilder()},
            {PieceType.B4.Id, new A1PieceBuilder()},
            {PieceType.B5.Id, new A1PieceBuilder()},
            {PieceType.B6.Id, new A1PieceBuilder()},
            {PieceType.B7.Id, new A1PieceBuilder()},
            {PieceType.B8.Id, new A1PieceBuilder()},
            {PieceType.B9.Id, new A1PieceBuilder()},
        };

        boardController.RegisterComponent(new ActionExecuteComponent()
            .RegisterComponent(new ActionHistoryComponent())); // action loop
        boardController.RegisterComponent(new BoardEventsComponent()); // external event system
        boardController.RegisterComponent(new BoardLoggerComponent()); // logger
        boardController.RegisterComponent(new BoardLogicComponent()
            .RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()))); // core logic
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