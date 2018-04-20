using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    New,
    InProgress,
    Complete
}

public class Quest
{
    private QuestDef def;

    private QuestState state;

    private int cachedCurrentAmount;
    
    public Quest(QuestDef def)
    {
        this.def = def;
        WantedPiece = PieceType.Parse(def.Price.Currency);
    }
    
    public QuestDef Def
    {
        get { return def; }
    }
    
    public QuestState State
    {
        get
        {
            if (state == QuestState.Complete)
            {
                state = Check() ? QuestState.Complete : QuestState.InProgress;
            }
            else
            {
                state = Check() ? QuestState.Complete : state;
            }
            
            return state;
        }
        set { state = value; }
    }
    
    public int WantedPiece { get; private set; }
    
    public string WantedIcon
    {
        get { return PieceType.Parse(WantedPiece); }
    }
    
    public int TargetAmount
    {
        get { return def.Price.Amount; }
    }
    
    public int CurrentAmount
    {
        get
        {
//            var board = BoardService.Current.GetBoardById(0);
//        
//            if(board == null) return 0;
//
//            return board.BoardLogic.PositionsCache.GetCountByType(WantedPiece);

            return cachedCurrentAmount;
        }
    }

    public int GetPiecesAmountOnBoard()
    {
        var board = BoardService.Current.GetBoardById(0);
        
        if(board == null) return 0;

        return board.BoardLogic.PositionsCache.GetCountByType(WantedPiece);
    }

    public void SendToQuest(int amount)
    {   
        cachedCurrentAmount += amount;

        cachedCurrentAmount = Mathf.Clamp(cachedCurrentAmount, 0, TargetAmount);
        
        var board = BoardService.Current.GetBoardById(0);
        
        if(board == null) return;

        var piecePositions = board.BoardLogic.PositionsCache.GetPiecePositionsByType(WantedPiece);
        
        if (piecePositions.Count <= 0) return;
        
        piecePositions.Shuffle();

        for (int i = 0; i < amount; i++)
        {
            if (i >= piecePositions.Count) continue;

            var position = piecePositions[i];
            
            board.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = position,
                Positions = new List<BoardPosition>{position}
            });
        }
    }
    
    public CurrencyPair Reward
    {
        get { return def.Reward; }
    }

    public bool Check()
    {
        return CurrentAmount >= TargetAmount;
    }

    public void Complete(Action onComplete)
    {
        var board = BoardService.Current.GetBoardById(0);

        var positions = board.BoardLogic.PositionsCache.GetRandomPositions(WantedPiece, TargetAmount);

        for (var i = 0; i < positions.Count; i++)
        {
            var pos = positions[i];
            
            board.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = pos,
                Positions = new List<BoardPosition> {pos},
                OnComplete = i == 0 ? onComplete : null
            });
        }
    }
}