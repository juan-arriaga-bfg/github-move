using System;
using Debug = IW.Logger;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiledLayoutDef
{
    public int Width;
    public int Height;
    public string Data;
}

public class FieldDataManager : IECSComponent, IDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    /// <summary>
    /// Include island and board pieces
    /// </summary>
    public Dictionary<int, List<BoardPosition>> AllPieces;

    public Dictionary<int, List<BoardPosition>> IslandPieces;
    public Dictionary<int, List<BoardPosition>> BoardPieces;
    
    public int LayoutW;
    public int LayoutH;
    public int[] LayoutData;
    public List<BoardPosition> LockedCells;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {
        AllPieces = new Dictionary<int, List<BoardPosition>>();
        IslandPieces = new Dictionary<int, List<BoardPosition>>();
        BoardPieces = new Dictionary<int, List<BoardPosition>>();

        LoadContentData(new HybridConfigDataMapper<Dictionary<string, List<BoardPosition>>>("configs/field.data", NSConfigsSettings.Instance.IsUseEncryption));
        LoadLayoutData(new HybridConfigDataMapper<FiledLayoutDef>("configs/layout.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadContentData(IDataMapper<Dictionary<string, List<BoardPosition>>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                foreach (var pair in data)
                {
                    var pieceType = PieceType.Parse(pair.Key);
                    AllPieces.Add(pieceType, pair.Value);
                    
                    var islandPiecePositions = new List<BoardPosition>();
                    var boardPiecePositions = new List<BoardPosition>();
                    var islandPositions = VIPIslandLogicComponent.IslandPositions;
                    foreach (var pos in pair.Value)
                    {
                        if (islandPositions.Contains(pos))
                        {
                            islandPiecePositions.Add(pos);
                        }
                        else
                        {
                            boardPiecePositions.Add(pos);
                        }
                    }

                    if (islandPiecePositions.Count > 0)
                    {
                        IslandPieces.Add(pieceType, islandPiecePositions);
                    }

                    if (boardPiecePositions.Count > 0)
                    {
                        BoardPieces.Add(pieceType, boardPiecePositions);
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
    
    public void LoadLayoutData(IDataMapper<FiledLayoutDef> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                LockedCells = new List<BoardPosition>();
                
                LayoutW = data.Width;
                LayoutH = data.Height;

                int size = LayoutW * LayoutH;

                string[] idsStr = data.Data.Split(',');
                
                if (size != idsStr.Length)
                {
                    Debug.LogError("[FieldDataManager] => LoadLayoutData: LayoutW * LayoutH != Data.Length"); 
                    return;
                }
                
                LayoutData = new int[size];
                int len = idsStr.Length;
                for (int i = 0; i < len; i++)
                {
                    // Working code to get x/y coords if needed
                    // int x = i / LayoutW;
                    // int y = i % LayoutW;
                    
                    var tileId = int.Parse(idsStr[i]);
                    LayoutData[i] = tileId;
                }
            }
            else
            {
                Debug.LogError("[FieldDataManager] => LoadLayoutData: config not loaded");
            }
        });
    }

    public int GetTileId(int x, int y)
    {
        int index = x * LayoutW + y;
        if (index < 0 || index >= LayoutData.Length)
        {
            return -1;
        }
        
        return LayoutData[x * LayoutW + y];
    }
}