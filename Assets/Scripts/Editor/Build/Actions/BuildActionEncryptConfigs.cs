#if UNITY_EDITOR

public class BuildActionEncryptConfigs : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        NSConfigEncription.EncryptConfigs();
    }
}

#endif