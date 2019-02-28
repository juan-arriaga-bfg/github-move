using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardProgressbar : MonoBehaviour
{
    [SerializeField] private Image progress;
    [SerializeField] private Image light;
    
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject dot;
    [SerializeField] private RectTransform sizeRect;

    [SerializeField] private bool illumination;
    [SerializeField] private bool dividing;

    private List<GameObject> dots = new List<GameObject>();
    
    public bool IsVisible
    {
        set { body.SetActive(value); }
    }

    public void Init(int max)
    {
        if(body.activeSelf == false) return;
        
        if (dividing)
        {
            for (var i = 1; i < max; i++)
            {
                var dt = Instantiate(dot, dot.transform.parent);
                dots.Add(dt);
            }
            
            layoutGroup.spacing = sizeRect.sizeDelta.x / max;
        
            dot.SetActive(false);
        }
        
        progress.gameObject.SetActive(illumination);
        
        if (!illumination) return;
        
        DOTween.Kill(light);

        DOTween.Sequence().SetId(light).SetLoops(int.MaxValue)
            .Append(light.DOFade(0.5f, 0.3f))
            .Append(light.DOFade(1f, 0.3f));
    }

    public void UpdateValue(float current, float next)
    {
        if(body.activeSelf == false) return;
        
        light.fillAmount = current;
        progress.fillAmount = next;
    }

    public void Clear()
    {
        if(body.activeSelf == false) return;
        
        if (dividing)
        {
            foreach (var dt in dots)
            {
                Destroy(dt);
            }
        
            dots = new List<GameObject>();
            dot.SetActive(true);
        }        
        
        if (!illumination) return;
        
        DOTween.Kill(light);
        light.DOFade(1f, 0f);
    }

    private void OnDisable()
    {
        DOTween.Kill(light); 
    }
}