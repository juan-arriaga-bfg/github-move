using System;
using System.Collections.Generic;

[Flags]
public enum PieceTypeFilter
{
    Default         = 0b_00000000000000000000000000000000,
    // Not used 
    Simple          = 0b_00000000000000000000000000000001,
    Multicellular   = 0b_00000000000000000000000000000010,
    Obstacle        = 0b_00000000000000000000000000000100,
    Chest           = 0b_00000000000000000000000000001000,
    Mine            = 0b_00000000000000000000000000010000,
    Resource        = 0b_00000000000000000000000000100000,
    Ingredient      = 0b_00000000000000000000000001000000,
    Reproduction    = 0b_00000000000000000000000010000000,
    Character       = 0b_00000000000000000000000100000000,
    Fake            = 0b_00000000000000000000001000000000,
    Enemy           = 0b_00000000000000000000010000000000,
    Booster         = 0b_00000000000000000000100000000000,
    Workplace       = 0b_00000000000000000001000000000000,
    Tree            = 0b_00000000000000000010000000000000,
    ProductionField = 0b_00000000000000000100000000000000, // bed
    Removable       = 0b_00000000000000001000000000000000,
    Bag             = 0b_00000000000000010000000000000000,
    Normal          = 0b_00000000000000100000000000000000,
    Analytics        = 0b_00000000000001000000000000000000, // analytics
    OrderPiece      = 0b_00000000000010000000000000000000, // piece droped from order
}

[Flags]
public enum PieceTypeBranch : ulong
{
    Default = 0b_0000000000000000000000000000000000000000000000000000000000000000,
    // Not used
    A       = 0b_0000000000000000000000000000000000000000000000000000000000000001,
    B       = 0b_0000000000000000000000000000000000000000000000000000000000000010,
    C       = 0b_0000000000000000000000000000000000000000000000000000000000000100,
    D       = 0b_0000000000000000000000000000000000000000000000000000000000001000,
    E       = 0b_0000000000000000000000000000000000000000000000000000000000010000,
    F       = 0b_0000000000000000000000000000000000000000000000000000000000100000,
    G       = 0b_0000000000000000000000000000000000000000000000000000000001000000,
    H       = 0b_0000000000000000000000000000000000000000000000000000000010000000,
    I       = 0b_0000000000000000000000000000000000000000000000000000000100000000,
    J       = 0b_0000000000000000000000000000000000000000000000000000001000000000,
    K       = 0b_0000000000000000000000000000000000000000000000000000010000000000,
    L       = 0b_0000000000000000000000000000000000000000000000000000100000000000,
    M       = 0b_0000000000000000000000000000000000000000000000000001000000000000,
    N       = 0b_0000000000000000000000000000000000000000000000000010000000000000,
    O       = 0b_0000000000000000000000000000000000000000000000000100000000000000,
    P       = 0b_0000000000000000000000000000000000000000000000001000000000000000,
    Q       = 0b_0000000000000000000000000000000000000000000000010000000000000000,
    R       = 0b_0000000000000000000000000000000000000000000000100000000000000000,
    S       = 0b_0000000000000000000000000000000000000000000001000000000000000000,
    T       = 0b_0000000000000000000000000000000000000000000010000000000000000000,
    U       = 0b_0000000000000000000000000000000000000000000100000000000000000000,
    V       = 0b_0000000000000000000000000000000000000000001000000000000000000000,
    W       = 0b_0000000000000000000000000000000000000000010000000000000000000000,
    X       = 0b_0000000000000000000000000000000000000000100000000000000000000000,
    Y       = 0b_0000000000000000000000000000000000000001000000000000000000000000,
    Z       = 0b_0000000000000000000000000000000000000010000000000000000000000000,
}

public static class PieceTypeBranchExtentions
{
    public static bool Has(this PieceTypeBranch type, PieceTypeBranch value)
    {
        return (type & value) == value;
    }
    
    public static PieceTypeBranch Add(this PieceTypeBranch type, PieceTypeBranch value)
    {
        return type | value;
    }
    
