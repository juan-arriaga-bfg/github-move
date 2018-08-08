using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject]
public class CodexChainState
{
    public HashSet<int> Unlocked = new HashSet<int>();
    public HashSet<int> PendingReward = new HashSet<int>(); 
}