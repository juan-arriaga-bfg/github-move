namespace Quests
{
    public enum TaskState
    {
        Pending,       // Task created but not react to any game event
        New,           // Task will react to corresponded event but player is not reviewed it yet
        InProgress,    // Task will react to corresponded event and player reviewed task
        Completed,     // All required actions are done by a player and he can claim a reward (if any) 
        Claimed        // Reward claimed
    }
}