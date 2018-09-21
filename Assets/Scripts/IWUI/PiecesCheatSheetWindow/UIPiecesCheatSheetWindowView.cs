using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPiecesCheatSheetWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private UIPiecesCheatSheetWindowItem itemPrefab;
    
    private List<UIPiecesCheatSheetWindowItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;

        SetTitle(model.Title);
        CreateItems(model);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;
        
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
        
    }
    
    private void CreateItems(UIPiecesCheatSheetWindowModel model)
    {
        var exclude = new HashSet<int>
        {
            0, 
            -1,
            1
        };
        
        var matchDef = BoardService.Current.GetBoardById(0).BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        
        itemPrefab.gameObject.SetActive(true);
        
        items = new List<UIPiecesCheatSheetWindowItem>();
        var piecesList = model.PiecesList();

        int prevChain = -1;
        
        foreach (var pieceId in piecesList)
        {
            if (exclude.Contains(pieceId))
            {
                continue;
            }
            
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(itemPrefab.transform.parent, false);

            var chainDef = matchDef.GetChain(pieceId);
            
            var chain = chainDef != null && chainDef.Count > 0 ? chainDef[0] : 0;
            // item.Init(chain == -1 || chain == prevChain ? pieceId : 0 ); 
            item.Init(pieceId);
            items.Add(item);

            prevChain = chain;
        }

        itemPrefab.gameObject.SetActive(false);
    }
}
