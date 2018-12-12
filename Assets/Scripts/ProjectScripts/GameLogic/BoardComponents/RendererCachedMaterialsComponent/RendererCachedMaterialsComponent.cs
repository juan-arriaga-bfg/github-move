using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRendererCachedMaterialsComponent
{
    RendererCachedMaterialsComponent MaterialsCache { get; }
}

public partial class BoardRenderer : IRendererCachedMaterialsComponent
{
    private RendererCachedMaterialsComponent materialsCache;
    
    public RendererCachedMaterialsComponent MaterialsCache => materialsCache ?? (materialsCache = GetComponent<RendererCachedMaterialsComponent>(RendererCachedMaterialsComponent.ComponentGuid));
}

public partial class BoardElementMaterialType
{
    public const string PiecesDefaultMaterial = R.PiecesDefaultMaterial;

    public const string PiecesFadeMaterial = R.PiecesFadeMaterial;

    public const string PiecesGrayscaleMaterial = R.PiecesGrayscaleMaterial;

    public const string PiecesHighlightMaterial = R.PiecesHighlightMaterial;

    public const string FogDefaultMaterial = R.FogDefaultMaterial;
    
    public const string PiecesUnderFogMaterial = R.PiecesUnderFogMaterial;

    public const string PiecesLockedMaterial = R.PiecesLockedMaterial;
}

public class RendererCachedMaterialsComponent : ECSEntity 
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    protected BoardRenderer context;

    public BoardRenderer Context
    {
        get { return context; }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);

        context = entity as BoardRenderer;
    }
    
    protected Dictionary<string, Material> cachedMaterials = new Dictionary<string, Material>();
    
    protected Dictionary<object, Dictionary<string, Material>> cachedGroupsMaterials = new Dictionary<object, Dictionary<string, Material>>();

    public Material GetMaterial(string uid)
    {
        Material targetMaterial;

        if (cachedMaterials.TryGetValue(uid, out targetMaterial))
        {
            return targetMaterial;
        }

        targetMaterial = ContentService.Current.GetObjectByName(uid) as Material;

        if (targetMaterial == null)
        {
            Debug.LogError($"[RendererCachedMaterialsComponent] no materials for:{uid}");
        }

        return targetMaterial;
    }
    
    public Material GetMaterialForGroup(string uid, object groupTag, bool isCreateIfNotExist = true)
    {
        if (cachedGroupsMaterials.ContainsKey(groupTag) == false)
        {
            cachedGroupsMaterials.Add(groupTag, new Dictionary<string, Material>());
        }

        var groupMaterials = cachedGroupsMaterials[groupTag];
        
        Material targetMaterial;

        if (groupMaterials.TryGetValue(uid, out targetMaterial))
        {
            return targetMaterial;
        }

        if (isCreateIfNotExist == false)
        {
            return null;
        }

        targetMaterial = ContentService.Current.GetObjectByName(uid) as Material;
        targetMaterial = new Material(targetMaterial);

        if (targetMaterial == null)
        {
            Debug.LogError($"[RendererCachedMaterialsComponent] no materials for:{uid}");
        }
        
        groupMaterials.Add(uid, targetMaterial);

        return targetMaterial;
    }
    
}
