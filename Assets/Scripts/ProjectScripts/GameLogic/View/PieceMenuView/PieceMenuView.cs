using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PieceMenuView : MonoBehaviour, IBoardEventListener
{
    [SerializeField] private Transform anchorView;

    private Piece cachedPiece;
    
    private BoardEventsComponent eventBus;
    private TouchReactionDefinitionMenu menuDef;

    private List<Transform> buttons;
    
    private void OnEnable()
    {
        if (BoardService.Current.GetBoardById(0) == null) return;
        
        eventBus = BoardService.Current.GetBoardById(0).BoardEvents;
    }

    private void OnDisable()
    {
        if(eventBus == null) return;
        
        eventBus.RemoveListener(this, GameEventsCodes.OpenPieceMenu);
        eventBus.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
    }

    private void Update()
    {
        var context = GetComponent<PieceBoardElementView>();
        
        if (context == null || context.Piece == null) return;

        if (menuDef != null) return;

        cachedPiece = context.Piece;
        
        var touchReaction = context.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(touchReaction == null) return;
        
        menuDef = touchReaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menuDef == null) return;
            
        eventBus.AddListener(this, GameEventsCodes.ClosePieceMenu);
        eventBus.AddListener(this, GameEventsCodes.OpenPieceMenu);
    }

    public void OnBoardEvent(int code, object context)
    {
        if (code == GameEventsCodes.OpenPieceMenu)
        {
            OpenMenu(context as Piece);
            return;
        }

        CloseMenu(context as string);
    }

    private void OpenMenu(Piece piece)
    {
        if (piece.CachedPosition.Equals(cachedPiece.CachedPosition) == false)
        {
            CloseMenu("");
            return;
        }

        var angle = (360 / (float) menuDef.Definitions.Count) * (Mathf.PI/180);
        
        buttons = new List<Transform>();

        var keys = new List<string>();

        foreach (var pair in menuDef.Definitions)
        {
            keys.Add(pair.Key);
        }

        for (int i = 0; i < keys.Count; i++)
        {
            var position = new Vector2(Mathf.Cos(angle  * i), Mathf.Sin(angle * i));
            var button = CreateObject<BoardButtonView>(R.BoardButtonView, position);
            
            button.Init(keys[i]);
            buttons.Add(button.transform);
        }
    }

    private void CloseMenu(string key)
    {
        if(buttons == null) return;
        
        foreach (var button in buttons)
        {
            Destroy(button.gameObject, 0.1f);
        }
        
        buttons = new List<Transform>();
    }
    
    private T CreateObject<T>(string prefab, Vector2 ofset) where T : IWBaseMonoBehaviour
    {
        var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(prefab));
        var view = go.GetComponent<T>();

        view.CachedTransform.SetParent(anchorView);
        view.CachedTransform.localPosition = ofset;
        view.CachedTransform.localRotation = Quaternion.identity;
        view.CachedTransform.localScale = Vector3.one;
        
        return view;
    }
}