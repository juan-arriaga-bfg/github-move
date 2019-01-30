using System;

namespace BfgAnalytics
{
    [Flags]
    public enum JsonDataGroup
    {
        None      = 0b_000001,
        Userstats = 0b_000010,
        Balance   = 0b_000100,
        Flags     = 0b_001000,
        Story     = 0b_010000,
        Standart  = 0b_100000,
    }
}