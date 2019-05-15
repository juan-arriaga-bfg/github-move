using System;
using System.Collections.Generic;
using System.Linq;

public class EventGamesLogicComponent : ECSEntity, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
	
    public Action<EventGameType> OnStart;
    public Action<EventGameType> OnStop;

    public readonly Dictionary<EventGameType, Dictionary<long, EventGame>> EventGames = new Dictionary<EventGameType, Dictionary<long, EventGame>>();
    
    private readonly List<EventGame> waiting = new List<EventGame>();
    
    private TimerComponent starter;
    
    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        locker = new LockerComponent();
        RegisterComponent(locker);

        if (GameDataService.Current.TutorialDataManager.CheckUnlockEventGame() == false) Locker.Lock(this);
        
        starter = new TimerComponent
        {
            Delay = 30,
            OnComplete = Check
        };

        RegisterComponent(starter);
        
        var save = ProfileService.Current.EventGameSave?.EventGames;
        var defs = GameDataService.Current.EventGameManager.Defs;
        var games = GameDataService.Current.EventGameManager.GetNewEventGame();

        if (save != null)
        {
            foreach (var item in save)
            {
                if (defs.TryGetValue(item.Key, out var def) == false) continue;
                
                var game = games.Find(g => g.EventType == item.Key && g.StartTimeLong == item.StartTime);

                if (game == null)
                {
                    game = new EventGame {EventType = item.Key};
                    game.InitData(item.StartTime, item.EndTime, item.IntroTime);
                }

                game.State = item.State;
                game.Steps = new List<EventGameStepDef>();
                
                for (var i = 0; i < def.Count; i++)
                {
                    var step = def[i].Copy();
                    var stepSave = item.Steps[i];
                
                    step.IsNormalClaimed = stepSave.Key;
                    step.IsPremiumClaimed = stepSave.Value;
                    game.Steps.Add(step);
                }
                
                games.Remove(game);
                AddEventGame(game);
            }
        }
        
        foreach (var game in games)
        {
            AddEventGame(game);
        }
        
        OnServerDataReceived(GameEventServerSideConfigLoader.ComponentGuid, ServerSideConfigService.Current.GetData<List<GameEventServerConfig>>());
        ServerSideConfigService.Current.OnDataReceived += OnServerDataReceived;
        Check();
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        ServerSideConfigService.Current.OnDataReceived -= OnServerDataReceived;
        starter.OnComplete = null;
    }

    public void Unlock()
    {
        Locker.Unlock(this);
    }
    
    private void OnServerDataReceived(int guid, object data)
    {
        if (guid != GameEventServerSideConfigLoader.ComponentGuid) return;

        var serverData = (List<GameEventServerConfig>) data;
        
        if (serverData == null) return;
        
        foreach (var config in serverData)
        {
            if (Enum.TryParse<EventGameType>(config.Type, false, out var gameType) == false || gameType != EventGameType.OrderSoftLaunch) continue;

            var game = new EventGame {EventType = EventGameType.OrderSoftLaunch};
            
            game.InitData(config.Start, config.End, config.IntroDuration);
            AddEventGame(game);
        }
    }

    private void AddEventGame(EventGame eventGame)
    {
        if (Locker.IsLocked || eventGame.State == EventGameState.Default)
        {
            if (waiting.IndexOf(eventGame) == -1) waiting.Add(eventGame);
            return;
        }
        
        var key = eventGame.EventType;
        var start = eventGame.StartTimeLong;

        if (EventGames.TryGetValue(key, out var games) == false)
        {
            games = new Dictionary<long, EventGame>();
            EventGames.Add(key, games);
        }

        if (games.TryGetValue(start, out var oldGame))
        {
            oldGame.UpdateData(eventGame.EndTime, eventGame.IntroTime);
            return;
        }

        if (eventGame.Steps == null)
        {
            if (GameDataService.Current.EventGameManager.Defs.TryGetValue(eventGame.EventType, out var def) == false) return;
            
            eventGame.Steps = new List<EventGameStepDef>();

            foreach (var step in def)
            {
                eventGame.Steps.Add(step.Copy());
            }
        }
        
        RegisterComponent(eventGame, true);
        games.Add(start, eventGame);
        eventGame.Init();
    }

    public bool GetEventGame(EventGameType key, out EventGame game)
    {
        game = null;
        
        if (EventGames.TryGetValue(key, out var games) == false || games.Count == 0) return false;
        
        var starters = games.Keys.ToList();
        
        starters.Sort((a, b) => a.CompareTo(b));

        var min = starters[0];

        game = games[min];

        return true;
    }

    public void Check()
    {
        starter.Stop();
        starter.Start();
        
        if (Locker.IsLocked) return;
        
        var now = SecuredTimeService.Current.UtcNow.ConvertToUnixTime();
        
        for (var i = waiting.Count - 1; i >= 0; i--)
        {
            var game = waiting[i];

            if (game.StartTimeLong > now) continue;

            if ((game.State == EventGameState.Default || game.State == EventGameState.Start) &&
                game.EndTime.ConvertToUnixTime() <= now)
            {
                waiting.Remove(game);
                continue;
            }
            
            game.State = EventGameState.Start;
            waiting.Remove(game);
            AddEventGame(game);
        }
    }
}