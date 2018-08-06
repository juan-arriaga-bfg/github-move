using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<Tab> tabs;

    private int activeIndex;
    
    public void Click(int index)
    {
        if (activeIndex == index)
        {
            return;
        }
        
        for (int i = 0; i < tabs.Count; i++)
        {
            var tab = tabs[i];
            if (tab == null)
            {
                continue;
            }
            
            tab.Toggle(i == index);
        }
    }
}