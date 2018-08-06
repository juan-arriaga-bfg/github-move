using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<Tab> tabs;

    private int activeIndex = - 1;

    public void ActivateTab(int index)
    {
        if (activeIndex == index)
        {
            return;
        }

        activeIndex = index;
        
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
    
    public void Click(int index)
    {
         ActivateTab(index);
    }
}