using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLevelUpView : IWBaseMonoBehaviour 
{
    [SerializeField] private Transform anchorView;
	
    private PieceBoardElementView context;

    private Transform arrowTransform;
    
    private TouchReactionConditionHeroLevelUp touchReactionConditionHeroLevelUp;

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

        if (touchReactionConditionHeroLevelUp == null)
        {
            var touchReactionComponent = context.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
            touchReactionConditionHeroLevelUp =
                touchReactionComponent.GetComponent<TouchReactionConditionHeroLevelUp>(TouchReactionConditionHeroLevelUp
                    .ComponentGuid);
        }
		
        if (touchReactionConditionHeroLevelUp == null) return;

        if (touchReactionConditionHeroLevelUp.Check(BoardPosition.Default(), context.Piece))
        {
            arrowTransform.gameObject.SetActive(true);
        }
        else
        {
            arrowTransform.gameObject.SetActive(false);
        }
    }
}
