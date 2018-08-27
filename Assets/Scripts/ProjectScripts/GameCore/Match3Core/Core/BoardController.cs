using System.Collections.Generic;
using UnityEngine;

public class BoardController : ECSEntity,
    IActionExecuteComponent, IBoardEventsComponent, IBoardLoggerComponent, IBoardLogicComponent, IBoardDefinitionComponent, IBoardStatesComponent, ISessionBoardStateComponent,
    IBoardSystemProcessor, IBoardRendererComponent, IBoardManipulatorComponent, IBoardRandomComponent, IReproductionLogicComponent, IEnemiesLogicComponent,
    IProductionLogicComponent, IWorkerCurrencyLogicComponent, IHintCooldownComponent, IPartPiecesLogicComponent, IPathfinderComponent
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
        get { return actionExecutor ?? (actionExecutor = GetComponent<ActionExecuteComponent>(ActionExecuteComponent.ComponentGuid)); }
    }

    protected BoardEventsComponent boardEvents;
    public virtual BoardEventsComponent BoardEvents
    {
        get { return boardEvents ?? (boardEvents = GetComponent<BoardEventsComponent>(BoardEventsComponent.ComponentGuid)); }
    }

    protected BoardLoggerComponent logger;
    public virtual BoardLoggerComponent Logger
    {
        get { return logger ?? (logger = GetComponent<BoardLoggerComponent>(BoardLoggerComponent.ComponentGuid)); }
    }

    protected BoardLogicComponent boardLogic;
    public virtual BoardLogicComponent BoardLogic
    {
        get { return boardLogic ?? (boardLogic = GetComponent<BoardLogicComponent>(BoardLogicComponent.ComponentGuid)); }
    }

    protected BoardDefinitionComponent boardDef;

    public virtual BoardDefinitionComponent BoardDef
    {
        get { return boardDef ?? (boardDef = GetComponent<BoardDefinitionComponent>(BoardDefinitionComponent.ComponentGuid)); }
    }

    protected BoardStatesComponent states;
    public virtual BoardStatesComponent States
    {
        get { return states ?? (states = GetComponent<BoardStatesComponent>(BoardStatesComponent.ComponentGuid)); }
    }
    
    protected SessionBoardStateComponent session;
    public SessionBoardStateComponent Session
    {
        get { return session ?? (session = States.GetState<SessionBoardStateComponent>(SessionBoardStateComponent.ComponentGuid)); }
    }
    
    protected BoardRenderer rendererContext;
    public BoardRenderer RendererContext
    {
        get { return rendererContext ?? (rendererContext = GetComponent<BoardRenderer>(BoardRenderer.ComponentGuid)); }
    }
    
    protected BoardManipulatorComponent manipulator;
    public BoardManipulatorComponent Manipulator
    {
        get { return manipulator ?? (manipulator = GetComponent<BoardManipulatorComponent>(BoardManipulatorComponent.ComponentGuid)); }
    }
    
    protected ReproductionLogicComponent reproductionLogic;
    public ReproductionLogicComponent ReproductionLogic
    {
        get { return reproductionLogic ?? (reproductionLogic = GetComponent<ReproductionLogicComponent>(ReproductionLogicComponent.ComponentGuid)); }
    }
    
    protected EnemiesLogicComponent enemiesLogic;
    public EnemiesLogicComponent EnemiesLogic
    {
        get { return enemiesLogic ?? (enemiesLogic = GetComponent<EnemiesLogicComponent>(EnemiesLogicComponent.ComponentGuid)); }
    }
    
    protected ProductionLogicComponent productionLogic;
    public ProductionLogicComponent ProductionLogic
    {
        get { return productionLogic ?? (productionLogic = GetComponent<ProductionLogicComponent>(ProductionLogicComponent.ComponentGuid)); }
    }
    
    protected WorkerCurrencyLogicComponent workerLogic;
    public WorkerCurrencyLogicComponent WorkerLogic
    {
        get { return workerLogic ?? (workerLogic = GetComponent<WorkerCurrencyLogicComponent>(WorkerCurrencyLogicComponent.ComponentGuid)); }
    }
    
    protected HintCooldownComponent hintCooldown;
    public HintCooldownComponent HintCooldown
    {
        get { return hintCooldown ?? (hintCooldown = GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid)); }
    }
    
    protected PartPiecesLogicComponent partPiecesLogic;
    public PartPiecesLogicComponent PartPiecesLogic
    {
        get { return partPiecesLogic ?? (partPiecesLogic = GetComponent<PartPiecesLogicComponent>(PartPiecesLogicComponent.ComponentGuid)); }
    }
    
    protected PathfinderComponent pathfinderComponent;
    public PathfinderComponent Pathfinder
    {
        get { return pathfinderComponent ?? (pathfinderComponent = GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid)); }
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
