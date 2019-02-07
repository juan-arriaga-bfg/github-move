using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGoIfDebug : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
#if DEBUG
    
    private void Start()
    {
        target.SetActive(true);
    }
    
#endif
}