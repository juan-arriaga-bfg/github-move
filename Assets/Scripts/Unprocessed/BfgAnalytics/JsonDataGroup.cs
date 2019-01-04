using System;

namespace BfgAnalytics
{
    [Flags]
    public enum JsonDataGroup
    {
        None      = 0b_0001,
        Userstats = 0b_0010,
        Balance   = 0b_0100,
        Flags     = 0b_1000
    }
}