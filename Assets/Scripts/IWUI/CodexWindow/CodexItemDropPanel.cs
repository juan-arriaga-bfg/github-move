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
        gameObject.SetActive(true);
        
        energyPanel.SetActive(false);
        reproductionPanel.SetActive(false);
        
        /*if (itemDef.PieceTypeDef.Filter.Has(PieceTypeFilter.Energy))
        {
            RenderEnergy(itemDef);
        }
        else */if (itemDef.PieceTypeDef.Filter.Has(PieceTypeFilter.Reproduction))
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
        int energyCount = itemDef.PieceDef?.SpawnResources?.Amount ?? 0;
        if (energyCount <= 0)
        {
            Debug.LogError($"[CodexItemDropPanel] => RenderEnergy: energyCount <= 0 for {itemDef.PieceTypeDef.Abbreviations[0]}");
            return;    
        }
        
        countLabel.text = energyCount.ToString();
        countLabel.gameObject.SetActive(true);
        
        energyPanel.SetActive(true);
    }
    
    private void RenderReproduction(CodexItemDef itemDef)
    {
        reproductionPanel.gameObject.SetActive(true);

        var currency = itemDef.PieceDef.ReproductionDef.Reproduction.Currency;
        Sprite sprite = IconService.Current.GetSpriteById(currency);
        pieceIco.sprite = sprite;
    }
}