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

    private void Update()
    {
        if (isNativeSplashHidden)
        {
            return;
        }

        isNativeSplashHidden = true;
        
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
        var seq = DOTween.Sequence();

        for (var i = 0; i < splashes.Count; i++)
        {
            seq.AppendInterval(timeBeforeChange);

            if (i == splashes.Count - 1)
            {
                ScheduleEnd(seq);
            }
            else
            {
                var prevSplash = splashes[i];
                var nextSplash = splashes[i + 1];
                
                seq.AppendCallback(() =>
                {
                    nextSplash.gameObject.SetActive(true);
                    nextSplash.color = new Color(1,1,1,0);
                    nextSplash.DOFade(1, 0.3f);
                    
                    prevSplash.DOFade(0, 0.3f);
                });
            }
        }
    }

    private void ScheduleEnd(Sequence seq)
    {
        seq.AppendCallback(() =>
        {
            canvasGroup.DOFade(0, 0.3f)
                       .OnComplete(() =>
                        {
                            Destroy(gameObject);
                        });
        });
    }
}