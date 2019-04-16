using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrystalPieceView : PieceBoardElementView
{
    private const string Simple = "Simple";
    private const string Production = "Production";
    private const string Chest = "Chest";
    private const string Character = "Character";
    private const string Hard = "Hard";
    private const string Mana = "Mana";
    private const string Soft = "Soft";
    private const string Extended = "Extended";
    
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    
    private CrystalPieceBoardObserver observer;
    private List<Piece> bestMatchPieces;
    
    private readonly List<int> ignoreIds = new List<int>
    {
        PieceType.Fog.Id,
        PieceType.Boost_CR1.Id,
        PieceType.Boost_CR2.Id,
        PieceType.Boost_CR3.Id,
        PieceType.Boost_CR.Id,
        PieceType.Boost_WR.Id,
        PieceType.CH_Free.Id,
        PieceType.CH_NPC.Id,
    };
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
        
        observer = Piece.GetComponent<CrystalPieceBoardObserver>(CrystalPieceBoardObserver.ComponentGuid);
        
        var ignoreFilters = new List<PieceTypeFilter>
        {
            PieceTypeFilter.Character,
            PieceTypeFilter.Mine,
            PieceTypeFilter.Obstacle,
            PieceTypeFilter.Fake,
            PieceTypeFilter.Ingredient,
            PieceTypeFilter.ProductionField,
            PieceTypeFilter.OrderPiece,
            PieceTypeFilter.Multicellular,
        };

        foreach (var filter in ignoreFilters)
        {
            var ids = PieceType.GetIdsByFilter(filter);

            foreach (var id in ids)
            {
                if (ignoreIds.Contains(id)) continue;
                
                ignoreIds.Add(id);
            }
        }
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        DOTween.Kill(AnimationId);
    }

    private void OnDisable()
    {
        DOTween.Kill(AnimationId);
    }
    
    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);

        NSAudioService.Current.Play(SoundId.CrystalDrag, true);
        Piece.Context.PartPiecesLogic.Remove(Piece.CachedPosition);

        if (Piece.Context.BoardLogic.CellHints.OnDragStartBoost(Piece.CachedPosition)) return;
        
        bestMatchPieces = FindBestMatches();
        
        foreach (var piece in bestMatchPieces)
        {
            if (piece.PieceType == PieceType.Boost_CR.Id || piece.ActorView == null) continue;
            HighlightMatchable(piece.ActorView, true);
        }
    }

    private void HighlightMatchable(PieceBoardElementView view, bool isOn)
    {
        view.ToggleHighlight(isOn);
        view.ToggleSelection(isOn);
    }

    private List<Piece> FindBestMatches()
    {
        var definition = Context.Context.BoardLogic.MatchDefinition;
        var positionsCache = Context.Context.BoardLogic.PositionsCache.Cache;
        var currentBestPieces = new List<Piece>();
        
        var cache = new Dictionary<string, Dictionary<int, List<BoardPosition>>>
        {
            {Simple,     new Dictionary<int, List<BoardPosition>>()}, // Simple: A .. Z
            {Production, new Dictionary<int, List<BoardPosition>>()}, // Reproduction: PR_A .. PR_Z
            {Chest,      new Dictionary<int, List<BoardPosition>>()}, // Chest: CH_A .. CH_Z, SK_PR
            {Character,  new Dictionary<int, List<BoardPosition>>()}, // Character: NPC_B .. NPC_Z
            {Hard,       new Dictionary<int, List<BoardPosition>>()}, // Hard
            {Mana,       new Dictionary<int, List<BoardPosition>>()}, // Mana
            {Soft,       new Dictionary<int, List<BoardPosition>>()}, // Soft
            {Extended,   new Dictionary<int, List<BoardPosition>>()}, // Extended: EXT_A .. EXT_Z
        };

        foreach (var pair in positionsCache)
        {
            var key = pair.Key;
            
            if (ignoreIds.Contains(key)
                || definition.GetFirst(key) == key
                || definition.GetLast(key) == key
                || definition.GetPieceCountForMatch(key) - 1 > pair.Value.Count) continue;

            var def = PieceType.GetDefById(key);

            if (def.Filter.Has(PieceTypeFilter.Normal))
            {
                cache[Simple].Add(key, pair.Value);
            }
            else if (def.Filter.Has(PieceTypeFilter.Chest))
            {
                cache[Chest].Add(key, pair.Value);
            }
            else if (key >= PieceType.Hard1.Id && key <= PieceType.Hard6.Id)
            {
                cache[Hard].Add(key, pair.Value);
            }
            else if (key >= PieceType.Mana1.Id && key <= PieceType.Mana6.Id)
            {
                cache[Mana].Add(key, pair.Value);
            }
            else if (key >= PieceType.Soft1.Id && key <= PieceType.Soft8.Id)
            {
                cache[Soft].Add(key, pair.Value);
            }
            else if (key >= PieceType.NPC_B1.Id && key <= PieceType.NPC_Z8.Id)
            {
                cache[Character].Add(key, pair.Value);
            }
            else if (key >= PieceType.EXT_E1.Id && key <= PieceType.EXT_Z9.Id)
            {
                cache[Extended].Add(key, pair.Value);
            }
            else if (key >= PieceType.PR_A1.Id && key <= PieceType.PR_Z5.Id)
            {
                cache[Production].Add(key, pair.Value);
            }
        }

        if (CheckBest(cache[Simple], 3, out var bestPosition, GameDataService.Current.CodexManager.IsPieceUnlocked) == false
            && CheckBest(cache[Production], 2, out bestPosition) == false
            && CheckBest(cache[Chest], 1, out bestPosition) == false
            && CheckBest(cache[Character], 1, out bestPosition) == false
            && CheckBest(cache[Hard], 1, out bestPosition) == false
            && CheckBest(cache[Mana], 1, out bestPosition) == false
            && CheckBest(cache[Soft], 3, out bestPosition) == false
            && CheckBest(cache[Extended], 3, out bestPosition) == false
            && CheckBest(cache[Simple], 3, out bestPosition) == false)
        {
            return new List<Piece>();
        }

        foreach (var position in bestPosition)
        {
            currentBestPieces.Add(Context.Context.BoardLogic.GetPieceAt(position));
        }
        
        return currentBestPieces;
    }

    private bool CheckBest(Dictionary<int, List<BoardPosition>> cache, int maxId, out List<BoardPosition> bestPosition, Func<int, bool> extraIgnoreCheck = null)
    {
        bestPosition = new List<BoardPosition>();
        
        if (cache.Count == 0) return false;
        
        var definition = Context.Context.BoardLogic.MatchDefinition;
        var ignoreBranch = new List<int>();
        
        var minBranch = int.MaxValue;
        var maxIndex = maxId;
        var minDistance = float.MaxValue;

        foreach (var pair in cache)
        {
            var branch = definition.GetLast(pair.Key);

            if (ignoreBranch.Contains(branch)) continue;

            if (minBranch < branch || extraIgnoreCheck != null && extraIgnoreCheck(branch))
            {
                ignoreBranch.Add(branch);
                continue;
            }
            
            var index = definition.GetIndexInChain(pair.Key);

            if (maxIndex > index) continue;
            
            var checkedPositions = new Dictionary<BoardPosition, bool>();

            foreach (var position in pair.Value)
            {
                if (checkedPositions.ContainsKey(position)) continue;
                
                var match = new List<BoardPosition>();
                
                if (CheckMatch(position, match) == false) continue;

                match.Remove(Piece.CachedPosition);
                
                foreach (var pos in match)
                {
                    checkedPositions.Add(pos, true);
                }

                if (minBranch != branch || maxIndex != index)
                {
                    minBranch = branch;
                    maxIndex = index;
                    bestPosition = match;
                }

                if (bestPosition.Count > match.Count) continue;
                
                if (bestPosition.Count == match.Count)
                {
                    var nearest = Piece.CachedPosition.GetImmediate(match)[0];
                    var distance = BoardPosition.SqrMagnitude(Piece.CachedPosition, nearest);

                    if (minDistance <= distance) continue;

                    minDistance = distance;
                }
                
                bestPosition = match;
            }
        }
        
        return bestPosition.Count > 0;
    }
    
    private bool CheckMatch(BoardPosition to, List<BoardPosition> positions)
    {
        if (FindPositions(to, positions, out var currentId) == false) return false;
        
        positions.Add(Piece.CachedPosition);

        return Context.Context.BoardLogic.MatchActionBuilder.CheckMatch(positions, currentId, to, out _)
               && positions.Count != SimpleMatchActionBuilder.AMOUNT_BONUS - 1
               && positions.Count % SimpleMatchActionBuilder.AMOUNT_BONUS != 2;
    }

    private bool FindPositions(BoardPosition point, List<BoardPosition> field, out int current)
    {
        if (!Context.Context.BoardLogic.FieldFinder.Find(point, field, out current, true))
            return false;

        var i = 0;
        
        while (i < field.Count)
        {
            if (IsValidPosition(field[i]) == false)
            {
                field.RemoveAt(i);
                continue;
            }

            i++;
        }
        
        return field.Count > 0;
    }

    private bool IsValidPosition(BoardPosition position)
    {
        var logic = Context.Context.BoardLogic;
        
        if (logic.IsLockedCell(position) == false) return true;
        
        var boardCell = logic.BoardCells[position.X, position.Y, position.Z];
        
        foreach (var locker in boardCell.Lockers)
        {
            if (locker is DragAndCheckMatchAction == false) return false;
        }

        return true;
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        Piece.Context.BoardLogic.CellHints.OnDragEnd();
        
        base.OnDragEnd(boardPos, worldPos);

        NSAudioService.Current.Stop(SoundId.CrystalDrag);
        
        EndBestMatch();
        CheckPartPieceMatch(boardPos);
    }

    private void EndBestMatch()
    {
        if (bestMatchPieces == null) return;
        
        foreach (var piece in bestMatchPieces)
        {
            if(piece.PieceType == PieceType.Boost_CR.Id || piece.ActorView == null) continue;
            
            HighlightMatchable(piece.ActorView, false);
        }

        bestMatchPieces = null;
    }

    private void CheckPartPieceMatch(BoardPosition boardPos)
    {
        var position = new BoardPosition(boardPos.X, boardPos.Y, BoardLayer.Piece.Layer);

        if (Piece.CachedPosition.Equals(position) == false) return;

        observer?.AddBubble(position, Piece.PieceType);
    }
}