public struct ActionHistoryStep 
{
    public int StepIndex { get; set; }
    
    public int ActionGuid { get; set; }
    
    public long TimeDuration { get; set; }

    public string ActionType { get; set; }
}
