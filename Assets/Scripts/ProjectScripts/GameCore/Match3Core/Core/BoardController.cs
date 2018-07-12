using System.Collections.Generic;
using UnityEngine;

public class BoardController : ECSEntity,
    IActionExecuteComponent, IBoardEventsComponent, IBoardLoggerComponent, IBoardLogicComponent, IBoardDefinitionComponent, IBoardStatesComponent, ISessionBoardStateComponent,
    IBoardSystemProcessor, IBoardRendererComponent, IBoardManipulatorComponent, IBoardRandomComponent, IReproductionLogicComponent, IEnemiesLogicComponent,
    IProductionLogicComponent, IWorkerCurrencyLogicComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    public IECSSystemProcessor SystemProcessor
    {
        get { return ECSService.Current.SystemProcessor; }
    }

    // components

    protected ActionExecuteComponent actionExecutor;
    public virtual ActionExecuteComponent ActionExecutor
    {
        get
        {
            if (actionExecutor == null)
            {
                actionExecutor = GetComponent<ActionExecuteComponent>(ActionExecuteComponent.ComponentGuid);
            }
            return actionExecutor;
        }
    }

    protected BoardEventsComponent boardEvents;
    public virtual BoardEventsComponent BoardEvents
    {
        get
        {
            if (boardEvents == null)
            {
                boardEvents = GetComponent<BoardEventsComponent>(BoardEventsComponent.ComponentGuid);
            }
            return boardEvents;
        }
    }

    protected BoardLoggerComponent logger;
    public virtual BoardLoggerComponent Logger
    {
        get
        {
            if (logger == null)
            {
                logger = GetComponent<BoardLoggerComponent>(BoardLoggerComponent.ComponentGuid);
            }
            return logger;
        }
    }

    protected BoardLogicComponent boardLogic;
    public virtual BoardLogicComponent BoardLogic
    {
        get
        {
            if (boardLogic == null)
            {
                boardLogic = GetComponent<BoardLogicComponent>(BoardLogicComponent.ComponentGuid);
            }
            return boardLogic;
        }
    }

    protected BoardDefinitionComponent boardDef;

    public virtual BoardDefinitionComponent BoardDef
    {
        get
        {
            if (boardDef == null)
            {
                boardDef = GetComponent<BoardDefinitionComponent>(BoardDefinitionComponent.ComponentGuid);
            }
            return boardDef;
        }
    }

    protected BoardStatesComponent states;
    public virtual BoardStatesComponent States
    {
        get
        {
            if (states == null)
            {
                states = GetComponent<BoardStatesComponent>(BoardStatesComponent.ComponentGuid);
            }
            return states;
        }
    }
    
    protected SessionBoardStateComponent session;
    public SessionBoardStateComponent Session
    {
        get
        {
            if (session == null)
            {
                session = States.GetState<SessionBoardStateComponent>(SessionBoardStateComponent.ComponentGuid);
            }
            return session;
        }
    }
    
    protected BoardRenderer rendererContext;
    public BoardRenderer RendererContext
    {
        get
        {
            if (rendererContext == null)
            {
                rendererContext = GetComponent<BoardRenderer>(BoardRenderer.ComponentGuid);
            }
            return rendererContext;
        }
    }
    
    protected BoardManipulatorComponent manipulator;
    public BoardManipulatorComponent Manipulator
    {
        get
        {
            if (manipulator == null)
            {
                manipulator = GetComponent<BoardManipulatorComponent>(BoardManipulatorComponent.ComponentGuid);
            }
            return manipulator;
        }
    }
    
    protected ReproductionLogicComponent reproductionLogic;
    public ReproductionLogicComponent ReproductionLogic
    {
        get
        {
            if (reproductionLogic == null)
            {
                reproductionLogic = GetComponent<ReproductionLogicComponent>(ReproductionLogicComponent.ComponentGuid);
            }
            return reproductionLogic;
        }
    }
    
    protected EnemiesLogicComponent enemiesLogic;
    public EnemiesLogicComponent EnemiesLogic
    {
        get
        {
            if (enemiesLogic == null)
            {
                enemiesLogic = GetComponent<EnemiesLogicComponent>(EnemiesLogicComponent.ComponentGuid);
            }
            return enemiesLogic;
        }
    }
    
    protected ProductionLogicComponent productionLogic;
    public ProductionLogicComponent ProductionLogic
    {
        get
        {
            if (productionLogic == null)
            {
                productionLogic = GetComponent<ProductionLogicComponent>(ProductionLogicComponent.ComponentGuid);
            }
            return productionLogic;
        }
    }
    
    protected WorkerCurrencyLogicComponent workerLogic;
    public WorkerCurrencyLogicComponent WorkerLogic
    {
        get
        {
            if (workerLogic == null)
            {
                workerLogic = GetComponent<WorkerCurrencyLogicComponent>(WorkerCurrencyLogicComponent.ComponentGuid);
            }
            return workerLogic;
        }
    }
    
    private Dictionary<int, IPieceBuilder> pieceBuilderDef;

    public virtual void Init(Dictionary<int, IPieceBuilder> pieceBuilderDef)
    {
        this.pieceBuilderDef = pieceBuilderDef;
    }

    public virtual void CreateBoard()
    {
        BoardLogic.Init(BoardDef.Width, BoardDef.Height, BoardDef.Depth);

        if (Logger.IsLoggingEnabled == false) return;

#if UNITY_EDITOR
        string targetLog = "";

        string path = Application.dataPath.Replace("Assets", "Logs");

        if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        string targetPath = path + "/gameboard.logic.log";

        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(targetPath, false, System.Text.Encoding.UTF8);
        streamWriter.Write(targetLog);
        streamWriter.Close();
#endif
    }

    public Piece CreatePieceFromType(int pieceType)
    {
        IPieceBuilder pieceBuilder;

        if (pieceBuilderDef.TryGetValue(pieceType, out pieceBuilder))
        {
            var piece = pieceBuilder.Build(pieceType, this);
            return piece;
        }

        return null;
    }
    
    protected BoardRandomComponent random;
    public BoardRandomComponent Random 
    { 
        get
        {
            if (random == null)
            {
                random = GetComponent<BoardRandomComponent>(BoardRandomComponent.ComponentGuid);
            }
            return random;
        }
    }

}
