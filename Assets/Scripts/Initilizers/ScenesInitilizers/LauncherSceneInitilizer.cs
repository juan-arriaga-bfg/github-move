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
	}
}