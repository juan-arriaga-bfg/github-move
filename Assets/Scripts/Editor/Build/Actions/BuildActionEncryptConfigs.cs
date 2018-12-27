public class BuildActionEncryptConfigs : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        NSConfigEncription.EncryptConfigs();
    }
}