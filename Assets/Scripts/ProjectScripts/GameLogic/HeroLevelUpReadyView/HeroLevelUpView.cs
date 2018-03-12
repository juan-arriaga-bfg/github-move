using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLevelUpView : IWBaseMonoBehaviour 
{
    [SerializeField] private Transform anchorView;
	
    private PieceBoardElementView context;

    private Transform arrowTransform;
    
    private void Awake()
    {
        context = GetComponent<PieceBoardElementView>();

        foreach (Transform view in anchorView)
        {
            DestroyImmediate(view.gameObject);
        }

        var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.ReadyArrowView));
        arrowTransform = go.GetComponent<Transform>();

        arrowTransform.SetParent(anchorView);
        arrowTransform.localPosition = Vector3.zero;
        arrowTransform.localRotation = Quaternion.identity;
        arrowTransform.localScale = Vector3.one;

        context.ClearCacheLayers();

    }
    
    private void Update()
    {
        if (context == null || context.Piece == null) return;

        var hero = GameDataService.Current.HeroesManager.GetHero("Robin");
        var level = GameDataService.Current.HeroesManager.HeroLevel;
        
        var isDone = ProfileService.Current.GetStorageItem(Currency.RobinCards.Name).Amount >= hero.Prices[level].Amount;

        if (isDone)
        {
            arrowTransform.gameObject.SetActive(true);
        }
        else
        {
            arrowTransform.gameObject.SetActive(false);
        }
    }
}
