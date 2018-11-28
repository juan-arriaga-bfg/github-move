using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontStylesTester : MonoBehaviour
{
    [SerializeField] private Transform host;
    [SerializeField] private NSText prefab;

    private void Start()
    {
        Refresh();
    }
    
    [ContextMenu("Refresh")]
    public void Refresh()
    {
        var childsCount = host.childCount;
        for (int i = childsCount - 1; i >= 0 ; i--)
        {
            var obj = host.GetChild(i);
            if (obj.transform == prefab.transform)
            {
                continue;
            }
            
            Destroy(obj.gameObject);
        }
        
        prefab.gameObject.SetActive(true);


        foreach (var style in FontsSettings.Instance.Styles)
        {
            NSText text = Instantiate(prefab.gameObject, prefab.transform.parent).GetComponent<NSText>();
            text.StyleId = style.Id;
            text.ApplyStyle();

            text.gameObject.name = $"[style '{style.NameId}' id {style.Id}]";
            text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text.gameObject.name;
        }
        
        prefab.gameObject.SetActive(false);
    }
}
