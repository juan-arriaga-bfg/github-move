using System;
using System.Collections.Generic;

public class BoardRandomComponent : IECSComponent 
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        random = new Random();
        
        // level_0 seed 100 result:win (1,2 stars)
        
        // level_0 seed 300 result:win (1,2 stars)
        
        SetSeed(1000);
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }

    private Random random;

    public void SetSeed(int seed)
    {
        random = new Random(seed);
    }

    public int GetRandom(int from, int to)
    {
        return random.Next(from, to);
    }
    
    public int GetRandom(int from, int to, Dictionary<int, int> wights)
    {
        int sum = 0;
        foreach (var wight in wights)
        {
            sum = sum + wight.Value;
        }
        
        var coef = random.Next(1, sum + 1);

        sum = 0;
        int target = random.Next(from, to);
        foreach (var wight in wights)
        {
            if ((coef >= sum) && coef < (sum + wight.Value))
            {
                target = wight.Key;
                break;
            }
            
            sum = sum + wight.Value;
        }
        
        return target;
    }
    
}
