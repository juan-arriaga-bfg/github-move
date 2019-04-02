using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineProgressView : UIBoardView
{
    [SerializeField] private List<Image> stars;
    
    protected override ViewType Id => ViewType.MineProgress;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
		
        Priority = defaultPriority = -1;

        var def = GameDataService.Current.PiecesManager.GetPieceDef(Context.PieceType).MineDef;
        var data = GameDataService.Current.PiecesManager.GetComponent<PiecesMineDataManager>(PiecesMineDataManager.ComponentGuid);
        var loop = data.GetCurrentLoop(Context.PieceType);
        var index = Context.Context.BoardLogic.MatchDefinition.GetIndexInChain(Context.PieceType) - 1;
        
        Debug.LogFormat("[MineProgress] => id: '{0}', star: {1}, loop: {2}/{3}", PieceType.Parse(Context.PieceType), index, def.Loop - loop, def.Loop);

        for (var i = 0; i < stars.Count; i++)
        {
            var star = stars[i];
            var value = 0f;

            if (def.Loop < 0)
            {
                star.fillAmount = 1;
                continue;
            }

            if (i < index) value = def.Loop;
            else if (i == index) value = (def.Loop - loop) / (float) def.Loop;
            
            star.fillAmount = value;
        }
    }
}