using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen: MonoBehaviour
{
    [SerializeField] private List<Image> splashes;
    [SerializeField] private float timeBeforeChange;
    [SerializeField] CanvasGroup canvasGroup;

    private int currentSplashIndex;

    private bool isNativeSplashHidden;
 
    private void LateUpdate()
    {
        if (isNativeSplashHidden)
        {
            return;
        }

        isNativeSplashHidden = true;
        
        StartCoroutine(HideNativeSplash());
    }
    
    private IEnumerator HideNativeSplash()
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("[SplashScreen] => HideNativeSplash");
        
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.custom.SplashDialog"))
        {
            ajc.CallStatic("Kill");
        } 
#endif
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Debug.Log("[SplashScreen] => Start");
        
        StartCoroutine(CheckLoadingState());
        
        var seq = DOTween.Sequence()
                         .SetId(this);

        for (var i = 0; i < splashes.Count; i++)
        {
            seq.AppendInterval(timeBeforeChange);

            if (i != splashes.Count - 1)
            {
                var prevSplash = splashes[i];
                var nextSplash = splashes[i + 1];

                seq.AppendCallback(() =>
                {
                    nextSplash.gameObject.SetActive(true);
                    nextSplash.color = new Color(1, 1, 1, 0);
                    nextSplash.DOFade(1, 0.3f);

                    prevSplash.DOFade(0, 0.3f);
                });
            }
        }
    }

    private IEnumerator CheckLoadingState()
    {
        yield return new WaitForSeconds(0.1f);
        
        var asyncInit = AsyncInitService.Current;
        
        if (asyncInit != null)
        {
            if (asyncInit.IsCompleted<ShowLoadingWindowInitComponent>())
            {
                HideAnimated();
            }
        }
        else
        {
            StartCoroutine(CheckLoadingState());
        }
    }

    private void HideAnimated()
    {
        Debug.Log("[SplashScreen] => HideAnimated");
        
        DOTween.Kill(this);
        canvasGroup.DOFade(0, 0.15f)
                   .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
    }
}