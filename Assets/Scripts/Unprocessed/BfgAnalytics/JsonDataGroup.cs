using System;
// ReSharper disable IdentifierTypo

namespace BfgAnalytics
{
    [Flags]
    public enum JsonDataGroup
    {
        None        = 0b_0000_0000,
        Userstats   = 0b_0000_0001,
        Balances    = 0b_0000_0010,
        Flags       = 0b_0000_0100,
        Story       = 0b_0000_1000,
        Standart    = 0b_0001_0000, 
        // Transaction = 0b_0010_0000,
        Abtest      = 0b_0100_0000,
    }                 
}                     