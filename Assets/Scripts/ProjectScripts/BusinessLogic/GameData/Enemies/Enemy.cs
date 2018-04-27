using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private EnemyDef def;

    public EnemyDef Def
    {
        get { return def; }
    }

    private readonly int life;
    private int damage;

    private int step;
    private int reward;

    public Enemy(EnemyDef def)
    {
        this.def = def;

        life = def.Steps[def.Steps.Count - 1].Amount;
        reward = PieceType.None.Id;
    }
    
    public float Progress
    {
        get { return Mathf.Clamp(damage / (float) life, 0, 1); }
    }

    public void SetDamage(int value)
    {
        damage = Mathf.Min(damage + value, life);

        if (damage < def.Steps[step].Amount) return;

        reward = PieceType.Parse(def.Steps[step].Currency);
        step++;
    }

    public int GetReward()
    {
        var current = reward;
        reward = PieceType.None.Id;
        
        return current;
    }

    public int Damage
    {
        get { return damage; }
    }

    public bool IsComplete
    {
        get { return damage >= life; }
    }
}