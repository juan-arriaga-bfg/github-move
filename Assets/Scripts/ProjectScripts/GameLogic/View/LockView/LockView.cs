using UnityEngine;
using UnityEngine.Rendering;

public class LockView : BoardElementView
{
    [SerializeField] private NSText label;
    
    [SerializeField] private SpriteRenderer levelLock;
    [SerializeField] private SpriteRenderer heroLock;
    
    [SerializeField] private SpriteRenderer heroBack;
    [SerializeField] private SpriteRenderer icon;
    
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private Transform hintArrowTarget;
    
    private Material defaultMaterial;

    public Transform GetHintTarget()
    {
        return hintArrowTarget;
    }

    public override void Init(BoardRenderer context)
    {
        base.Init(context);
        defaultMaterial = levelLock.material;
    }

    public void SetCondition(string level, string hero)
    {
        var isLevelUnlock = string.IsNullOrEmpty(level) == false;
        var isHeroUnlock = string.IsNullOrEmpty(hero) == false;
        var isTwo = isLevelUnlock && isHeroUnlock;
        
        if(isLevelUnlock) label.Text = level;
        if(isHeroUnlock) icon.sprite = IconService.Current.GetSpriteById($"{hero}_Icon");
        
        levelLock.gameObject.SetActive(isLevelUnlock);
        heroLock.gameObject.SetActive(isHeroUnlock);
        
        levelLock.transform.localPosition = isTwo ? Vector3.right * 0.55f : Vector3.zero;
        heroLock.transform.localPosition = isTwo ? Vector3.left * 0.55f : Vector3.zero;
    }
    
    public void SetGrayscale(bool enabled)
    {
        if (enabled)
        {
            levelLock.material = grayscaleMaterial;
            heroLock.material = grayscaleMaterial;
            
            heroBack.material = grayscaleMaterial;
            icon.material = grayscaleMaterial;
            return;
        }
        
        levelLock.material = defaultMaterial;
        heroLock.material = defaultMaterial;
        
        heroBack.material = defaultMaterial;
        icon.material = defaultMaterial;
    }

    public void SetSortingOrder(BoardPosition position)
    {
        var cachedSpriteSortingGroup = GetComponent<SortingGroup>();
        
        if (cachedSpriteSortingGroup == null) cachedSpriteSortingGroup = gameObject.AddComponent<SortingGroup>();
        
        cachedSpriteSortingGroup.sortingOrder = GetLayerIndexBy(new BoardPosition(position.X, position.Y - 1, BoardLayer.Piece.Layer));
    }
}