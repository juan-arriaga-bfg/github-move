#if UNITY_EDITOR

using UnityEditor;

public class BuildActionSetScriptingBackend : BuildActionSetScriptingBackendBase
{
    private ScriptingImplementation scriptingImplementation;

    public override ScriptingImplementation ScriptingImplementation => scriptingImplementation;

    public IProjectBuildAction SetType(ScriptingImplementation scriptingImplementation)
    {
        this.scriptingImplementation = scriptingImplementation;
        return this;
    }
    
    public BuildActionSetScriptingBackend(ScriptingImplementation scriptingImplementation)
    {
        this.scriptingImplementation = scriptingImplementation;
    }
    
    public BuildActionSetScriptingBackend()
    {
    }
}
#endif