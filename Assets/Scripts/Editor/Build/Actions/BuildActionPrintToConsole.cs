#if UNITY_EDITOR

using UnityEngine;

public class BuildActionPrintToConsole : IProjectBuildAction
{
    private string message;

    public BuildActionPrintToConsole(string message)
    {
        this.message = message;
    }
    
    public BuildActionPrintToConsole()
    {
    }

    public IProjectBuildAction SetMessage(string message)
    {
        this.message = message;
        return this;
    }
    
    public void Execute(ProjectBuilder context)
    {
        Debug.Log(message);
    }
}

#endif