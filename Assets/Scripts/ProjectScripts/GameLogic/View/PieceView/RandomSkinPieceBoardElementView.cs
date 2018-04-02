using UnityEngine;

public class RandomSkinPieceBoardElementView : PieceBoardElementView
{
    [SerializeField] private SpriteRenderer skin;

    [SerializeField] private string defaultSkinName;
    
    [SerializeField] private int minIndex;
    [SerializeField] private int maxIndex;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        skin.sprite = IconService.Current.GetSpriteById(string.Format("{0}_{1}", defaultSkinName, Random.Range(minIndex, maxIndex)));
    }
}