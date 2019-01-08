#if UNITY_EDITOR

using UnityEditor;

public class BuildActionSetScriptingBackendIL2CPP : BuildActionSetScriptingBackendBase
{
    public override ScriptingImplementation ScriptingImplementation => ScriptingImplementation.IL2CPP;
}

#endif