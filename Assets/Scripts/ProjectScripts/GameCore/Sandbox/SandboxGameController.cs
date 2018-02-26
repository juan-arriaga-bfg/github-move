﻿using System.Collections.Generic;
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
                {PieceType.O1.Id, R.O1Piece},

                {PieceType.A1.Id, R.A1Piece},
                {PieceType.A2.Id, R.A2Piece},
                {PieceType.A3.Id, R.A3Piece},
                {PieceType.A4.Id, R.A4Piece},

                {PieceType.B1.Id, R.B1Piece},
                {PieceType.B2.Id, R.B2Piece},
                {PieceType.B3.Id, R.B3Piece},
                {PieceType.B4.Id, R.B4Piece},
            }
        };
        
        var pieceBuilderDef = new Dictionary<int, IPieceBuilder>
        {
            {PieceType.Generic.Id, new GenericPieceBuilder()},
            {PieceType.Empty.Id, new EmptyPieceBuilder()},
            
            {PieceType.O1.Id, new ObstaclePieceBuilder()},

            {PieceType.A1.Id, new SimplePieceBuilder()},
            {PieceType.A2.Id, new SimplePieceBuilder()},
            {PieceType.A3.Id, new SpawnPieceBuilder()},
            {PieceType.A4.Id, new SimplePieceBuilder()},

            {PieceType.B1.Id, new SimplePieceBuilder()},
            {PieceType.B2.Id, new SimplePieceBuilder()},
            {PieceType.B3.Id, new SimplePieceBuilder()},
            {PieceType.B4.Id, new SimplePieceBuilder()},
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
            UnitSize = 1f,
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
        
        boardController.ActionExecutor.PerformAction(new CreateBoardAction());
        
        var position = new BoardPosition(15, 15, boardController.BoardDef.PieceLayer);
        var positions = new List<BoardPosition>();

        for (int i = 8; i < 16; i++)
        {
            boardController.BoardLogic.EmptyCellsFinder.FindRingWithPointInCenter(position, positions, 1000, i);
        }
        
        boardController.ActionExecutor.PerformAction(new FillBoardAction{Piece = PieceType.O1.Id, Positions = positions});
        
//        boardController.ActionExecutor.PerformAction(new StartSessionBoardAction());
    }
}