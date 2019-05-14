public class GameDataManager : ECSEntity,
    IChestsDataManager, IPiecesDataManager, IFogsDataManager, IObstaclesDataManager, ILevelsDataManager,
    IFieldDataManager, ICodexDataManager, IEnemiesDataManager, IConstantsDataManager, IQuestsDataManager, IShopDataManager,
    IOrdersDataManager, IConversationsDataManager, IMarketDataManager, ICharactersDataManager, IDailyRewardDataManager, IAbTestDataManager, IFireflyDataManager, IEventGameDataManager, ITutorialDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private MatchDefinitionComponent matchDefinition;
    public MatchDefinitionComponent MatchDefinition => matchDefinition ?? (matchDefinition = GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid));
    
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
    
    private MarketDataManager marketManager;
    public MarketDataManager MarketManager => marketManager ?? (marketManager = GetComponent<MarketDataManager>(MarketDataManager.ComponentGuid));
    
    private CharactersDataManager charactersManager;
    public CharactersDataManager CharactersManager => charactersManager ?? (charactersManager = GetComponent<CharactersDataManager>(CharactersDataManager.ComponentGuid));
    
    private DailyRewardDataManager dailyRewardManager;
    public DailyRewardDataManager DailyRewardManager => dailyRewardManager ?? (dailyRewardManager = GetComponent<DailyRewardDataManager>(DailyRewardDataManager.ComponentGuid));
    
    private AbTestDataManager abTestManager;
    public AbTestDataManager AbTestManager => abTestManager ?? (abTestManager = GetComponent<AbTestDataManager>(AbTestDataManager.ComponentGuid));

    private FireflyDataManager fireflyManager;
    public FireflyDataManager FireflyManager => fireflyManager ?? (fireflyManager = GetComponent<FireflyDataManager>(FireflyDataManager.ComponentGuid));
    
    private EventGameDataManager eventGameManager;
    public EventGameDataManager EventGameManager => eventGameManager ?? (eventGameManager = GetComponent<EventGameDataManager>(EventGameDataManager.ComponentGuid));
    
    private TutorialDataManager tutorialDataManager;
    public TutorialDataManager TutorialDataManager => tutorialDataManager ?? (tutorialDataManager = GetComponent<TutorialDataManager>(TutorialDataManager.ComponentGuid));
    
    public UserProfile UserProfile { get; private set; } 
    
    public void SetupComponents(UserProfile userProfile)
    {
        UserProfile = userProfile;
        
        RegisterComponent(new MatchDefinitionComponent(new MatchDefinitionBuilder().Build()));
        
        RegisterComponent(new AbTestDataManager());
        RegisterComponent(new TutorialDataManager());
        RegisterComponent(new ChestsDataManager());
        RegisterComponent(new PiecesDataManager());
        RegisterComponent(new ObstaclesDataManager());
        RegisterComponent(new EnemiesDataManager());
        RegisterComponent(new ConversationsDataManager());
        RegisterComponent(new QuestsDataManager());
        RegisterComponent(new FogsDataManager());
        RegisterComponent(new FieldDataManager());
        RegisterComponent(new CodexDataManager());
        RegisterComponent(new LevelsDataManager());
        RegisterComponent(new ConstantsDataManager());
        RegisterComponent(new ShopDataManager());
        RegisterComponent(new OrdersDataManager());
        RegisterComponent(new MarketDataManager());
        RegisterComponent(new CharactersDataManager());
        RegisterComponent(new DailyRewardDataManager());
        RegisterComponent(new FireflyDataManager());
        RegisterComponent(new EventGameDataManager());
    }
}