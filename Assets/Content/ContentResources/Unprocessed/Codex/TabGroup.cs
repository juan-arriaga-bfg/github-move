using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Transform tabsHost;
    
    private List<Tab> tabs = new List<Tab>();

    private int activeIndex = - 1;

    public void AddTab(Tab tab, int tabIndex)
    {       
        tab.transform.SetParent(tabsHost, false);
        tabs.Add(tab);
        tab.Index = tabIndex;
        tab.TabGroup = this;
    }
    
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
            tab.Toggle(i == index);
        }
    }
    
    public void Click(int index)
    {
         ActivateTab(index);
    }

    public void RemoveAllTabs()
    {
        tabs.Clear();
        activeIndex = -1;
    }
}