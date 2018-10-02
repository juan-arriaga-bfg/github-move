using Random = UnityEngine.Random;

public class RandomPieceSkin : PieceSkin
{
    private int totalWeight;

    public override void Init(int value)
    {
        base.Init(GetRandom());
    }
    
    private void OnDisable()
    {
        totalWeight = 0;
    }
    
    private int GetRandom()
    {
        if (totalWeight == 0) CalculationTotalWeight();
        
        var curWeight = 0;
        var randomValue = Random.Range(0, totalWeight + 1);

        for (var i = 0; i < skins.Count; i++)
        {
            curWeight += skins[i].Weight;
            
            if (curWeight >= randomValue) return i;
        }
        
        return 0;
    }

    private void CalculationTotalWeight()
    {
        foreach (var skin in skins)
        {
            totalWeight += skin.Weight;
        }
    }
}