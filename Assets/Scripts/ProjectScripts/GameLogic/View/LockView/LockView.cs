using UnityEngine;
using UnityEngine.Rendering;

public class LockView : BoardElementView
{
    [SerializeField] private NSText label;
    
    [SerializeField] private Transform levelLock;
    [SerializeField] private Transform heroLock;
    
    [SerializeField] private SpriteRenderer levelLockBack;
    [SerializeField] private SpriteRenderer heroLockBack;
    [SerializeField] private SpriteRenderer levelLockUp;
    [SerializeField] private SpriteRenderer heroLockUp;
    
    [SerializeField] private SpriteRenderer heroBack;
    [SerializeField] private SpriteRenderer icon;
    
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private Transform hintArrowTarget;
    
    [SerializeField] private GameObject levelCheck;
    [SerializeField] private GameObject heroCheck;
    
    [SerializeField] private Transform levelCheckEar;
    [SerializeField] private Transform heroCheckEar;
    
    private Material defaultMaterial;

    public Transform GetHintTarget()
    {
        return hintArrowTarget;
    }

    public override void Init(BoardRenderer context)
    {
        base.Init(context);
        defaultMaterial = levelLockBack.material;
    }

    public void SetCondition(string level, string hero, bool isLevelUnlock, bool isHeroUnlock, bool isTwo)
    {
        if (string.IsNullOrEmpty(level) == false) label.Text = level;
        if (string.IsNullOrEmpty(hero) == false) icon.sprite = IconService.Current.GetSpriteById($"{hero}_Icon");
        
        heroLock.gameObject.SetActive(isTwo);
        
        levelCheck.SetActive(isLevelUnlock);
        heroCheck.SetActive(isHeroUnlock);

        levelCheckEar.rotation = Quaternion.Euler(0, 0, isLevelUnlock ? 17f : 0);
        heroCheckEar.rotation = Quaternion.Euler(0, 0, isHeroUnlock ? 17f : 0);
        
        levelLock.transform.localPosition = isTwo ? Vector3.right * 0.55f : Vector3.zero;
        heroLock.transform.localPosition = Vector3.left * 0.55f;
    }
    
    public void SetGrayscale(bool enabled)
    {
        if (enabled)
        {
            levelLockBack.material = grayscaleMaterial;
            heroLockBack.material = grayscaleMaterial;
            levelLockUp.material = grayscaleMaterial;
            heroLockUp.material = grayscaleMaterial;
            
            heroBack.material = grayscaleMaterial;
            icon.material = grayscaleMaterial;
            return;
        }
        
        levelLockBack.material = defaultMaterial;
        heroLockBack.material = defaultMaterial;
        levelLockUp.material = defaultMaterial;
        heroLockUp.material = defaultMaterial;
        
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