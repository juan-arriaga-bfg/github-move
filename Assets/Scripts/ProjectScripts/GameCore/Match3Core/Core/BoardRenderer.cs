using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = IW.Logger;

public partial class BoardRenderer : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }

    public Transform SectorsContainer { get; private set; }

    protected BoardController context;
    
    public BoardController Context
    {
        get { return context; }
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardController;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    private NSPoolContainer pool = new NSPoolContainer(false);

    private BoardResourcesDef gameBoardResourcesDef;

    private Dictionary<BoardPosition, BoardElementView> cachedViewMatrix;

    private List<BoardAnimation> animationsQueue = new List<BoardAnimation>();

    private List<BoardAnimation> performingAnimationsQueue = new List<BoardAnimation>();

    protected Transform viewRoot;

    public Transform ViewRoot
    {
        get { return viewRoot; }
    }

    public virtual BoardResourcesDef GameBoardResourcesDef
    {
        get { return gameBoardResourcesDef;  }
    }

    public string GetResourceNameForType(int pieceType)
    {
        if (gameBoardResourcesDef.ElementsResourcesDef.ContainsKey(pieceType) == false)
        {
            Debug.LogWarning($"[BoardRenderer:GetResourceNameForType] No resource for piece type: {pieceType}");
            return null;
        }

        return gameBoardResourcesDef.ElementsResourcesDef[pieceType];
    }

    public virtual BoardRenderer Init(BoardResourcesDef gameBoardResourcesDef, Transform viewRoot)
    {
        this.gameBoardResourcesDef = gameBoardResourcesDef;
        this.viewRoot = viewRoot;

        cachedViewMatrix = new Dictionary<BoardPosition, BoardElementView>();

        // register pool with pieces
        foreach (var pieceDef in gameBoardResourcesDef.ElementsResourcesDef)
        {
            var obj = ContentService.Instance.Manager.GetObjectByName(pieceDef.Value) as GameObject;

            if (obj == null) continue;

            pool.Register(obj);
        }

        return this;
    }

    public virtual Transform CreateElement(int elementType)
    {
        var resourceName = GetResourceNameForType(elementType);
        if (string.IsNullOrEmpty(resourceName)) return null;

        var element = pool.Create<Transform>(resourceName);

        return element;
    }

    public virtual T CreateBoardElement<T>(int elementType) where T : BoardElementView
    {
        var resourceName = GetResourceNameForType(elementType);
        if (string.IsNullOrEmpty(resourceName)) return null;

        var element = pool.Create<T>(resourceName);

        return element;
    }

    public virtual T CreateBoardElement<T>(string resourceName) where T : BoardElementView
    {
        if (string.IsNullOrEmpty(resourceName)) return null;

        var element = pool.Create<T>(resourceName);

        return element;
    }

    public virtual T CreateBoardElementAt<T>(string resourceName, BoardPosition pos) where T : BoardElementView
    {
        if (string.IsNullOrEmpty(resourceName)) return null;

        var element = pool.Create<T>(resourceName);

        if (element != null)
        {
            element.Init(this);
            ResetBoardElement(element, pos);
        }

        return element;
    }

    private T CreateBoardElementAt<T>(int elementType, BoardPosition pos) where T : BoardElementView
    {
        var resourceName = GetResourceNameForType(elementType);
        if (string.IsNullOrEmpty(resourceName)) return null;

        var element = pool.Create<T>(resourceName);

        if (element != null)
        {
            element.Init(this);
            ResetBoardElement(element, pos);
        }

        return element;
    }

    public virtual bool IsHasCellAt(int[,,] logicMatrix, int x, int y)
    {
        int w = logicMatrix.GetLength(0);
        int h = logicMatrix.GetLength(1);
        if (x < 0 || x >= w) return false;
        if (y < 0 || y >= h) return false;
        if (logicMatrix != null && logicMatrix[x, y, BoardLayer.Piece.Layer] == PieceType.Empty.Id) return false;

        return true;
    }

    public virtual void DrawBoardLayout(int[,,] logicMatrix, Dictionary<string, string> skinDef)
    {
        // generate mesh
        Mesh cornerMesh = new Mesh();
        var cornerVertices = new List<Vector3>();
        var cornerTris = new List<int>();
        var cornerUV = new List<Vector2>();
        var cornerColors = new List<Color>();
        int cornerCellIndex = 0;

        
        Mesh cellsMesh = new Mesh();
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        var uv = new List<Vector2>();
        var colors = new List<Color>();
        int cellIndex = 0;
        float borderWidth = 0.2f;
        float cornerSize = 0.2f;
        
        var defaultColor = Color.white;
        float grayCoef = 256f;
        var grayColor = new Color(grayCoef / 256f, grayCoef / 256f, grayCoef / 256f, 1f);

        for (int x = 0; x < context.BoardDef.Width; x++)
        {
            for (int y = 0; y < context.BoardDef.Height; y++)
            {
                if (IsHasCellAt(logicMatrix, x, y))
                {
                    var fullTile = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
                    if ((x + y) % 2 == 0)
                    {
                        fullTile = IconService.Current.GetSpriteById(skinDef["cell_tile_full_1"]);
                    }

                    vertices.Add(new Vector3(x, y + 1, 0));
                    vertices.Add(new Vector3(x + 1, y + 1, 0));
                    vertices.Add(new Vector3(x, y, 0));
                    vertices.Add(new Vector3(x + 1, y, 0));


                    tris.Add(cellIndex);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 2);

                    tris.Add(cellIndex + 2);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 3);


                    if (IsHasCellAt(logicMatrix, x, y + 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(logicMatrix, x - 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(logicMatrix, x + 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(logicMatrix, x, y - 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(logicMatrix, x + 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(logicMatrix, x, y + 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(logicMatrix, x - 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(logicMatrix, x, y - 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    for (int i = 0; i < fullTile.uv.Length; i++)
                    {
                        var uvPos = fullTile.uv[i];
                        uv.Add(uvPos);
                    }

                    cellIndex = cellIndex + 4;
                }

                // check for borders
                if (IsHasCellAt(logicMatrix, x - 1, y) == false && IsHasCellAt(logicMatrix, x, y))
                {
                    // left border
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_00"]);

                    float offsetYBottom = 0f;
                    float offsetYTop = 0f;

                    if ((IsHasCellAt(logicMatrix, x, y - 1) || IsHasCellAt(logicMatrix, x - 1, y - 1))
                        && (IsHasCellAt(logicMatrix, x, y - 1) && IsHasCellAt(logicMatrix, x - 1, y - 1)) == false)
                    {
                        offsetYBottom = 0f;
                    }
                    else
                    {
                        offsetYBottom = cornerSize;
                    }
                    
                    if ((IsHasCellAt(logicMatrix, x, y + 1) || IsHasCellAt(logicMatrix, x - 1, y + 1))
                        && (IsHasCellAt(logicMatrix, x, y + 1) && IsHasCellAt(logicMatrix, x - 1, y + 1)) == false )
                    {
                        offsetYTop = 0f;
                    }
                    else
                    {
                        offsetYTop = cornerSize;
                    }
                    

                    // draw border for x0 y0
                    cornerVertices.Add(new Vector3(x - borderWidth, y + 1 - offsetYTop, 0));
                    cornerVertices.Add(new Vector3(x, y + 1 - offsetYTop, 0));
                    cornerVertices.Add(new Vector3(x - borderWidth, y + offsetYBottom, 0));
                    cornerVertices.Add(new Vector3(x, y + offsetYBottom, 0)); 
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);

                    for (int i = 0; i < borderTile.uv.Length; i++)
                    {
                        cornerUV.Add(borderTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x + 1, y) == false && IsHasCellAt(logicMatrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_01"]);
                    
                    float offsetYBottom = 0f;
                    float offsetYTop = 0f;

                    if ((IsHasCellAt(logicMatrix, x, y - 1) || IsHasCellAt(logicMatrix, x + 1, y - 1))
                        && (IsHasCellAt(logicMatrix, x, y - 1) && IsHasCellAt(logicMatrix, x + 1, y - 1)) == false)
                    {
                        offsetYBottom = 0f;
                    }
                    else
                    {
                        offsetYBottom = cornerSize;
                    }
                    
                    if ((IsHasCellAt(logicMatrix, x, y + 1) || IsHasCellAt(logicMatrix, x + 1, y + 1))
                        && ((IsHasCellAt(logicMatrix, x, y + 1) && IsHasCellAt(logicMatrix, x + 1, y + 1)) == false))
                    {
                        offsetYTop = 0f;
                    }
                    else
                    {
                        offsetYTop = cornerSize;
                    }
                    

                    // draw border for x0 y0
                    cornerVertices.Add(new Vector3(x + 1, y + 1 - offsetYTop, 0));
                    cornerVertices.Add(new Vector3(x + 1 + borderWidth, y + 1 - offsetYTop, 0));
                    cornerVertices.Add(new Vector3(x + 1, y + offsetYBottom, 0));
                    cornerVertices.Add(new Vector3(x + 1 + borderWidth, y + offsetYBottom, 0));

                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);

                    for (int i = 0; i < borderTile.uv.Length; i++)
                    {
                        cornerUV.Add(borderTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x, y - 1) == false && IsHasCellAt(logicMatrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_02"]);
                    
                    float offsetXLeft = 0f;
                    float offsetXRight = 0f;

                    if ((IsHasCellAt(logicMatrix, x - 1, y) || IsHasCellAt(logicMatrix, x - 1, y - 1))
                        && (IsHasCellAt(logicMatrix, x - 1, y) && IsHasCellAt(logicMatrix, x - 1, y - 1)) == false)
                    {
                        offsetXLeft = 0f;
                    }
                    else
                    {
                        offsetXLeft = cornerSize;
                    }
                    
                    if ((IsHasCellAt(logicMatrix, x + 1, y) || IsHasCellAt(logicMatrix, x + 1, y - 1))
                        && (IsHasCellAt(logicMatrix, x + 1, y) && IsHasCellAt(logicMatrix, x + 1, y - 1)) == false)
                    {
                        offsetXRight = 0f;
                    }
                    else
                    {
                        offsetXRight = cornerSize;
                    }

                    // draw border for x0 y0
                    cornerVertices.Add(new Vector3(x + offsetXLeft, y, 0));
                    cornerVertices.Add(new Vector3(x + 1 - offsetXRight, y, 0));
                    cornerVertices.Add(new Vector3(x + offsetXLeft, y - borderWidth, 0));
                    cornerVertices.Add(new Vector3(x + 1 - offsetXRight, y - borderWidth, 0));

                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);

                    for (int i = 0; i < borderTile.uv.Length; i++)
                    {
                        cornerUV.Add(borderTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x, y + 1) == false && IsHasCellAt(logicMatrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_03"]);
                    
                    float offsetXLeft = 0f;
                    float offsetXRight = 0f;

                    if ((IsHasCellAt(logicMatrix, x - 1, y) || IsHasCellAt(logicMatrix, x - 1, y + 1))
                        && (IsHasCellAt(logicMatrix, x - 1, y) && IsHasCellAt(logicMatrix, x - 1, y + 1)) == false)
                    {
                        offsetXLeft = 0f;
                    }
                    else
                    {
                        offsetXLeft = cornerSize;
                    }
                    
                    if ((IsHasCellAt(logicMatrix, x + 1, y) || IsHasCellAt(logicMatrix, x + 1, y + 1))
                        && (IsHasCellAt(logicMatrix, x + 1, y) && IsHasCellAt(logicMatrix, x + 1, y + 1)) == false)
                    {
                        offsetXRight = 0f;
                    }
                    else
                    {
                        offsetXRight = cornerSize;
                    }

                    cornerVertices.Add(new Vector3(x + offsetXLeft, y + 1 + borderWidth, 0));
                    cornerVertices.Add(new Vector3(x + 1 - offsetXRight, y + 1 + borderWidth, 0));
                    cornerVertices.Add(new Vector3(x + offsetXLeft, y + 1, 0));
                    cornerVertices.Add(new Vector3(x + 1 - offsetXRight, y + 1, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);

                    for (int i = 0; i < borderTile.uv.Length; i++)
                    {
                        cornerUV.Add(borderTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                // check corner
                if (IsHasCellAt(logicMatrix, x - 1, y - 1) == false 
                    && IsHasCellAt(logicMatrix, x - 1, y) 
                    && IsHasCellAt(logicMatrix, x, y - 1))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_10"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize, y, 0));
                    cornerVertices.Add(new Vector3(x, y, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize, y - cornerSize, 0));
                    cornerVertices.Add(new Vector3(x, y - cornerSize, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x, y - 1) == false 
                    && IsHasCellAt(logicMatrix, x - 1, y - 1)
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_20"]);
                    
                    cornerVertices.Add(new Vector3(x, y, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y, 0));
                    cornerVertices.Add(new Vector3(x, y - cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y - cornerSize, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x, y + 1) == false 
                    && IsHasCellAt(logicMatrix, x - 1, y + 1)
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_30"]);
                    
                    cornerVertices.Add(new Vector3(x, y + 1 + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y + 1 + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x, y + 1, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y + 1, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x - 1, y) == false 
                    && IsHasCellAt(logicMatrix, x - 1, y - 1)
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_40"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize, y, 0));
                    cornerVertices.Add(new Vector3(x, y, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                //out corners
                if (IsHasCellAt(logicMatrix, x - 1, y) == false 
                    && IsHasCellAt(logicMatrix, x, y - 1) == false
                    && IsHasCellAt(logicMatrix, x - 1, y - 1) == false
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_11"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize, y - cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y - cornerSize, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x - 1, y) == false 
                    && IsHasCellAt(logicMatrix, x - 1, y + 1) == false
                    && IsHasCellAt(logicMatrix, x, y + 1) == false
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_14"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize, y + cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y + cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize, y - cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize, y - cornerSize + 1, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x + 1, y) == false 
                    && IsHasCellAt(logicMatrix, x + 1, y + 1) == false
                    && IsHasCellAt(logicMatrix, x, y + 1) == false
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_13"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize + 1, y + cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1, y + cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize + 1, y - cornerSize + 1, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1, y - cornerSize + 1, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
                if (IsHasCellAt(logicMatrix, x + 1, y) == false 
                    && IsHasCellAt(logicMatrix, x + 1, y - 1) == false
                    && IsHasCellAt(logicMatrix, x, y - 1) == false
                    && IsHasCellAt(logicMatrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_12"]);
                    
                    cornerVertices.Add(new Vector3(x - cornerSize + 1, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1, y + cornerSize, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize + 1, y - cornerSize, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1, y - cornerSize, 0));
                    
                    cornerTris.Add(cornerCellIndex);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 2);

                    cornerTris.Add(cornerCellIndex + 2);
                    cornerTris.Add(cornerCellIndex + 1);
                    cornerTris.Add(cornerCellIndex + 3);
  
                    for (int i = 0; i < cornerTile.uv.Length; i++)
                    {
                        cornerUV.Add(cornerTile.uv[i]);
                        cornerColors.Add(defaultColor);
                    }
                    cornerCellIndex = cornerCellIndex + 4;
                }
                
            }
        }

        cellsMesh.vertices = vertices.ToArray();
        cellsMesh.triangles = tris.ToArray();
        cellsMesh.uv = uv.ToArray();
        cellsMesh.colors = colors.ToArray();

        cellsMesh.RecalculateBounds();

        
        var meshGO = new GameObject("_cells");
        var meshTransform = meshGO.transform;
        meshTransform.SetParent(ViewRoot);

        var meshRenderer = meshGO.AddComponent<MeshRenderer>();
        var meshFilter = meshGO.AddComponent<MeshFilter>();

        meshTransform.localScale = new Vector3(127.4f, 127.4f, 1f);

        meshFilter.mesh = cellsMesh;

        // apply material 
        var material = new Material(Shader.Find("Sprites/Opaque"));
        material.renderQueue = 999;

        // load texture
        var tileSprite = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
        var tileTexture = tileSprite == null ? null : tileSprite.texture;
        material.mainTexture = tileTexture;

        meshRenderer.material = material;
        
        // size 
        var boundsOffset = new Vector3(-meshRenderer.bounds.extents.x, -meshRenderer.bounds.extents.y, 0f);
        var localOffset = new Vector3(0f, -61f, 0f);
        meshTransform.localPosition = boundsOffset + localOffset;
        
        
        // corners
        cornerMesh.vertices = cornerVertices.ToArray();
        cornerMesh.triangles = cornerTris.ToArray();
        cornerMesh.uv = cornerUV.ToArray();
        cornerMesh.colors = cornerColors.ToArray();

        cornerMesh.RecalculateBounds();

        var cornerMeshGO = new GameObject("_cells_corner");
        var cornerMeshTransform = cornerMeshGO.transform;
        cornerMeshTransform.SetParent(meshTransform);

        var cornerMeshRenderer = cornerMeshGO.AddComponent<MeshRenderer>();
        var cornerMeshFilter = cornerMeshGO.AddComponent<MeshFilter>();

        cornerMeshTransform.localPosition = new Vector3(0f, 0f, 0f);
        cornerMeshTransform.localScale = new Vector3(1f, 1f, 1f);

        cornerMeshFilter.mesh = cornerMesh;

        // apply material 
        var cornerMaterial = new Material(Shader.Find("Sprites/Default"));
        cornerMaterial.renderQueue = 1000;

        // load texture
        var cornerTileSprite = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
        var cornerTileTexture = cornerTileSprite == null ? null : cornerTileSprite.texture;
        cornerMaterial.mainTexture = cornerTileTexture;

        cornerMeshRenderer.material = cornerMaterial;
    }

    public virtual BoardElementView GetElementAt(BoardPosition boardPosition)
    {
        if (cachedViewMatrix.ContainsKey(boardPosition))
        {
            return cachedViewMatrix[boardPosition];
        }
        return null;
    }

    public virtual PieceBoardElementView CreatePieceAt(Piece piece, BoardPosition at)
    {
        return CreatePieceAt(piece, at.X, at.Y);
    }

    public virtual PieceBoardElementView CreatePieceAt(Piece piece, int x, int y)
    {
        var pieceView = CreateElementAt(piece.PieceType, x, y, piece.Layer.Index) as PieceBoardElementView;

        if (pieceView != null)
        {
            pieceView.Init(this, piece);
        }

        return pieceView;
    }

    public virtual bool SwapElements(BoardPosition from, BoardPosition to)
    {
        if (cachedViewMatrix.ContainsKey(from) == false || cachedViewMatrix.ContainsKey(to) == false) return false;
        var fromElement = cachedViewMatrix[from];
        var toElement = cachedViewMatrix[to];

        cachedViewMatrix[to] = fromElement;
        cachedViewMatrix[from] = toElement;

        toElement.SyncRendererLayers(from);
        fromElement.SyncRendererLayers(to);

        return true;
    }

    public virtual bool MoveElement(BoardPosition from, BoardPosition to)
    {
        if (cachedViewMatrix.ContainsKey(from) == false || cachedViewMatrix.ContainsKey(to)) return false;
        var fromElement = cachedViewMatrix[from];

        cachedViewMatrix.Remove(from);
        cachedViewMatrix[to] = fromElement;

        fromElement.SyncRendererLayers(to);

        return true;
    }

    public virtual bool SetElementAt(BoardPosition to, BoardElementView element, bool isIgnoreExisting = true)
    {
        if (isIgnoreExisting == false && cachedViewMatrix.ContainsKey(to) && cachedViewMatrix[to] != null)
        {
            return false;
        }

        if (cachedViewMatrix.ContainsKey(to) == false)
        {
            cachedViewMatrix.Add(to, element);
        }

        cachedViewMatrix[to] = element;
        element.SyncRendererLayers(to);

        return true;
    }

    public virtual void DrawMatrix(BoardLogicComponent logicMatrix)
    {
        ClearMatrix();

        foreach (var pieceDef in logicMatrix.BoardEntities)
        {
            var pieceView = CreatePieceAt(pieceDef.Value, pieceDef.Key.X, pieceDef.Key.Y);
        }
    }

    public virtual void ClearMatrix()
    {
        if (cachedViewMatrix == null) return;

        var cachedViewMatrixCopy = new Dictionary<BoardPosition, BoardElementView>();

        foreach (var views in cachedViewMatrixCopy)
        {
            if (views.Key.Z > 0)
            {
                pool.Return(views.Value.gameObject);
                cachedViewMatrix.Remove(views.Key);
            }
        }
    }

    public virtual BoardElementView RemoveElement(BoardElementView elementView, bool isDestroy = true)
    {
        var boardPosition = GetBoardPosition(elementView);

        return RemoveElementAt(boardPosition, isDestroy);
    }
    
    public virtual BoardElementView RemoveElementAt(BoardPosition boardPosition, bool isDestroy = true)
    {
        BoardElementView elementView;
        if (cachedViewMatrix.TryGetValue(boardPosition, out elementView))
        {
            if (cachedViewMatrix.Remove(boardPosition))
            {
                if (isDestroy)
                {
                    pool.Return(elementView.gameObject);
                }
            }
            return elementView;
        }

        return null;
    }
    
    public virtual void DestroyElement(BoardElementView elementView)
    {
        if (elementView == null) return;

        pool.Return(elementView.gameObject);
    }
    
    public virtual void DestroyElement(GameObject element)
    {
        if (element == null) return;
        
        pool.Return(element);
    }
    
    public virtual BoardElementView CreateElementAt(int elementType, BoardPosition pos)
    {
        return CreateElementAt(elementType, pos.X, pos.Y, pos.Z);
    }

    public virtual void ResetBoardElement(BoardElementView view, BoardPosition boardPosition)
    {
        if (view == null) return;

        var pos = context.BoardDef.GetPiecePosition(boardPosition.X, boardPosition.Y);
        view.CachedTransform.SetParent(ViewRoot);
        view.CachedTransform.localPosition = new Vector3(pos.x, pos.y, 0f);
        view.CachedTransform.localRotation = Quaternion.identity;
        view.CachedTransform.localScale = Vector3.one * context.BoardDef.GlobalPieceScale;

        view.SyncRendererLayers(boardPosition);
    }

    public virtual BoardElementView CreateElementAt(int elementType, int x, int y, int z)
    {
        var boardPosition = new BoardPosition(x, y, z);
        // check is current pos is blocked
        if (cachedViewMatrix.ContainsKey(boardPosition) && cachedViewMatrix[boardPosition] != null)
        {
            context.Logger.Log("CreateElementAt already used: " + boardPosition.ToString());
            Debug.LogError("CreateElementAt already used: " + boardPosition.ToString());
            return null;
        }

        var elementView = CreateBoardElement<BoardElementView>(elementType);

        if (elementView != null)
        {
            var pos = context.BoardDef.GetPiecePosition(x, y);
            elementView.CachedTransform.SetParent(ViewRoot);
            elementView.CachedTransform.localPosition = new Vector3(pos.x, pos.y, 0f);
            elementView.CachedTransform.localRotation = Quaternion.identity;
            elementView.CachedTransform.localScale = Vector3.one * context.BoardDef.GlobalPieceScale;

            RegisterElement(elementView, boardPosition);
            elementView.SyncRendererLayers(boardPosition);
        }

        return elementView;
    }

    public BoardElementView CreateElementOutsideOfBoard(int elementType)
    {
        var elementView = CreateBoardElement<BoardElementView>(elementType);

        if (elementView != null)
        {
            elementView.CachedTransform.SetParent(ViewRoot);
            elementView.CachedTransform.localPosition = Vector3.zero;
            elementView.CachedTransform.localRotation = Quaternion.identity;
            elementView.CachedTransform.localScale = Vector3.one * context.BoardDef.GlobalPieceScale;
        }

        return elementView;
    }

    public virtual void RegisterElement(BoardElementView boardElementView, BoardPosition boardPosition)
    {
        if (cachedViewMatrix.ContainsKey(boardPosition) == false)
        {
            cachedViewMatrix.Add(boardPosition, boardElementView);
            boardElementView.Init(this);
        }
    }

    public virtual BoardPosition GetBoardPosition(BoardElementView elementView)
    {
        BoardPosition position = new BoardPosition(-1, -1);
        foreach (var elementPair in cachedViewMatrix)
        {
            if (elementPair.Value == elementView)
            {
                position = elementPair.Key;
            }
        }
        return position;
    }

    public virtual void AddAnimationToQueue(BoardAnimation boardAnimation)
    {
        animationsQueue.Add(boardAnimation);
    }

    public virtual void PerformAnimations()
    {
        for (int i = animationsQueue.Count - 1; i >= 0; i--)
        {
            if (performingAnimationsQueue.Contains(animationsQueue[i]) == false)
            {
                performingAnimationsQueue.Add(animationsQueue[i]);
                
#if IW_FULL_ACTION_LOG
                Debug.LogWarning($"--> Anim Started: {animationsQueue[i].GetType()}");
#endif
                animationsQueue[i].Animate(this);
            }
        }
    }

    public virtual bool IsPerformingAnimation<T>() where T : BoardAnimation
    {
        bool state = false;
        foreach (var anim in performingAnimationsQueue)
        {
            if (anim is T)
            {
                state = true;
            }
        }
        return state;
    }

    public virtual T GetPerformingAnimation<T>() where T : BoardAnimation
    {
        for (int i = 0; i < performingAnimationsQueue.Count; i++)
        {
            var anim = performingAnimationsQueue[i];
            if (anim is T)
            {
                return anim as T;
            }
        }
        return null;
    }

    public virtual List<T> GetPerformingAnimations<T>() where T : BoardAnimation
    {
        List<T> animations = new List<T>();
        foreach (var anim in performingAnimationsQueue)
        {
            if (anim is T)
            {
                animations.Add(anim as T);
            }
        }
        return animations;
    }
    
    public virtual void StopMoveAnimationsAt(BoardRenderer context, BoardPosition at)
    {
        var performingAnimations = context.GetPerformingAnimations();

        context.context.Manipulator.StopDragAnimation(at);
    
        for (int i = 0; i < performingAnimations.Count; i++)
        {
            var movePieceFromToAnimation = performingAnimations[i] as MovePieceFromToAnimation;
            if (movePieceFromToAnimation != null)
            {
                if (movePieceFromToAnimation.To.Equals(at))
                {
                    movePieceFromToAnimation.StopAnimation(context);
                }
            }
            
            var resetPiecePositionAnimation = performingAnimations[i] as ResetPiecePositionAnimation;
            if (resetPiecePositionAnimation != null)
            {
                if (resetPiecePositionAnimation.At.Equals(at))
                {
                    resetPiecePositionAnimation.StopAnimation(context);
                }
            }
        }
    }
    
    public virtual List<BoardAnimation> GetPerformingAnimations()
    {
        return performingAnimationsQueue;
    }
    
    public virtual List<BoardAnimation> GetAnimationsQueue()
    {
        return animationsQueue;
    }

    public virtual void CompleteAnimation(BoardAnimation animation)
    {
        performingAnimationsQueue.Remove(animation);
        animationsQueue.Remove(animation);
        
#if IW_FULL_ACTION_LOG
                Debug.LogWarning($"--> Anim Completed: {animation.GetType()}");
#endif
    }

    public virtual BoardPosition GetEmptyPointUp(BoardPosition point, BoardLogicComponent matrix)
    {
        if (cachedViewMatrix.ContainsKey(point) && cachedViewMatrix[point] != null)
        {
            if (point.Up.IsValid && cachedViewMatrix.ContainsKey(point.Up) && cachedViewMatrix[point.Up] != null)
            {
                return GetEmptyPointUp(point.Up, matrix);
            }

            return point;
        }

        return point;
    }

    public virtual void ResetBoardPosition()
    {
        context.BoardDef.ViewCamera.orthographicSize = 622f;
        viewRoot.localPosition = new Vector3(52f, 61f, 0f);
    }
    
    public Transform GenerateField(int width, int height, float size, Dictionary<int, BoardTileDef> tileDefs)
    {
         SectorsContainer = new GameObject("Sectors.Container").transform;
         SectorsContainer.localPosition = new Vector3(0f, 0f, 0f);
         SectorsContainer.localRotation = Quaternion.Euler(54.5f, 0f, -45f);
         SectorsContainer.localScale = Vector3.one;

         var layout = GameDataService.Current.FieldManager.LayoutData;
         
         var sectorsMesh = GenerateMesh(width, height, size, layout, tileDefs);
         
         var meshGO = new GameObject("_cells");
         var meshTransform = meshGO.transform;
         meshTransform.SetParent(SectorsContainer);
         meshTransform.localPosition = Vector3.zero;
         meshTransform.localScale = new Vector3(1f, 1f, 1f);
         meshTransform.localRotation = Quaternion.identity;
         
         var meshRenderer = meshGO.AddComponent<MeshRenderer>();
         var meshFilter = meshGO.AddComponent<MeshFilter>();

         meshFilter.mesh = sectorsMesh;
                 
         // load texture
         Sprite tileSprite = null;
         foreach (var def in tileDefs.Values)
         {
             string id = def.SpriteName;
             if (!string.IsNullOrEmpty(id))
             {
                 tileSprite = IconService.Current.GetSpriteById(id);
             }
         }
         
         var tileTexture = tileSprite == null ? null : tileSprite.texture;
         
         // apply material 
         var material = new Material(Shader.Find("Sprites/Default"));
         material.mainTexture = tileTexture;
         
         meshRenderer.material = material;
         meshRenderer.sortingOrder = 0;
         meshRenderer.sortingLayerName = "Default";
         meshRenderer.material.renderQueue = 2000;

         return SectorsContainer.transform;
     }

    private Mesh GenerateMesh(int width, int height, float size, int[] layout, Dictionary<int, BoardTileDef> tileDefs)
    {
#if DEBUG
        var sw = new Stopwatch();
        sw.Start();
#endif        
        Mesh mesh = new Mesh();
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        var uv = new List<Vector2>();
        var colors = new List<Color>();
        int cellIndex = 0;
        float borderWidth = size;
        var defaultColor = new Color(1f, 1f, 1f, 1f);
        
        // todo: implement index-based cache for sprites access instead of dictionary? 
        // Cache sprites
        // Sprite[] tilesSprites = new Sprite[tiles.Count];
        // for (var i = 0; i < tiles.Count; i++)
        // {
        //     var tile = tiles[i];
        //     if (string.IsNullOrEmpty(tile))
        //     {
        //         continue;
        //     }
        //     
        //     Sprite sprite = IconService.Current.GetSpriteById(tile);
        //     tilesSprites[i] = sprite;
        // }

        // Build layout
        int layoutIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileId = layout[layoutIndex];
#if DEBUG
                if (!tileDefs.TryGetValue(tileId, out BoardTileDef tileDef))
                {
                    IW.Logger.LogError($"[BoardRenderer] => GenerateMesh: Unknown tile id: {tileId}");
                }
#else
                BoardTileDef tileDef = tileDefs[tileId];    
#endif

                Sprite sprite;
                if (tileDef.SpriteChess != null && (x + y) % 2 == 0)
                {
                    sprite = tileDef.SpriteChess;
                }
                else
                {
                    sprite = tileDef.Sprite; 
                }

                if (sprite != null)
                {
                    vertices.Add(new Vector3(x * borderWidth,       (y + 1) * borderWidth, 0));
                    vertices.Add(new Vector3((x + 1) * borderWidth, (y + 1) * borderWidth, 0));
                    vertices.Add(new Vector3(x * borderWidth,       y * borderWidth,       0));
                    vertices.Add(new Vector3((x + 1) * borderWidth, y * borderWidth,       0));

                    tris.Add(cellIndex);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 2);

                    tris.Add(cellIndex + 2);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 3);

                    for (int i = 0; i < sprite.uv.Length; i++)
                    {
                        var uvPos = sprite.uv[i];
                        uv.Add(uvPos);
                        colors.Add(defaultColor);
                    }
                    
                    cellIndex = cellIndex + 4;
                }

                layoutIndex++;
            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateBounds();

#if DEBUG
        sw.Stop();
        Debug.Log($"[BoardRenderer] => GenerateMesh: Done in {sw.ElapsedMilliseconds}ms");
#endif
        
        return mesh;
    }

    public void CreateBackgroundWater()
    {
        var waterPrefab = ContentService.Current.GetObjectByName(R.BackgroundWater);
        GameObject water = (GameObject) GameObject.Instantiate(waterPrefab);
    }

    private Dictionary<string, List<string>> GetAddonsList()
    {
        // Value in dict is a count of NULL, added as addons. More nulls - more tiles without any addons
        Dictionary<string, int> data = new Dictionary<string, int>
        {
            {R.BorderBottom1,         4},
            {R.BorderBottom2,         4},
            {R.BorderLeft1,           4},
            {R.BorderLeft2,           4},
            {R.BorderWallBottomLeft1, 1},
            {R.BorderWallRight1,      1},
            {R.BorderWallTop1,        1},
            {R.BorderWallTopRight1,   1},
        };

        var contentService = ContentService.Current;
        Dictionary<string, List<string>> addons = new Dictionary<string, List<string>>();
        foreach (var pair in data)
        {
            string borderId = pair.Key;
            List<string> addonsForBorder = new List<string>();
            for (int i = 1; i < 100; i++)
            {
                string addonId = $"{borderId}_Addon_{i}";
                if (!contentService.IsObjectRegistered(addonId))
                {
                    break;
                }
                
                addonsForBorder.Add(addonId);
            }

            for (int i = 0; i < pair.Value; i++)
            {
                addonsForBorder.Add(null);
            }
            
            addons.Add(borderId, addonsForBorder);
        }

        return addons;
    }
    
    public void CreateBorders()
    {
#if DEBUG
        var sw = new Stopwatch();
        sw.Start();
#endif
        // int seed = UnityEngine.Random.Range(0, 10000);
        // IW.Logger.Log($"Seed: {seed}");

        int seed = 8322;
        
        System.Random random = new System.Random(seed);
        
        
        var boardDef = context.BoardDef;
        
        var fieldManager = GameDataService.Current.FieldManager;

        var w = fieldManager.LayoutW;
        var h = fieldManager.LayoutH;

        GameObject root = new GameObject();
        root.name = "FieldBorders";
        
        bool IsCellExists(int x, int y)
        {
            return x >= 0 && y >= 0 && x < w && y < h;
        }

        var addons = GetAddonsList();

        void CreateBorder(int x, int y, string item)
        {
            InstantiateItem(x, y, item);

            if (addons.TryGetValue(item, out List<string> availableProps))
            {
                int len = availableProps.Count;
                int index = random.Next(0, len);
                string prop = availableProps[index];

                if (prop != null)
                {
                    InstantiateItem(x, y, prop);
                }
            }
        }
            
        void InstantiateItem(int x, int y, string item)
        {
            var prefab = ContentService.Current.GetObjectByName(item);
            if (prefab == null)
            {
                IW.Logger.LogError($"InstantiateItem: Prefab Not found: {item}");
            }
            
            GameObject go = (GameObject) Object.Instantiate(prefab, root.transform, true);
            go.transform.position = boardDef.GetPiecePosition(x, y);
        }

        int waterId = BoardTiles.WATER_TILE_ID;
        Dictionary<int, BoardTileDef> tilesDefs = BoardTiles.GetDefs();
        
        for (int x = 0; x < w; x++)
        {
            for (int y = h - 1; y >= 0; y--)// Reversed for proper z orders!
            {
                int tileId = fieldManager.GetTileId(x, y);                

                int currentHeight = tilesDefs[tileId].Height;
                // ReSharper disable InconsistentNaming
                int idR  = fieldManager.GetTileId(x + 0, y + 1);
                int idL  = fieldManager.GetTileId(x + 0, y - 1);
                int idT  = fieldManager.GetTileId(x - 1, y + 0);
                int idB  = fieldManager.GetTileId(x + 1, y + 0);
                
                int idBL = fieldManager.GetTileId(x + 1, y - 1);
                int idTR = fieldManager.GetTileId(x - 1, y + 1);
                int idBR = fieldManager.GetTileId(x + 1, y + 1);
                int idTL = fieldManager.GetTileId(x - 1, y - 1);
                // ReSharper restore InconsistentNaming
                
                LayoutTileMeta meta = new LayoutTileMeta {tileId = tileId};
                
                // IsCellExists NOT required until we have any non-water tiles place on borders of the field
                meta.neighborR  = /*IsCellExists(x + 0, y + 1) && */idR  > waterId;
                meta.neighborL  = /*IsCellExists(x + 0, y - 1) && */idL  > waterId;              
                meta.neighborT  = /*IsCellExists(x - 1, y + 0) && */idT  > waterId;
                meta.neighborB  = /*IsCellExists(x + 1, y + 0) && */idB  > waterId;
                
                // for fogs
                if (meta.neighborB || meta.neighborL || meta.neighborR || meta.neighborT)
                {
                    if (tilesDefs[tileId].IsLock)
                    {
                        fieldManager.LockedCells.Add(new BoardPosition(x, y));
                    } 
                }
                
                if (tileId == waterId)
                {
                    continue;
                }
                
                meta.neighborBL = /*IsCellExists(x + 1, y - 1) && */idBL > waterId;
                meta.neighborBR = /*IsCellExists(x - 1, y + 1) && */idBR > waterId;
                meta.neighborTR = /*IsCellExists(x - 1, y + 1) && */idTR > waterId;
                meta.neighborTL = /*IsCellExists(x - 1, y + 1) && */idTL > waterId;

                meta.floorDiffR  = tilesDefs[idR ].Height - currentHeight;
                meta.floorDiffL  = tilesDefs[idL ].Height - currentHeight;                
                meta.floorDiffT  = tilesDefs[idT ].Height - currentHeight;
                meta.floorDiffB  = tilesDefs[idB ].Height - currentHeight;
                
                meta.floorDiffBL = tilesDefs[idBL].Height - currentHeight;
                meta.floorDiffTR = tilesDefs[idTR].Height - currentHeight;
                meta.floorDiffBR = tilesDefs[idBR].Height - currentHeight;
                meta.floorDiffTL = tilesDefs[idTL].Height - currentHeight;
                
                // DebugTextView debugView = BoardService.Current.FirstBoard.RendererContext.CreateBoardElementAt<DebugTextView>(R.DebugCell2, new BoardPosition(x, y, BoardLayer.MAX.Layer));
                // debugView.SetText(meta.ToString());

#region WALLS
                if (meta.neighborT && meta.neighborR && meta.floorDiffT == 1 && meta.floorDiffR == 1)
                {
                    CreateBorder(x, y, R.BorderWallBottomLeft1);
                }
                else if (meta.neighborTR && meta.floorDiffTR == 1 && meta.floorDiffT == 0 && meta.floorDiffR == 0)
                {
                    CreateBorder(x, y, R.BorderWallTopRight1);
                }
                else if (meta.neighborR && meta.floorDiffR == 1)
                {
                    CreateBorder(x, y, R.BorderWallRight1);
                }
                else if (meta.neighborT && meta.floorDiffT == 1)
                {
                    CreateBorder(x, y, R.BorderWallTop1);
                }
#endregion
                
#region COAST                
                if (!meta.neighborT)
                {
                    if (meta.neighborR && meta.floorDiffR == 1 
                     || !meta.neighborT && !meta.neighborR && !meta.neighborTR)
                    {
                        CreateBorder(x, y, R.BorderTop0Short);   
                    }
                    else
                    {
                        CreateBorder(x, y, R.BorderTop0);
                    }
                }
                else if (!meta.neighborTL && meta.neighborT && meta.floorDiffT == -1 && meta.neighborTR && meta.floorDiffTR == 0)
                {
                    CreateBorder(x, y, R.BorderTop0);
                }
 
                if (!meta.neighborR)
                {
                    if (meta.neighborT && meta.floorDiffT == 1
                     || !meta.neighborT && !meta.neighborR && !meta.neighborTR)
                    {
                        CreateBorder(x, y, R.BorderRight0Short);   
                    }
                    else
                    {
                        CreateBorder(x, y, R.BorderRight0);
                    }
                }
                
                if (!meta.neighborB)
                {
                    if (meta.floorDiffB == 0)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderBottom0Hole : R.BorderBottom0);
                    }
                    else if (meta.floorDiffB == -1)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderBottom1Hole : R.BorderBottom1);
                    }
                    else if (meta.floorDiffB == -2)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderBottom2Hole : R.BorderBottom2);
                    }
                }
                
                if (!meta.neighborL)
                {
                    if (meta.floorDiffL == 0)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderLeft0Hole : R.BorderLeft0);
                    }
                    else if (meta.floorDiffL == -1)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderLeft1Hole : R.BorderLeft1);
                    }
                    else if (meta.floorDiffL == -2)
                    {
                        CreateBorder(x, y, meta.neighborBL ? R.BorderLeft2Hole : R.BorderLeft2);
                    }
                }

#if DEBUG
                if (!meta.neighborT && !meta.neighborTR && meta.floorDiffR > 0)
                {
                    string scheme =
                        "WRONG:    |   CORRECT: \n" +
                        "10  10    |   10  10   \n" +
                        "21  302   |   10  302  \n" +
                        "21  302   |   21  302  \n";

                    IW.Logger.LogError($"[BoardRenderer] => CreateBorders: Unsupported case for tile {x},{y}. Use 'ladder' here!\n{scheme}");
                }
#endif
#endregion
                
#region OUTER CORNERS
                if (!meta.neighborT && !meta.neighborR)
                {
                    string id = "BorderTopRightOuterCorner" + Mathf.Abs(meta.floorDiffTR);                    
                    CreateBorder(x, y, id);
                }
                if ((!meta.neighborT || meta.neighborT && meta.floorDiffT < 0) && !meta.neighborL)
                {
                    string id = "BorderTopLeftOuterCorner" + Mathf.Abs(meta.floorDiffTL); 
                    CreateBorder(x, y, id);
                }
                if (!meta.neighborB && !meta.neighborR)
                {
                    string id = "BorderBottomRightOuterCorner" + Mathf.Abs(meta.floorDiffBR); 
                    CreateBorder(x, y, id);
                }
                if ((!meta.neighborB || meta.neighborB && meta.floorDiffB < 0) && !meta.neighborL)
                {
                    string id = "BorderBottomLeftOuterCorner" + Mathf.Abs(meta.floorDiffBL); 
                    CreateBorder(x, y, id);
                } 
#endregion
                
#region INNER CORNERS
                if (meta.neighborT && meta.neighborR && !meta.neighborTR && meta.floorDiffT == meta.floorDiffR)
                {
                    string id = "BorderBottomLeftInnerCorner" + Mathf.Abs(meta.floorDiffTR);                    
                    CreateBorder(x, y, id);
                }
                if (meta.neighborT && meta.neighborL && (!meta.neighborTL || meta.neighborTL && meta.floorDiffTL < 0) && meta.floorDiffT == meta.floorDiffL)
                {
                    string id = "BorderBottomRightInnerCorner" + Mathf.Abs(meta.floorDiffTL); 
                    CreateBorder(x, y, id);
                }
                if (meta.neighborB && meta.neighborR && !meta.neighborBR && meta.floorDiffB == meta.floorDiffR)
                {
                    string id = "BorderTopLeftInnerCorner" + Mathf.Abs(meta.floorDiffBR); 
                    CreateBorder(x, y, id);
                }
                if (meta.neighborB && meta.neighborL && !meta.neighborBL && meta.floorDiffB == meta.floorDiffL)
                {
                    string id = "BorderTopRightInnerCorner" + Mathf.Abs(meta.floorDiffBL); 
                    CreateBorder(x, y, id);
                } 
#endregion
                                
#region INTERSECTION CORNERS
                if (!meta.neighborBL && meta.neighborL && meta.neighborB && meta.floorDiffL == -1 && meta.floorDiffB == 0 && meta.floorDiffBL == -1)
                {                  
                    CreateBorder(x, y, R.BorderTopRightIntersectionCorner0);
                }
                
                if (!meta.neighborL && meta.neighborB && !meta.neighborBL && meta.floorDiffB == -1 && meta.floorDiffBL == -1 && meta.floorDiffL == -1)
                {                  
                    CreateBorder(x, y, R.BorderTopIntersectionCorner0);
                }

#endregion
            }
        }
#if DEBUG
        sw.Stop();
        Debug.LogFormat($"[BoardRenderer] => CreateBorders: Done in {sw.ElapsedMilliseconds}ms");
#endif
    }
}