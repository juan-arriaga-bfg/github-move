using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverrideSortingGroup : MonoBehaviour
{
    [SerializeField] private SortingGroup cachedSortingGroup;
    
    [SerializeField] private int targetLayer;
    
    public virtual void SyncRendererLayers(BoardElementView elementView, BoardPosition position)
    {
        cachedSortingGroup.sortingOrder = elementView.GetLayerIndexBy(new BoardPosition(position.X, position.Y, targetLayer));
    }
}