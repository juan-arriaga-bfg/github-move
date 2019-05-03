using System;
using System.Collections.Generic;
using System.Linq;

public class EventGamesLogicComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
	
    private BoardLogicComponent context;
    
    public Action<EventGameType> OnStart;
    public Action<EventGameType> OnStop;

    public readonly Dictionary<EventGameType, Dictionary<long, EventGame>> EventGames = new Dictionary<EventGameType, Dictionary<long, EventGame>>();
    
    private readonly List<EventGame> waiting = new List<EventGame>();

    private TimerComponent starter;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
        starter = new TimerComponent
        {
            Delay = 30,
            OnComplete = Check
        };

        RegisterComponent(starter);
        starter.Start();
        
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
                    game.InitData(DateTimeExtension.UnixTimeToDateTime(item.StartTime),DateTimeExtension.UnixTimeToDateTime(item.EndTime), item.Intro);
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
    }

    public void AddEventGame(EventGame eventGame)
    {
        if (eventGame.State == EventGameState.Default)
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
            oldGame.UpdateData(eventGame.EndTime, eventGame.IntroDuration);
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

    private void Check()
    {
        var now = SecuredTimeService.Current.UtcNow.ConvertToUnixTime();
        
        for (var i = waiting.Count - 1; i >= 0; i--)
        {
            var game = waiting[i];
            
            if(game.StartTimeLong > now) continue;

            waiting.Remove(game);
            AddEventGame(game);
        }
        
        starter.Start();
    }
}