using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPiecesCheatSheetWindowItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lblName;
    [SerializeField] private TextMeshProUGUI lblId;
    [SerializeField] private Image ico;
    [SerializeField] private CanvasGroup mainCanvasGroup;

    private int pieceId;
    
    public void Init(int pieceId)
    {
        this.pieceId = pieceId;

        if (pieceId <= 0)
        {
            lblName.gameObject.SetActive(false);
            ico.gameObject.SetActive(false);
            return;
        }
        
        // var pieceManager = GameDataService.Current.PiecesManager;
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);

        lblName.text = pieceTypeDef.Abbreviations[0];
        lblId.text = $"id {pieceId}";

        var spr = GetPieecSprite();
        if (spr != null)
        {
            ico.sprite = spr;
        }
    }
    
    private Sprite GetPieecSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(pieceId));
    }

    public void OnClick()
    {
        // var camPos = Camera.main.transform.position;
        // BoardPosition bp = BoardService.Current.GetBoardById(0).BoardDef.GetSectorPosition(camPos);
        // Debug.Log($"{bp}");
        BoardPosition pos = new BoardPosition(23, 10, BoardService.Current.GetBoardById(0).BoardDef.PieceLayer);
        var availablePoints = new List<BoardPosition>();

        BoardService.Current.GetBoardById(0).BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(pos, availablePoints, 1, 100);
        if (availablePoints.Count > 0)
        {
            DOTween.Sequence()
                   .Append(mainCanvasGroup.DOFade(0f, 0.1f))
                   .AppendInterval(0.3f)
                   .AppendCallback(() =>
                    {
                        BoardService.Current.GetBoardById(0).ActionExecutor.AddAction(new CreatePieceAtAction
                        {
                            At = availablePoints[0],
                            PieceTypeId = pieceId
                        });
                    })
                   .AppendInterval(0.6f)
                   .Append(mainCanvasGroup.DOFade(1f, 0.1f));

        }
    }
}