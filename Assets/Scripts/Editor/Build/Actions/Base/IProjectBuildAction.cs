#if UNITY_EDITOR

public interface IProjectBuildAction
{
    void Execute(ProjectBuilder context);
}

#endif