using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexItemDropPanel : MonoBehaviour
{
    [SerializeField] private Image recycleIco;
    [SerializeField] private Image pieceIco;
    [SerializeField] private TextMeshProUGUI countLabel;

    public void Init(CodexItemDef itemDef)
    {
        if (itemDef.PieceTypeDef.Filter.Has(PieceTypeFilter.Energy))
        {
            RenderEnergy(itemDef);
        }
        else if (itemDef.PieceTypeDef.Filter.Has(PieceTypeFilter.Reproduction))
        {
            RenderReproduction(itemDef); 
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void RenderEnergy(CodexItemDef itemDef)
    {
        gameObject.SetActive(true);
        
        pieceIco.gameObject.SetActive(true);
        recycleIco.gameObject.SetActive(false);
        countLabel.gameObject.SetActive(true);

        int energyCount = itemDef.PieceDef.SpawnResources.Amount;
        countLabel.text = energyCount.ToString();
    }
    
    private void RenderReproduction(CodexItemDef itemDef)
    {
        gameObject.SetActive(true);
        
        pieceIco.gameObject.SetActive(true);
        recycleIco.gameObject.SetActive(true);
        countLabel.gameObject.SetActive(false);
        
        var currency = itemDef.PieceDef.Reproduction.Currency;
        Sprite sprite = IconService.Current.GetSpriteById(currency);
        pieceIco.sprite = sprite;
    }
}