using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDataManager : SequenceData, IDataLoader<List<ObstacleDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public Dictionary<int, ObstacleDef> Obstacles;
    
    private Dictionary<int, List<ObstacleDef>> branches;
    
    
    public override void Reload()
    {
        base.Reload();
        Obstacles = null;
        LoadData(new HybridConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<ObstacleDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Obstacles = new Dictionary<int, ObstacleDef>();
            branches = new Dictionary<int, List<ObstacleDef>>();
            
            if (string.IsNullOrEmpty(error))
            {
                data.Sort((a, b) => a.Piece.CompareTo(b.Piece));

                var dataManager = (GameDataManager) context;
                
                foreach (var def in data)
                {
                    Obstacles.Add(def.Piece, def);
                    AddToBranch(dataManager.MatchDefinition.GetFirst(def.Piece), def);
                    AddSequence(def.Uid, def.PieceWeights);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private void AddToBranch(int key, ObstacleDef def)
    {
        List<ObstacleDef> list;

        if (branches.TryGetValue(key, out list) == false)
        {
            list = new List<ObstacleDef>();
            branches.Add(key, list);
        }
        
        list.Add(def);
        list.Sort((a, b) => a.Piece.CompareTo(b.Piece));
    }

    private ObstacleDef GetStep(int piece, int step)
    {
        var key = GameDataService.Current.MatchDefinition.GetFirst(piece);

        if (branches.TryGetValue(key, out var list) == false) return null;
        
        step = Mathf.Clamp(step, 0, list.Count - 1);
        
        return list[step];
    }
    
    public int GetDelayByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.Delay;
    }

    public List<CurrencyPair> GetRewardByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.StepRewards;
    }

    public Dictionary<int, int> GetPiecesByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        var hard = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name);
        var extras = GameDataService.Current.LevelsManager.GetSequence(Currency.Extra.Name);
        var character = GameDataService.Current.CharactersManager.GetSequence(Currency.Character.Name);
        var sequence = GetSequence(def.Uid);
        
        var reward = hard.GetNextDict(def.ProductionAmount.Range());
        
        reward = character.GetNextDict(def.CharactersAmount.Range(), reward);
        reward = extras.GetNextDict(def.ExtrasAmount.Range(), reward);
        if(def.OtherAmount != null) reward = GetOtherPieces(def.OtherAmount.Range(), reward);
        reward = sequence.GetNextDict(def.PieceAmount, reward);
        
        return reward;
    }
    
    private Dictionary<int, int> GetOtherPieces(int amount, Dictionary<int, int> dict = null)
    {
        var result = dict ?? new Dictionary<int, int>();

        var board = BoardService.Current.FirstBoard;
        var positionsCache = board.BoardLogic.PositionsCache;
        var definition = board.BoardLogic.MatchDefinition;
        
        var idsMine = PieceType.GetIdsByFilter(PieceTypeFilter.Mine);
        var idsObstacle = PieceType.GetIdsByFilter(PieceTypeFilter.Obstacle | PieceTypeFilter.Tree);
        var typeBranches = PieceTypeBranch.Default;

        foreach (var id in idsMine)
        {
            if (positionsCache.Cache.TryGetValue(id, out var cache) == false || cache.Count == 0) continue;
            
            var def = PieceType.GetDefById(id);

            if (typeBranches.Has(def.Branch)) continue;

            typeBranches = typeBranches.Add(def.Branch);
        }
        
        foreach (var id in idsObstacle)
        {
            var def = PieceType.GetDefById(id);

            if (typeBranches.Has(def.Branch) == false) continue;

            typeBranches = typeBranches.Remove(def.Branch);
        }
        
        var idsMonument = PieceType.GetIdsByFilterAndBranch(PieceTypeFilter.Multicellular, PieceTypeFilter.Fake | PieceTypeFilter.Mine, typeBranches);

        foreach (var id in idsMonument)
        {
            if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;
            
            var def = PieceType.GetDefById(id);
            
            typeBranches = typeBranches.Remove(def.Branch);
        }

        var idsResult = PieceType.GetIdsByFilterAndBranch(PieceTypeFilter.Simple | PieceTypeFilter.Removable, typeBranches);

        if (idsResult.Count == 0) return result;
        
        var idsFirst = new List<int>();
        var idsSecond = new List<int>();

        foreach (var id in idsResult)
        {
            if (id == definition.GetFirst(id))
            {
                idsFirst.Add(id);
                continue;
            }
            
            idsSecond.Add(id);
        }
        
        for (var i = 0; i < amount; i++)
        {
            var isFirst = Random.Range(0, 4) != 0;
            var id = isFirst ? idsFirst[Random.Range(0, idsFirst.Count)] : idsSecond[Random.Range(0, idsSecond.Count)];
            
            if (result.ContainsKey(id) == false) result.Add(id, 0);
            
            result[id]++;
        }
        
        return result;
    }
    
    public Dictionary<int, int> GetPiecesByLastStep(int piece, int step)
    {
        return Obstacles.TryGetValue(piece, out var def) && def.Chest != PieceType.None.Id ? new Dictionary<int, int> {{def.Chest, 1}} : GetPiecesByStep(piece, step);
    }

    public CurrencyPair GetPriceByStep(int piece, int step)
    {
        var def = GetStep(piece, step);
        
        return def.Price;
    }
}