using UnityEngine;
using UnityEngine.U2D;

public class DefaultSpriteManager : IconResourceManager
{
	protected override Sprite TryGetSpriteFromBundle(IconData iconData)
	{
	    if (string.IsNullOrEmpty(iconData.BundleName)) return null;

	    AssetBundle assetBundle = IWAssetBundleService.Instance.Manager.GetBundleFromCache(iconData.BundleName);

	    if (assetBundle == null) return null;

	    if (assetBundle.Contains( iconData.SpriteName ))
	    {
	        Sprite targetSprite = assetBundle.LoadAsset<Sprite>( iconData.SpriteName );

	        return targetSprite;
	    }
        
#if UNITY_2018_3_OR_NEWER
        
	    if (assetBundle.Contains( iconData.SpriteSheetName ))
	    {
	        SpriteAtlas spriteAtlas = assetBundle.LoadAsset<SpriteAtlas>( iconData.SpriteSheetName );

	        if (spriteAtlas != null)
	        {
	            return spriteAtlas.GetSprite(iconData.SpriteName);
	        }
	    }
        
#else
        if (assetBundle.Contains( iconData.SpriteSheetName ))
        {
            Sprite[] spriteSheetAssets = assetBundle.LoadAssetWithSubAssets<Sprite>( iconData.SpriteSheetName );

            for (int i = 0; i < spriteSheetAssets.Length; i++)
            {
                if ( spriteSheetAssets[i].name.Equals(iconData.SpriteName) )
                {
                    return spriteSheetAssets[i];
                }
            }
        }
    
#endif

	    return null;
	}
}