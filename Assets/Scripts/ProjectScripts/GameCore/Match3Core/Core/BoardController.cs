using System.Collections.Generic;
using UnityEngine;

public class BoardController : ECSEntity,
    IActionExecuteComponent, IBoardEventsComponent, IBoardLoggerComponent, IBoardLogicComponent, IBoardDefinitionComponent, IBoardStatesComponent, ISessionBoardStateComponent,
    IBoardSystemProcessor, IBoardRendererComponent, IBoardManipulatorComponent, IBoardRandomComponent, IWorkerCurrencyLogicComponent, IHintCooldownComponent,
    IPartPiecesLogicComponent, IPathfinderComponent, IFreeChestLogicComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public IECSSystemProcessor SystemProcessor => ECSService.Current.SystemProcessor;

    // components

    protected ActionExecuteComponent actionExecutor;
    public virtual ActionExecuteComponent ActionExecutor => actionExecutor ?? (actionExecutor = GetComponent<ActionExecuteComponent>(ActionExecuteComponent.ComponentGuid));

    protected BoardEventsComponent boardEvents;
    public virtual BoardEventsComponent BoardEvents => boardEvents ?? (boardEvents = GetComponent<BoardEventsComponent>(BoardEventsComponent.ComponentGuid));

    protected BoardLoggerComponent logger;
    public virtual BoardLoggerComponent Logger => logger ?? (logger = GetComponent<BoardLoggerComponent>(BoardLoggerComponent.ComponentGuid));

    protected BoardLogicComponent boardLogic;
    public virtual BoardLogicComponent BoardLogic => boardLogic ?? (boardLogic = GetComponent<BoardLogicComponent>(BoardLogicComponent.ComponentGuid));

    protected BoardDefinitionComponent boardDef;
    public virtual BoardDefinitionComponent BoardDef => boardDef ?? (boardDef = GetComponent<BoardDefinitionComponent>(BoardDefinitionComponent.ComponentGuid));

    protected BoardStatesComponent states;
    public virtual BoardStatesComponent States => states ?? (states = GetComponent<BoardStatesComponent>(BoardStatesComponent.ComponentGuid));

    protected SessionBoardStateComponent session;
    public SessionBoardStateComponent Session => session ?? (session = States.GetState<SessionBoardStateComponent>(SessionBoardStateComponent.ComponentGuid));

    protected BoardRenderer rendererContext;
    public BoardRenderer RendererContext => rendererContext ?? (rendererContext = GetComponent<BoardRenderer>(BoardRenderer.ComponentGuid));

    protected BoardManipulatorComponent manipulator;
    public BoardManipulatorComponent Manipulator => manipulator ?? (manipulator = GetComponent<BoardManipulatorComponent>(BoardManipulatorComponent.ComponentGuid));
    
    protected WorkerCurrencyLogicComponent workerLogic;
    public WorkerCurrencyLogicComponent WorkerLogic => workerLogic ?? (workerLogic = GetComponent<WorkerCurrencyLogicComponent>(WorkerCurrencyLogicComponent.ComponentGuid));

    protected HintCooldownComponent hintCooldown;
    public HintCooldownComponent HintCooldown => hintCooldown ?? (hintCooldown = GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid));

    protected PartPiecesLogicComponent partPiecesLogic;
    public PartPiecesLogicComponent PartPiecesLogic => partPiecesLogic ?? (partPiecesLogic = GetComponent<PartPiecesLogicComponent>(PartPiecesLogicComponent.ComponentGuid));

    protected PathfinderComponent pathfinderComponent;
    public PathfinderComponent Pathfinder => pathfinderComponent ?? (pathfinderComponent = GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid));
    
    protected PathfindLockerComponent pathfindLockerComponent;
    public PathfindLockerComponent PathfindLocker => pathfindLockerComponent ?? (pathfindLockerComponent = GetComponent<PathfindLockerComponent>(PathfindLockerComponent.ComponentGuid));
    
    protected FreeChestLogicComponent freeChestLogic;
    public FreeChestLogicComponent FreeChestLogic => freeChestLogic ?? (freeChestLogic = GetComponent<FreeChestLogicComponent>(FreeChestLogicComponent.ComponentGuid));
    
    protected BoardRandomComponent random;
    public BoardRandomComponent Random => random ?? (random = GetComponent<BoardRandomComponent>(BoardRandomComponent.ComponentGuid));

    protected AreaAccessControllerComponent areaAccess;
    public AreaAccessControllerComponent AreaAccessController => areaAccess ?? (areaAccess = GetComponent<AreaAccessControllerComponent>(AreaAccessControllerComponent.ComponentGuid));
    
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
}
