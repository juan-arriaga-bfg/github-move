using System;
using UnityEngine;

public class Quest
{
    private QuestDef def;

    private int currentAmount;
    
    public Quest(QuestDef def)
    {
        this.def = def;
        WantedPiece = PieceType.Parse(def.Price.Currency);
    }
    
    public QuestDef Def
    {
        get { return def; }
    }
    
    public int WantedPiece { get; private set; }
    
    public string WantedIcon
    {
        get { return PieceType.Parse(WantedPiece); }
    }
    
    public int TargetAmount
    {
        get { return def.Price.Amount; }
    }

    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = Mathf.Min(value, def.Price.Amount); }
    }

    public CurrencyPair Reward
    {
        get { return def.Reward; }
    }

    public bool Check()
    {
        return CurrentAmount >= TargetAmount;
    }

    public void Complete(Action onComplete)
    {
        if(onComplete != null) onComplete();
    }
}