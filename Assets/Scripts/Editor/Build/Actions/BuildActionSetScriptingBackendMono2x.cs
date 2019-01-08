#if UNITY_EDITOR

using UnityEditor;

public class BuildActionSetScriptingBackendMono2x : BuildActionSetScriptingBackendBase
{
    public override ScriptingImplementation ScriptingImplementation => ScriptingImplementation.Mono2x;
}

#endif