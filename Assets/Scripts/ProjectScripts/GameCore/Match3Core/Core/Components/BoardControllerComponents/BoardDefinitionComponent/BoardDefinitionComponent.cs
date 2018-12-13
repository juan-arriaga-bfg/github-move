using System.Collections.Generic;
using UnityEngine;

public class BoardDefinitionComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected BoardController context;

    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
        
        MaxPoit = new BoardPosition(Width, Height, Depth);
    }

    public void OnUnRegisterEntity(ECSEntity entity) { }

    public int Depth { get; set; }

    public int CellWidth { get; set; }

    public int CellHeight { get; set; }

    public float UnitSize { get; set; }

    public Camera ViewCamera { get; set; }

    public float GlobalPieceScale { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
    
    public Dictionary<int, int> LayersDef { get; set; }


    public Transform SectorsGridView { get; set; }

    public BoardPosition MaxPoit;

    public virtual Vector3 GetPiecePosition(int x, int y, int z = 0)
    {
        return GetSectorCenterWorldPosition(x, y, z);
    }

    /// <summary>
    /// return predefined Z position for layer type
    /// </summary>
    /// <param name="boardLayerType"></param>
    /// <returns></returns>
    public virtual int GetLayerFor(int boardLayerType)
    {
        int layer = 0;
        if (LayersDef != null && LayersDef.TryGetValue(boardLayerType, out layer))
        {
            return layer;
        }

        return layer;
    }
    
    public virtual Vector3 GetScreenPosition(int x, int y, int z = 0)
    {
        //Vector3 worldPosition = GetWorldPosition(x, y);

        //Vector3 screenPosition = ViewCamera.WorldToScreenPoint(worldPosition);

        return GetSectorCenterWorldPosition(x, y, z);
    }

    public virtual Vector3 GetLocalPosition(int x, int y)
    {
        Vector2 localPosition = GetPiecePosition(x, y);

        return localPosition;
    }

    public virtual Vector3 GetWorldPosition(int x, int y)
    {
        Vector2 localPosition = GetLocalPosition(x, y);

        Vector3 worldPosition = new Vector3(localPosition.x, localPosition.y, 0f) + context.RendererContext.ViewRoot.position;

        return worldPosition;
    }

    public virtual float CellWidthInUnit()
    {
        return CellWidth * UnitSize;
    }

    public virtual float CellHeightInUnit()
    {
        return CellHeight * UnitSize;
    }

    public virtual BoardPosition GetPointAtScreenPosition(Vector3 screenPosition)
    {
        Vector3 worldPosition = ViewCamera.ScreenToWorldPoint(screenPosition);
        Vector3 worldPoint00 = GetWorldPosition(0, 0) - new Vector3(CellWidthInUnit() * 0.5f, CellHeightInUnit() * 0.5f, 0f);
        Vector3 worldPoint11 = GetWorldPosition(Width, Height) - new Vector3(CellWidthInUnit() * 0.5f, CellHeightInUnit() * 0.5f, 0f);


        float xPercent = (worldPosition.x - worldPoint00.x) / (worldPoint11.x - worldPoint00.x);
        float yPercent = (worldPosition.y - worldPoint00.y) / (worldPoint11.y - worldPoint00.y);

        if (xPercent < 0f || xPercent > 1f || yPercent < 0f || yPercent > 1f)
        {
            return new BoardPosition(-1, -1);
        }

        int x = (int)Mathf.Lerp(0, Width, xPercent);
        int y = (int)Mathf.Lerp(0, Height, yPercent);

        return new BoardPosition(x, y);
    }

    public BoardPosition GetSectorPosition(Vector3 worldPos)
    {
        Vector3 opPoint = new Vector3(worldPos.x, SectorsGridView.position.y, 0f);
        
        float angle = SectorsGridView.localRotation.eulerAngles.x;

        float offset = (worldPos - opPoint).magnitude * Mathf.Tan(angle * Mathf.Deg2Rad);
        
        Vector3 projPoint = new Vector3(worldPos.x, worldPos.y, Mathf.Sign(worldPos.y - SectorsGridView.position.y) *offset);
        
        Vector3 localPos = SectorsGridView.InverseTransformPoint(projPoint);

        int x = (int)((localPos.x) / UnitSize);
        int y = (int)((localPos.y) / UnitSize);

        return  new BoardPosition(x, y, 0);
    }
    
    public Vector3 GetSectorWorldPosition(int x, int y, int z)
    {
        Vector3 localPosition = new Vector3(x * UnitSize, y * UnitSize, z);

        Vector3 targetPos = SectorsGridView.TransformPoint(localPosition);

        targetPos = Vector3.ProjectOnPlane(targetPos, Vector3.forward);

        return targetPos;
    }
    
    public Vector3 GetSectorCenterWorldPosition(int x, int y, int z)
    {
        Vector3 localPosition = new Vector3(x * UnitSize + UnitSize * 0.5f, y * UnitSize +UnitSize * 0.5f, z);

        Vector3 targetPos = SectorsGridView.TransformPoint(localPosition);

        targetPos = Vector3.ProjectOnPlane(targetPos, Vector3.forward);

        return targetPos;
    }

}