using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIProductionWindowView : IWUIWindowView
{
    [SerializeField] private RectTransform body;
    [SerializeField] private GameObject pattern;
    [SerializeField] private LayoutElement border;
    [SerializeField] private int index = 3;

    private const int open = -85;
    private const int close = 115;
    
    private bool isOpen;
    
    private List<UIProductionItem> items = new List<UIProductionItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        body.DOAnchorPosX(close, 0f).SetId(body);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIProductionWindowModel windowModel = Model as UIProductionWindowModel;
    }

    public void UpdateList()
    {
        var windowModel = Model as UIProductionWindowModel;

        var productions = windowModel.Productions;
        
        productions.Sort((a, b) => -a.CompareTo(b));
        
        while (items.Count > productions.Count)
        {
            var i = items.Count - 1;
            var item = items[i];
            
            items.RemoveAt(i);
            Destroy(item.gameObject);
        }
        
        for (var i = 0; i < productions.Count; i++)
        {
            if (i == items.Count)
            {
                pattern.SetActive(true);
                var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UIProductionItem>();
                items.Add(item);
            }
            
            items[i].Init(productions[i]);
        }
        
        pattern.SetActive(false);
        border.preferredHeight = items.Count > 4 ? 970 : -1;
    }
    
    public void OnClick()
    {
        Change(!isOpen);
    }

    public void Change(bool isShow)
    {
        DOTween.Kill(body, true);
        
        isOpen = isShow;
        
        if(isOpen) UpdateList();

        body.DOAnchorPosX(isOpen ? open : close, 0.3f).SetId(body).SetEase(Ease.OutBack);
    }

    public bool Drop(int resource, Vector3 pos)
    {
        var v = new Vector3[4];
        body.GetWorldCorners(v);
        
        var rect = new Rect(v[0], v[2]-v[0]);
        
        if(rect.Contains(pos) == false) return false;
        
        foreach (var item in items)
        {
            if(item.Check(resource, pos)) return true;
        }
        
        return false;
    }
}

