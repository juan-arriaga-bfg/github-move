using System.Collections.Generic;
using UnityEngine;


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
    
    public Transform GenerateField(int width, int height, float size, List<string> tiles, string backgroundTile = null, IList<BoardPosition> ignorablePositions = null)
    {
         SectorsContainer = new GameObject("Sectors.Container").transform;
         SectorsContainer.localPosition = new Vector3(0f, 0f, 0f);
         SectorsContainer.localRotation = Quaternion.Euler(54.5f, 0f, -45f);
         SectorsContainer.localScale = Vector3.one;

         var layout = GameDataService.Current.FieldManager.LayoutData;
         
         var sectorsMesh = GenerateMesh(width, height, size, tiles, backgroundTile, ignorablePositions, layout);
         
         var meshGO = new GameObject("_cells");
         var meshTransform = meshGO.transform;
         meshTransform.SetParent(SectorsContainer);
         meshTransform.localPosition = Vector3.zero;
         meshTransform.localScale = new Vector3(1f, 1f, 1f);
         meshTransform.localRotation = Quaternion.identity;
         
         var meshRenderer = meshGO.AddComponent<MeshRenderer>();
         var meshFilter = meshGO.AddComponent<MeshFilter>();
         
         
         
         meshFilter.mesh = sectorsMesh;
         
         // apply material 
         var material = new Material(Shader.Find("Sprites/Default"));
         material.renderQueue = 2000;
         
         // load texture
         var tileSprite = IconService.Current.GetSpriteById(tiles[1]);
         var tileTexture = tileSprite == null ? null : tileSprite.texture;
         material.mainTexture = tileTexture;
         
         meshRenderer.material = material;

         return SectorsContainer.transform;
     }

    private Mesh GenerateMesh(int width, int height, float size, List<string> tiles, string ignorableTileName = null, IList<BoardPosition> ignorablePositions = null, byte[] layout = null)
    {
        ignorablePositions = ignorablePositions ?? new List<BoardPosition>();
        Mesh sectorsMesh = new Mesh();
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        var uv = new List<Vector2>();
        var colors = new List<Color>();
        int cellIndex = 0;
        float borderWidth = size;

        var defaultColor = new Color(1f, 1f, 1f, 1f);

        int layoutIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Sprite fullTile;
                var isIgnorable = ignorablePositions.Contains(new BoardPosition(x, y));
                if (!string.IsNullOrEmpty(ignorableTileName) && isIgnorable)
                {
                    fullTile = IconService.Current.GetSpriteById(ignorableTileName);
                }
                else
                {
                    if(isIgnorable)
                        continue;

                    var id = layout == null ? (x + y) % 2 == 0 
                                                ? tiles[1] 
                                                : tiles[0]
                            
                                            : tiles[layout[layoutIndex]];

                    fullTile = IconService.Current.GetSpriteById(id);
                }

                if (fullTile != null && layout[layoutIndex] != 1)
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

                    for (int i = 0; i < fullTile.uv.Length; i++)
                    {
                        var uvPos = fullTile.uv[i];
                        uv.Add(uvPos);
                        colors.Add(defaultColor);
                    }
                    
                    cellIndex = cellIndex + 4;
                }

                layoutIndex++;
            }
        }
        
        sectorsMesh.vertices = vertices.ToArray();
        sectorsMesh.triangles = tris.ToArray();
        sectorsMesh.uv = uv.ToArray();
        sectorsMesh.colors = colors.ToArray();

        sectorsMesh.RecalculateBounds();

        return sectorsMesh;
    }

    public void GenerateBackground(Vector3 position, int width, int height, float size, string backImage, IList<BoardPosition> ignorablePositions = null)
    {
        var sectorsContainer = new GameObject("Background").transform;
        sectorsContainer.localPosition = position;
        sectorsContainer.localRotation = Quaternion.Euler(54.5f, 0f, -45f);
        sectorsContainer.localScale = Vector3.one;

        var fullTile = IconService.Current.GetSpriteById(backImage);

        var mesh = GenerateMesh(width, height, size, new List<string> {backImage, backImage}, ignorablePositions:ignorablePositions);
        
        var meshGO = new GameObject("_background");
        var meshTransform = meshGO.transform;
        meshTransform.SetParent(sectorsContainer);
        meshTransform.localPosition = Vector3.zero;
        meshTransform.localScale = new Vector3(1f, 1f, 1f);
        meshTransform.localRotation = Quaternion.identity;

        var meshRenderer = meshGO.AddComponent<MeshRenderer>();
        var meshFilter = meshGO.AddComponent<MeshFilter>();

        

        meshFilter.mesh = mesh;

        // apply material 
        var material = new Material(Shader.Find("Sprites/Default"));
        material.renderQueue = 2000;

        // load texture
        var tileSprite = fullTile;
        var tileTexture = tileSprite == null ? null : tileSprite.texture;
        material.mainTexture = tileTexture;

        meshRenderer.material = material;
    }

    public void CreateBackgroundWater()
    {
        var waterPrefab = ContentService.Current.GetObjectByName(R.BackgroundWater);
        GameObject water = (GameObject) GameObject.Instantiate(waterPrefab);
    }
}