using UnityEngine;
using UnityEngine.UI;

public class ProductionViewItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText amount;
    [SerializeField] private GameObject check;
    
    public void Init(int piece, int current, int target)
    {
        var isComplete = current == target;
        
        icon.sprite = IconService.Current.GetSpriteById(PieceType.Parse(piece));
        
        amount.Text = string.Format("<color=#{0}><size=40>{1}</size></color>/{2}", isComplete ? "FFFFFF" : "FE4704", current, target);
        
        amount.gameObject.SetActive(!isComplete);
        check.SetActive(isComplete);
    }
}