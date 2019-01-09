#if UNITY_EDITOR

using UnityEditor;

public class BuildActionSetScriptingBackendMono2x : BuildActionSetScriptingBackendBase
{
    protected override ScriptingImplementation ScriptingImplementation => ScriptingImplementation.Mono2x;
}

#endif