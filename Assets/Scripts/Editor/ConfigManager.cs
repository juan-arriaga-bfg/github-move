#if UNITY_EDITOR
using UnityEditor;

public class ConfigManager: CustomEditorBase
{
    private ViewType targetView = ViewType.None;
    
    private float offsetY = 0f;
    private float ratioCoefficient = 0.8f;
    
    private bool isRecursion;
    private bool useSpriteHeight;
    private bool useProportionalOffset;
    
    private string targetSpriteObject = "sprite";
    
    [MenuItem("Tools/Configs/Manager", false, 50)]
    public static void Create()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        window.Show();
    }
    
    protected virtual void OnGUI()
    {
        ScrollArea(this, () =>
        {
            HorizontalArea(() =>
            {
                Button("Refresh status", () => {});
                Button("Update all (force)", () => {});
                Button("Update all", () => {});
                Button("Update selected", () => {});    
            });
        });
    }

#region WindowUI
    
#endregion

#region WindowLogic
    
#endregion
    
}
#endif