using System.Collections.Generic;

public class PieceMakingDef : SimplePieceDef
{
	public int PieceAmount;
	public CurrencyPair Price;
	public List<CurrencyPair> StepRewards;
	public List<ItemWeight> PieceWeights;
}