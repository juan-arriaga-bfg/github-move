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
            At = new BoardPosition(10, 13),
            PieceTypeId = PieceType.Sawmill1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(12, 17),
            PieceTypeId = PieceType.Castle1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 14),
            PieceTypeId = PieceType.Mine1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 17),
            PieceTypeId = PieceType.Tavern1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(14, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(15, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(16, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(17, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(19, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(20, 16),
            PieceTypeId = PieceType.O3.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(13, 13),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(11, 16),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 18),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(17, 17),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(18, 19),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(19, 19),
            PieceTypeId = PieceType.O2.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(15, 18),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(10, 10),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(12, 15),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(15, 12),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(17, 10),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(19, 13),
            PieceTypeId = PieceType.O1.Id
        });
        
        boardController.ActionExecutor.PerformAction(new CreatePieceAtAction
        {
            At = new BoardPosition(14, 11),
            PieceTypeId = PieceType.O2.Id
        });
        
        /*AddPieces(new BoardPosition(10, 10), PieceType.A1.Id, PieceType.A9.Id, boardController);
        AddPieces(new BoardPosition(12, 10), PieceType.C1.Id, PieceType.C9.Id, boardController);
        AddPieces(new BoardPosition(14, 10), PieceType.B1.Id, PieceType.B5.Id, boardController);
        AddPieces(new BoardPosition(16, 10), PieceType.D1.Id, PieceType.D5.Id, boardController);
        AddPieces(new BoardPosition(18, 10), PieceType.E1.Id, PieceType.E6.Id, boardController);*/
        
        //register board
        BoardService.Current.RegisterBoard(boardController, 0);
        AddStartResources();
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

    private void AddStartResources()
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", "strtCoins"), 
            ItemUid = Currency.Coins.Name, 
            Amount = 500,
            CurrentPrices = new List<Price>{new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}}
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
        
        var shopItem2 = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", "Crystals"), 
            ItemUid = Currency.Crystals.Name, 
            Amount = 50,
            CurrentPrices = new List<Price>{new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}}
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem2,
            (item, s) =>
            {
                // on purchase success
                
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
        
        var shopItem3 = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", "Crystals"), 
            ItemUid = Currency.RobinCard.Name, 
            Amount = 50,
            CurrentPrices = new List<Price>{new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}}
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem3,
            (item, s) =>
            {
                // on purchase success
                
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
    }

}