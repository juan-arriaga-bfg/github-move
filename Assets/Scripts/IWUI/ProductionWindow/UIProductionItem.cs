using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductionItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject pattern;
    [SerializeField] private HorizontalLayoutGroup group;

    private List<ProductionViewItem> items = new List<ProductionViewItem>();

    private RectTransform body;
    private ProductionComponent production;

    public void Init(ProductionComponent production)
    {
        this.production = production;
        body = GetComponent<RectTransform>();
        
        icon.sprite = IconService.Current.GetSpriteById(production.Target);
        button.SetActive(production.State == ProductionState.Full);
        
        while (items.Count > production.Storage.Count)
        {
            var i = items.Count - 1;
            var item = items[i];
            
            items.RemoveAt(i);
            Destroy(item.gameObject);
        }
        
        var index = 0;
        
        foreach (var pair in production.Storage)
        {
            if (index == items.Count)
            {
                pattern.SetActive(true);
                var item = Instantiate(pattern, pattern.transform.parent).GetComponent<ProductionViewItem>();
                items.Add(item);
            }
            
            items[index].Init(pair.Key, pair.Value.Value, pair.Value.Key, 26);
            index++;
        }
        
        pattern.SetActive(false);
        group.spacing = items.Count == 2 ? 20 : 5;
    }

    public bool Check(int resource, Vector3 pos)
    {
        var v = new Vector3[4];
        body.GetWorldCorners(v);
        
        var rect = new Rect(v[0], v[2]-v[0]);
        
        return rect.Contains(pos) && production.AddViaUI(resource);
    }

    public void OnClick()
    {
        if(production.State != ProductionState.Full) return;
        
        production.Change();
        production.Start();
    }
}