    public static PieceTypeBranch Remove(this PieceTypeBranch type, PieceTypeBranch value)
    {
        return type & ~value;
    }
}

public static partial class PieceType
{
    public static PieceTypeDef GetDefById(int id)
    {
        return defs.ContainsKey(id) == false ? defs[None.Id] : defs[id];
    }

    public static List<int> GetIdsByFilter(PieceTypeFilter include)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            if ((def.Filter & include) != include) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    /// <summary>
    /// Return List of ids for pieces that have ALL of *include* and have no ANY of *exclude*
    /// </summary>
    /// <param name="include">Select pieces that have one of specified flags.
    /// Let Piece X1 has PieceTypeFilters: Simple, Obstacle, Tree 
    /// Let Piece X2 has PieceTypeFilters: Simple, Tree 
    /// Let Piece X3 has PieceTypeFilters: Simple
    /// GetIdsByFilter(Simple, Workplace) => X1, X2, X3
    /// GetIdsByFilter(Simple | Obstacle, Workplace) => X1
    /// </param>
    /// <param name="exclude">Pieces that has ANY of flag from this param will be excluded</param>
    /// Let Piece X1 has PieceTypeFilters: Simple, Obstacle, Tree 
    /// Let Piece X2 has PieceTypeFilters: Simple, Tree 
    /// Let Piece X3 has PieceTypeFilters: Simple
    /// GetIdsByFilter(Simple, Obstacle | Tree) => X3
    /// GetIdsByFilter(Simple, Obstacle) => X2, X3
    /// GetIdsByFilter(Simple, Tree) => X3
    public static List<int> GetIdsByFilter(PieceTypeFilter include, PieceTypeFilter exclude)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            if ((def.Filter & include) != include) continue;
            if ((def.Filter & exclude) != PieceTypeFilter.Default) continue; // Exclude pieces with ANY of flags from 'exclude' param
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetIdsByBranch(PieceTypeBranch branch)
    {
        var result = new List<int>();

        if (branch == PieceTypeBranch.Default) return result;

        foreach (var def in defs.Values)
        {
            if ((ulong) (def.Branch & branch) == 0) continue; // Exclude pieces with ANY of flags from 'exclude' param
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetIdsByFilterAndBranch(PieceTypeFilter include, PieceTypeBranch includeBranch)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            if ((def.Filter & include) != include) continue;
            if (def.Branch == PieceTypeBranch.Default || (def.Branch | includeBranch) != includeBranch) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetIdsByFilterAndExcludeBranch(PieceTypeFilter include, PieceTypeBranch exclude)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            if ((def.Filter & include) != include) continue;
            if (def.Branch != PieceTypeBranch.Default && (def.Branch | exclude) == exclude) continue; // Exclude pieces with ANY of flags from 'exclude' param
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetIdsByFilterAndBranch(PieceTypeFilter include, PieceTypeFilter exclude, PieceTypeBranch includeBranch)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            // ignore empty piece
            if ((def.Filter & include) != include) continue;
            if ((def.Filter & exclude) != PieceTypeFilter.Default) continue; // Exclude pieces with ANY of flags from 'exclude' param
            if (def.Branch == PieceTypeBranch.Default || (def.Branch | includeBranch) != includeBranch) continue;
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetIdsByFilterAndExcludeBranch(PieceTypeFilter include, PieceTypeFilter exclude, PieceTypeBranch branchExclude)
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            // ignore empty piece
            if ((def.Filter & include) != include) continue;
            if ((def.Filter & exclude) != PieceTypeFilter.Default) continue; // Exclude pieces with ANY of flags from 'exclude' param
            if (def.Branch != PieceTypeBranch.Default && (def.Branch | branchExclude) == branchExclude) continue; // Exclude pieces with ANY of flags from 'exclude' param
            
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
    
    public static List<int> GetAllIds()
    {
        var result = new List<int>();

        foreach (var def in defs.Values)
        {
            result.Add(def.Id);
        }
        
        result.Sort((a, b) => a.CompareTo(b));
        
        return result;
    }
}
