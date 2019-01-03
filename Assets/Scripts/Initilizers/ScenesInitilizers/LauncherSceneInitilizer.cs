using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherSceneInitilizer : SceneInitializer<DefaultApplicationInitilizer>
{
	protected override void InitScene(ApplicationInitializer applicationInitializer, Action onComplete)
	{
		base.InitScene(applicationInitializer, onComplete);

		Application.targetFrameRate = 60;

		InitAsync();
		
		// set resource deliverer for UI
		IWUISettings.Instance.SetResourceManager(new DefaultUIResourceManager());

		UIService.Get.ShowWindowInstantly(UIWindowType.LauncherWindow);
	}

	private void InitAsync()
	{
         AsyncInitManager asyncInitManager = new AsyncInitManager();
         AsyncInitService.Instance.SetManager(asyncInitManager);
         asyncInitManager
	        .RegisterComponent(new BfgSdkUnityMessageHandlerInitComponent(), true)
	        .RegisterComponent(new BfgSdkGdprInitComponent(),                true)
	        .RegisterComponent(new BfgSdkBrandingInitComponent(),            true)
	        .RegisterComponent(new MainSceneLoaderComponent(),               true);

	}
}