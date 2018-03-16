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
        
        var hero = GameDataService.Current.HeroesManager.GetHero(context.Piece.PieceType == PieceType.H1.Id ? "Robin" : "John");

        if (hero == null) return;
        
        arrowTransform.gameObject.SetActive(hero.CurrentProgress >= hero.TotalProgress);
    }
}
