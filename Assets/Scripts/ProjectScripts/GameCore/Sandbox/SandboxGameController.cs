﻿using System;
using System.Collections.Generic;
using DG.Tweening.Plugins.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using UnityEngine;

public class SandboxGameController : MonoBehaviour
{
    /*protected virtual void OnDestroy()
    {
        List<IECSSystem> ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);

        for (int i = 0; i < ecsSystems.Count; i++)
        {
            if (ecsSystems[i].IsPersistence == false)
            {
                ECSService.Current.SystemProcessor.UnRegisterSystem(ecsSystems[i]);
            }
        }
    }*/
    
    
    public virtual void Run()
    {
        BoardController boardController = new BoardController();
        
        //register board
        BoardService.Current.RegisterBoard(boardController, 0);
        
        var gameBoardRendererView = new GameObject("_BoardRenderer");
        
        var gameBoardResourcesDef = new BoardResourcesDef
        {
            ElementsResourcesDef = new ElementsResourcesBuilder().Build()
        };
        
        boardController
            .RegisterComponent(new ActionExecuteComponent()
            .RegisterComponent(new ActionHistoryComponent())); // action loop
        
        boardController.RegisterComponent(new BoardEventsComponent()); // external event system
        boardController.RegisterComponent(new BoardLoggerComponent()); // logger
        
        boardController.RegisterComponent(new WorkerCurrencyLogicComponent());
        boardController.RegisterComponent(new EnergyCurrencyLogicComponent{Delay = 60});
        
        boardController
            .RegisterComponent(new BoardLogicComponent() // core logic
            .RegisterComponent(new PiecePositionsCacheComponent())
            .RegisterComponent(new FieldFinderComponent())
            .RegisterComponent(new EmptyCellsFinderComponent()) // finds empty cells
            .RegisterComponent(new MatchActionBuilderComponent() // creates match action
                .RegisterDefaultBuilder(new SimpleMatchActionBuilder()) // creates default match action
                .RegisterBuilder(new MulticellularPieceMatchActionBuilder())
                .RegisterBuilder(new СompositePieceMatchActionBuilder()))
            .RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()))); 
        
        boardController.RegisterComponent(new BoardRandomComponent()); // random
        boardController.RegisterComponent(new ReproductionLogicComponent());
        boardController.RegisterComponent(new HintCooldownComponent());
        boardController.RegisterComponent(new PartPiecesLogicComponent());
        boardController.RegisterComponent(new BoardRenderer().Init(gameBoardResourcesDef,
            gameBoardRendererView.transform)); // renderer context
        
        boardController
            .RegisterComponent(new BoardManipulatorComponent()
            .RegisterComponent(new LockerComponent())); // user manipualtor
        
        boardController.RegisterComponent(new BoardDefinitionComponent
        {
            CellWidth = 1,
            CellHeight = 1,
            UnitSize = 1.8f,
            GlobalPieceScale = 1f,
            ViewCamera = Camera.main,
            Width = 40,
            Height = 40,
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
            },
            GenereateIgnorable(boardController)
        );
        
        var widthShift = boardController.BoardDef.Width / 4;
        var heightShift = boardController.BoardDef.Height / 4;
        
        var leftPoint = boardController.BoardDef.GetSectorCenterWorldPosition(widthShift, widthShift, 0);
        var rightPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width - widthShift, boardController.BoardDef.Height - widthShift, 0);
        var topPoint = boardController.BoardDef.GetSectorCenterWorldPosition(heightShift, boardController.BoardDef.Height - heightShift, 0);
        var bottomPoint = boardController.BoardDef.GetSectorCenterWorldPosition(boardController.BoardDef.Width - heightShift, heightShift, 0);
        
        var centerPosition = boardController.BoardDef.GetSectorCenterWorldPosition(19, 14, boardController.BoardDef.PieceLayer);
        
        boardController.Manipulator.CameraManipulator.CurrentCameraSettings.CameraClampRegion = new Rect
        (
            leftPoint.x - boardController.BoardDef.UnitSize, 
            bottomPoint.y - boardController.BoardDef.UnitSize,
            Mathf.Abs((leftPoint - rightPoint).x) + boardController.BoardDef.UnitSize,
            Mathf.Abs((topPoint - bottomPoint).y) + boardController.BoardDef.UnitSize
        );
       
        boardController.Manipulator.CameraManipulator.CachedCameraTransform.localPosition = new Vector3
        (
            centerPosition.x,
            centerPosition.y,
            boardController.Manipulator.CameraManipulator.CachedCameraTransform.localPosition.z
        );

        var shift = 12;
        var vectorShift = (shift / 2) * boardController.BoardDef.UnitSize;
        boardController.RendererContext.GenerateBackground
        (
            new Vector3(-vectorShift, 0),
            boardController.BoardDef.Width + shift,
            boardController.BoardDef.Height + shift,
            boardController.BoardDef.UnitSize,
            "background_tile"
        );
        
        boardController.ActionExecutor.PerformAction(new CreateBoardAction());

        boardController.RegisterComponent(new FieldControllerComponent());
    }
    
    private List<BoardPosition> GenereateIgnorable(BoardController board)
    {
        var width = board.BoardDef.Width;
        var height = board.BoardDef.Height;
        var ignorable = new List<BoardPosition>();

        var maxEdge = Math.Max(width, height);
        
        var widthCount = maxEdge / 2;
        for (int i = 0; i < widthCount ; i++)
        {
            for (int j = 0; j < widthCount-i; j++)
            {
                ignorable.Add(new BoardPosition(i, j));
                ignorable.Add(new BoardPosition(width - 1 - i, height - 1 - j));
            }
        }

        var heightCount = maxEdge / 2;
        for (int i = 0; i < heightCount; i++)
        {
            for (int j = 0; j < heightCount-i; j++)
            {
                ignorable.Add(new BoardPosition(i, height - 1 - j));
                ignorable.Add(new BoardPosition(width - 1 - i, j));
            }
        }

        return ignorable;
    }
}