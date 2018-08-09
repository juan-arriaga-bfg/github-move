using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexItemDropPanel : MonoBehaviour
{
    [SerializeField] private Image pieceIco;
    [SerializeField] private TextMeshProUGUI countLabel;
    [SerializeField] private GameObject energyPanel;
    [SerializeField] private GameObject reproductionPanel;

    public void Init(CodexItemDef itemDef)
    {
        energyPanel.SetActive(false);
        reproductionPanel.SetActive(false);
        
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
        
        energyPanel.SetActive(true);

        int energyCount = itemDef.PieceDef.SpawnResources.Amount;
        countLabel.text = energyCount.ToString();
        countLabel.gameObject.SetActive(true);
    }
    
    private void RenderReproduction(CodexItemDef itemDef)
    {
        gameObject.SetActive(true);
        
        reproductionPanel.gameObject.SetActive(true);

        var currency = itemDef.PieceDef.Reproduction.Currency;
        Sprite sprite = IconService.Current.GetSpriteById(currency);
        pieceIco.sprite = sprite;
    }
}