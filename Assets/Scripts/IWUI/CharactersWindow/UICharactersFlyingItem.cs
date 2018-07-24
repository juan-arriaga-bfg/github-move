using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharactersFlyingItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText nameLabel;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform rect;

    public void Fly(Hero hero)
    {
        icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head", hero.Def.Uid));

        nameLabel.Text = hero.Def.Uid;
        
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);
        
        gameObject.SetActive(true);
        rect.localScale = Vector3.one;
        rect.position = model.From;
        group.alpha = 1;
        
        DOTween.Kill(rect);
        
        DOTween.Sequence().SetId(rect)
            .Insert(0, rect.DOAnchorPos(model.To + Vector2.up * 300, 0.5f))
            .Insert(0, rect.DOScale(Vector3.one * 0.3f, 0.5f))
            .Insert(0.3f, group.DOFade(0, 0.2f))
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        DOTween.Kill(rect);
    }
}