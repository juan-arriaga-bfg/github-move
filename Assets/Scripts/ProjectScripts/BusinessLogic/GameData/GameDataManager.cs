public class GameDataManager : ECSEntity,
    IChestsDataManager, IPiecesDataManager, IFogsDataManager, IMinesDataManager, IObstaclesDataManager, ILevelsDataManager,
    IFieldDataManager, ICodexDataManager, IEnemiesDataManager, IConstantsDataManager, IQuestsDataManager, IShopDataManager,
    IOrdersDataManager, IConversationsDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private ChestsDataManager chestsManager;
    public ChestsDataManager ChestsManager => chestsManager ?? (chestsManager = GetComponent<ChestsDataManager>(ChestsDataManager.ComponentGuid));
    
    private PiecesDataManager piecesManager;
    public PiecesDataManager PiecesManager => piecesManager ?? (piecesManager = GetComponent<PiecesDataManager>(PiecesDataManager.ComponentGuid));
    
    private LevelsDataManager levelsManager;
    public LevelsDataManager LevelsManager => levelsManager ?? (levelsManager = GetComponent<LevelsDataManager>(LevelsDataManager.ComponentGuid));
    
    private FogsDataManager fogsManager;
    public FogsDataManager FogsManager => fogsManager ?? (fogsManager = GetComponent<FogsDataManager>(FogsDataManager.ComponentGuid));

    private QuestsDataManager questsManager;
    public QuestsDataManager QuestsManager => questsManager ?? (questsManager = GetComponent<QuestsDataManager>(QuestsDataManager.ComponentGuid));
    
    private ObstaclesDataManager obstaclesManager;
    public ObstaclesDataManager ObstaclesManager => obstaclesManager ?? (obstaclesManager = GetComponent<ObstaclesDataManager>(ObstaclesDataManager.ComponentGuid));
    
    private MinesDataManager minesManager;
    public MinesDataManager MinesManager => minesManager ?? (minesManager = GetComponent<MinesDataManager>(MinesDataManager.ComponentGuid));
    
    private FieldDataManager fieldManager;
    public FieldDataManager FieldManager => fieldManager ?? (fieldManager = GetComponent<FieldDataManager>(FieldDataManager.ComponentGuid));
    
    private CodexDataManager codexManager;
    public CodexDataManager CodexManager => codexManager ?? (codexManager = GetComponent<CodexDataManager>(CodexDataManager.ComponentGuid));
   
    private EnemiesDataManager enemyManager;
    public EnemiesDataManager EnemiesManager => enemyManager ?? (enemyManager = GetComponent<EnemiesDataManager>(EnemiesDataManager.ComponentGuid));
    
    private ConstantsDataManager constantsManager;
    public ConstantsDataManager ConstantsManager => constantsManager ?? (constantsManager = GetComponent<ConstantsDataManager>(ConstantsDataManager.ComponentGuid));
    
    private ShopDataManager shopManager;
    public ShopDataManager ShopManager => shopManager ?? (shopManager = GetComponent<ShopDataManager>(ShopDataManager.ComponentGuid));
    
    private OrdersDataManager ordersManager;
    public OrdersDataManager OrdersManager => ordersManager ?? (ordersManager = GetComponent<OrdersDataManager>(OrdersDataManager.ComponentGuid));
    
    private ConversationsDataManager conversationsManager;
    public ConversationsDataManager ConversationsManager => conversationsManager ?? (conversationsManager = GetComponent<ConversationsDataManager>(ConversationsDataManager.ComponentGuid));
    
    public void SetupComponents()
    {
        RegisterComponent(new ChestsDataManager());
        RegisterComponent(new PiecesDataManager());
        RegisterComponent(new ObstaclesDataManager());
        RegisterComponent(new EnemiesDataManager());
        RegisterComponent(new ConversationsDataManager());
        RegisterComponent(new QuestsDataManager());
        RegisterComponent(new FogsDataManager());
        RegisterComponent(new LevelsDataManager());
        RegisterComponent(new MinesDataManager());
        RegisterComponent(new FieldDataManager());
        RegisterComponent(new CodexDataManager()); // should be the last one
        RegisterComponent(new ConstantsDataManager());
        RegisterComponent(new ShopDataManager());
        RegisterComponent(new OrdersDataManager());
    }

    public void Reload()
    {
        foreach (var component in componentsCache.Values)
        {
            var manager = component as IDataManager;

            manager?.Reload();
        }
    }
}