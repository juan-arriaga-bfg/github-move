#if UNITY_EDITOR

using UnityEditor;

public class BuildActionSetScriptingBackendIL2CPP : BuildActionSetScriptingBackendBase
{
    protected override ScriptingImplementation ScriptingImplementation => ScriptingImplementation.IL2CPP;
}

#endif