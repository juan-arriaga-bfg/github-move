using UnityEngine;

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

		return null;
	}
}