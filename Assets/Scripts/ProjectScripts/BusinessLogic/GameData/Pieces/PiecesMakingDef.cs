using System.Collections.Generic;

public class PiecesMakingDef : SimplePieceDef
{
	public int PieceAmount;
	public CurrencyPair Price;
	public List<CurrencyPair> StepRewards;
	public List<ItemWeight> PieceWeights;
}