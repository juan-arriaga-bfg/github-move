using System.Collections.Generic;
using UnityEngine;

public class LevelsDataManager : SequenceData, IDataLoader<List<LevelsDef>>
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	public List<LevelsDef> Levels;

	private MatchDefinitionComponent cachedMatchDef;
	private MatchDefinitionComponent CachedMatchDef
	{
		get
		{
			if (cachedMatchDef != null) return cachedMatchDef;
			
			return (cachedMatchDef = BoardService.Current?.FirstBoard?.BoardLogic?.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid))
			       ?? new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
		}
	}

	public override void Reload()
	{
		base.Reload();
		Levels = null;
		LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void UpdateSequence()
	{
		GetSequence(Currency.Level.Name).Reinit(Levels[Level - 1].PieceWeights);
		GetSequence(Currency.Order.Name).Reinit(Levels[Level - 1].OrdersWeights);
		GetSequence(Currency.Resources.Name).Reinit(Levels[Level - 1].ResourcesWeights);
		GetSequence(Currency.Character.Name).Reinit(Levels[Level - 1].CharactersWeights);
	}

	public void LoadData(IDataMapper<List<LevelsDef>> dataMapper)
	{
		dataMapper.LoadData((data, error) =>
		{
			if (string.IsNullOrEmpty(error))
			{
				data.Sort((a, b) => a.Index.CompareTo(b.Index));
				Levels = new List<LevelsDef>{data[0]};
				
				for (var i = 1; i < data.Count; i++)
				{
					var previous = data[i - 1];
					var next = data[i];
					
					next.OrdersWeights = ItemWeight.ReplaseWeights(previous.OrdersWeights, next.OrdersWeights);
					next.PieceWeights = ItemWeight.ReplaseWeights(previous.PieceWeights, next.PieceWeights);
					next.ResourcesWeights = ItemWeight.ReplaseWeights(previous.ResourcesWeights, next.ResourcesWeights);
					next.CharactersWeights = ItemWeight.ReplaseWeights(previous.CharactersWeights, next.CharactersWeights);
					
					Levels.Add(next);
				}

				var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Character);

				foreach (var id in ids)
				{
					if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;
					
					FiltrationCharactersWeights(id);
				}
				
				AddSequence(Currency.Level.Name, Levels[Level - 1].PieceWeights);
				AddSequence(Currency.Resources.Name, Levels[Level - 1].ResourcesWeights);
				AddSequence(Currency.Order.Name, Levels[Level - 1].OrdersWeights);
				AddSequence(Currency.Character.Name, Levels[Level - 1].CharactersWeights);
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}

	public int Level => ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;

	public int Price
	{
		get
		{
			var price = Levels[Level - 1].Price;
			return price.Amount;
		}
	}

	public List<CurrencyPair> Rewards => Levels[Level - 1].Rewards;
	public List<ItemWeight> PieceWeights => Levels[Level - 1].PieceWeights;
	public List<ItemWeight> ResourcesWeights => Levels[Level - 1].ResourcesWeights;
	public List<ItemWeight> CharactersWeights => Levels[Level - 1].CharactersWeights;
	
    public int OrdersDelay => Levels[Level - 1].OrdersDelay;

    public void UnlockNewCharacter(int id)
    {
	    FiltrationCharactersWeights(id);
	    GetSequence(Currency.Character.Name).Reinit(Levels[Level - 1].CharactersWeights);
    }
    
    private void FiltrationCharactersWeights(int id)
    {
	    var chain = CachedMatchDef.GetChain(id);
	    
	    foreach (var def in Levels)
	    {
		    for (var i = def.CharactersWeights.Count - 1; i >= 0; i--)
		    {
			    var weights = def.CharactersWeights[i];
			    
			    if(chain.Contains(weights.Piece) == false) continue;

			    def.CharactersWeights.RemoveAt(i);
		    }
	    }
    }
}