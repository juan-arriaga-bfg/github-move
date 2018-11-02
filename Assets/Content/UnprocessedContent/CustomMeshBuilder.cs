using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// public interface IBoardLayoutComponent
// {
//     BoardLayoutComponent BoardLayout { get; }
// }

public class CustomMeshBuilder : ECSEntity, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    protected BoardRenderer context;

    private MeshRenderer cachedGridMeshRenderer;

    private MeshRenderer cachedBorderMeshRenderer;

    private List<Vector3> cachedGridVertices;

    private Dictionary<BoardPosition, BoardElementView> cachedHighlights = new Dictionary<BoardPosition, BoardElementView>();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        this.context = entity as BoardRenderer;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    //0.2f, 0.5f, 0.3f
    // public void AnimateHighlight(BoardPosition point, float fadeIn = 0.2f, float fadeOut = 0.5f, float fadeOutDelay = -1f)
    // {
    //     BoardElementView highlightView;
    //     
    //     BoardPosition targetPoint = new BoardPosition(point.X, point.Y, 0);
    //
    //     if (cachedHighlights.TryGetValue(targetPoint, out highlightView))
    //     {
    //         
    //     }
    //     else
    //     {
    //         highlightView = context.CreateBoardElementAt<BoardElementView>
    //         (
    //             R.CellHighlight,
    //             targetPoint
    //         );
    //         
    //         cachedHighlights.Add(targetPoint, highlightView);
    //
    //         highlightView.FadeAlpha(0f, -1f, (_) => { });
    //         
    //         highlightView.FadeAlpha(0.5f, fadeIn, (_) =>
    //         {
    //             highlightView.FadeAlpha(0f, fadeOut, (__) =>
    //             {
    //                 cachedHighlights.Remove(targetPoint);
    //                 highlightView.DestroyOnBoard();
    //             });
    //         });
    //     }
    //
    // }

    public virtual bool IsHasCellAt(int[,] matrix, int x, int y)
    {
        int w = matrix.GetLength(0);
        int h = matrix.GetLength(1);
        if (x < 0 || x >= w) return false;
        if (y < 0 || y >= h) return false;
        return matrix[x, y] > 0;
    }

    public class LayoutDef
    {
        public int[,] Matrix;
        public Dictionary<string, string> SkinDef;
        public Transform Parent;
        public float BorderWidth = 0.2f;
        public Material BorderMaterial;
        public Material FillMaterial;
        public bool Fill = true;
    }
    
    public virtual void Build(LayoutDef def)
    {
        //var boardDef = context.Context.BoardDef;

        // generate mesh
        Mesh cornerMesh      = new Mesh();
        var  cornerVertices  = new List<Vector3>();
        var  cornerTris      = new List<int>();
        var  cornerUV        = new List<Vector2>();
        var  cornerColors    = new List<Color>();
        int  cornerCellIndex = 0;


        Mesh  cellsMesh   = new Mesh();
        var   vertices    = new List<Vector3>();
        var   tris        = new List<int>();
        var   uv          = new List<Vector2>();
        var   colors      = new List<Color>();
        int   cellIndex   = 0;

        int[,] matrix = def.Matrix;
        float borderWidth = def.BorderWidth;
        float cornerSize  = borderWidth;
        Dictionary<string, string> skinDef = def.SkinDef;
        float offsetFromCell = -0.05f;
        
        
        var   defaultColor = Color.white;
        float grayCoef     = 256f;
        var   grayColor    = new Color(grayCoef / 256f, grayCoef / 256f, grayCoef / 256f, 1f);

        // for (int x = 0; x < boardDef.Width; x++)
        // {
        //     for (int y = 0; y < boardDef.Height; y++)
        int fieldW = matrix.GetLength(0);
        int fieldH = matrix.GetLength(1);
        for (int x = 0; x < fieldW; x++)
        {
            for (int y = 0; y < fieldH; y++)
            {
                if (IsHasCellAt(matrix, x, y))
                {
                    var fullTile = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
                    if ((x + y) % 2 == 0)
                    {
                        fullTile = IconService.Current.GetSpriteById(skinDef["cell_tile_full_1"]);
                    }

                    vertices.Add(new Vector3(x,     y + 1, 0));
                    vertices.Add(new Vector3(x + 1, y + 1, 0));
                    vertices.Add(new Vector3(x,     y,     0));
                    vertices.Add(new Vector3(x + 1, y,     0));


                    tris.Add(cellIndex);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 2);

                    tris.Add(cellIndex + 2);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 3);


                    if (IsHasCellAt(matrix, x, y + 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(matrix, x - 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(matrix, x + 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(matrix, x, y - 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(matrix, x + 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(matrix, x, y + 1) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else
                    {
                        colors.Add(defaultColor);
                    }

                    if (IsHasCellAt(matrix, x - 1, y) == false)
                    {
                        colors.Add(grayColor);
                    }
                    else if (IsHasCellAt(matrix, x, y - 1) == false)
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
                // LEFT
                if (IsHasCellAt(matrix, x - 1, y) == false && IsHasCellAt(matrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_00"]);

                    float offsetYBottom = 0f;
                    float offsetYTop    = 0f;

                    bool isOuterCornerBottom = false; 
                    bool isOuterCornerTop = false;
                    
                    if ((IsHasCellAt(matrix, x, y - 1) || IsHasCellAt(matrix, x - 1, y - 1))
                     && (IsHasCellAt(matrix, x, y - 1) && IsHasCellAt(matrix, x - 1, y - 1)) == false)
                    {
                        offsetYBottom = 0f;
                    }
                    else
                    {
                        isOuterCornerBottom = true;
                        offsetYBottom = cornerSize;
                    }

                    if ((IsHasCellAt(matrix, x, y + 1) || IsHasCellAt(matrix, x - 1, y + 1))
                     && (IsHasCellAt(matrix, x, y + 1) && IsHasCellAt(matrix, x - 1, y + 1)) == false)
                    {
                        offsetYTop = 0f;
                    }
                    else
                    {
                        isOuterCornerTop = true;
                        offsetYTop = cornerSize;
                    }

                    bool isInnerCornerBottom = IsHasCellAt(matrix, x - 1, y - 1) && IsHasCellAt(matrix, x, y - 1);
                    bool isInnerCornerTop    = IsHasCellAt(matrix, x - 1, y + 1) && IsHasCellAt(matrix, x, y + 1);

                    // draw border for x0 y0
                    Vector3 tl = new Vector3(x - borderWidth, y + 1 - offsetYTop, 0);
                    Vector3 tr = new Vector3(x,               y + 1 - offsetYTop, 0);
                    Vector3 br = new Vector3(x - borderWidth, y + offsetYBottom,  0);
                    Vector3 bl = new Vector3(x,               y + offsetYBottom,  0); 

                    Vector3 xOffset = new Vector3(offsetFromCell, 0, 0);
                    tl -= xOffset;
                    tr -= xOffset;
                    br -= xOffset;
                    bl -= xOffset;

                    float topOffsetFromCell = 0f;
                    if (isInnerCornerTop)
                    {
                        topOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerTop)
                    {
                        topOffsetFromCell = offsetFromCell;
                    }
                    
                    float bottomOffsetFromCell = 0f;
                    if (isInnerCornerBottom)
                    {
                        bottomOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerBottom)
                    {
                        bottomOffsetFromCell = offsetFromCell;
                    }
                    
                    Vector3 yOffsetTop = new Vector3(0, topOffsetFromCell, 0);
                    tl += yOffsetTop;
                    tr += yOffsetTop;
                    
                    Vector3 yOffsetBottom = new Vector3(0, bottomOffsetFromCell, 0);
                    br -= yOffsetBottom;
                    bl -= yOffsetBottom;
                    
                    cornerVertices.Add(tl);
                    cornerVertices.Add(tr);
                    cornerVertices.Add(br);
                    cornerVertices.Add(bl);

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

                // RIGHT
                if (IsHasCellAt(matrix, x + 1, y) == false && IsHasCellAt(matrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_01"]);

                    float offsetYBottom = 0f;
                    float offsetYTop    = 0f;

                    bool isOuterCornerBottom = false; 
                    bool isOuterCornerTop = false;
                    
                    if ((IsHasCellAt(matrix, x, y - 1) || IsHasCellAt(matrix, x + 1, y - 1))
                     && (IsHasCellAt(matrix, x, y - 1) && IsHasCellAt(matrix, x + 1, y - 1)) == false)
                    {
                        offsetYBottom = 0f;
                    }
                    else
                    {
                        isOuterCornerBottom = true;
                        offsetYBottom = cornerSize;
                    }

                    if ((IsHasCellAt(matrix, x, y + 1) || IsHasCellAt(matrix, x + 1, y + 1))
                     && ((IsHasCellAt(matrix, x, y + 1) && IsHasCellAt(matrix, x + 1, y + 1)) == false))
                    {
                        offsetYTop = 0f;
                    }
                    else
                    {
                        isOuterCornerTop = true;
                        offsetYTop = cornerSize;
                    }

                    bool isInnerCornerBottom = IsHasCellAt(matrix, x + 1, y - 1) && IsHasCellAt(matrix, x, y - 1);
                    bool isInnerCornerTop    = IsHasCellAt(matrix, x + 1, y + 1) && IsHasCellAt(matrix, x, y + 1);

                    Vector3 tl = new Vector3(x + 1,               y + 1 - offsetYTop, 0);
                    Vector3 tr = new Vector3(x + 1 + borderWidth, y + 1 - offsetYTop, 0);
                    Vector3 br = new Vector3(x + 1,               y + offsetYBottom , 0);
                    Vector3 bl = new Vector3(x + 1 + borderWidth, y + offsetYBottom , 0);

                    Vector3 xOffset = new Vector3(offsetFromCell, 0, 0);
                    tl += xOffset;
                    tr += xOffset;
                    br += xOffset;
                    bl += xOffset;

                    float topOffsetFromCell = 0f;
                    if (isInnerCornerTop)
                    {
                        topOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerTop)
                    {
                        topOffsetFromCell = offsetFromCell;
                    }
                    
                    float bottomOffsetFromCell = 0f;
                    if (isInnerCornerBottom)
                    {
                        bottomOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerBottom)
                    {
                        bottomOffsetFromCell = offsetFromCell;
                    }
                    
                    Vector3 yOffsetTop = new Vector3(0, topOffsetFromCell, 0);
                    tl += yOffsetTop;
                    tr += yOffsetTop;
                    
                    Vector3 yOffsetBottom = new Vector3(0, bottomOffsetFromCell, 0);
                    br -= yOffsetBottom;
                    bl -= yOffsetBottom;
                    
                    cornerVertices.Add(tl);
                    cornerVertices.Add(tr);
                    cornerVertices.Add(br);
                    cornerVertices.Add(bl);
                    
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

                //BOTTOM
                if (IsHasCellAt(matrix, x, y - 1) == false && IsHasCellAt(matrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_02"]);

                    // float offsetXLeft  = 0f;
                    // float offsetXRight = 0f;
                    //
                    // if ((IsHasCellAt(matrix, x - 1, y) || IsHasCellAt(matrix, x - 1, y - 1))
                    //  && (IsHasCellAt(matrix, x - 1, y) && IsHasCellAt(matrix, x - 1, y - 1)) == false)
                    // {
                    //     offsetXLeft = 0f;
                    // }
                    // else
                    // {
                    //     offsetXLeft = cornerSize;
                    // }
                    //
                    // if ((IsHasCellAt(matrix, x + 1, y) || IsHasCellAt(matrix, x + 1, y - 1))
                    //  && (IsHasCellAt(matrix, x + 1, y) && IsHasCellAt(matrix, x + 1, y - 1)) == false)
                    // {
                    //     offsetXRight = 0f;
                    // }
                    // else
                    // {
                    //     offsetXRight = cornerSize;
                    // }
                    //
                    // // draw border for x0 y0
                    // cornerVertices.Add(new Vector3(x +     offsetXLeft  - offsetFromCell, - offsetFromCell + y,               0));
                    // cornerVertices.Add(new Vector3(x + 1 - offsetXRight + offsetFromCell, - offsetFromCell + y,               0));
                    // cornerVertices.Add(new Vector3(x +     offsetXLeft  - offsetFromCell, - offsetFromCell + y - borderWidth, 0));
                    // cornerVertices.Add(new Vector3(x + 1 - offsetXRight + offsetFromCell, - offsetFromCell + y - borderWidth, 0));

                    float offsetXLeft  = 0f;
                    float offsetXRight = 0f;

                    bool isOuterCornerLeft = false; 
                    bool isOuterCornerRight = false;
                    
                    if ((IsHasCellAt(matrix, x - 1, y) || IsHasCellAt(matrix, x - 1, y - 1))
                     && (IsHasCellAt(matrix, x - 1, y) && IsHasCellAt(matrix, x - 1, y - 1)) == false)
                    {
                        offsetXLeft = 0f;
                    }
                    else
                    {
                        isOuterCornerLeft = true;
                        offsetXLeft = cornerSize;
                    }

                    if ((IsHasCellAt(matrix, x + 1, y) || IsHasCellAt(matrix, x + 1, y - 1))
                     && (IsHasCellAt(matrix, x + 1, y) && IsHasCellAt(matrix, x + 1, y - 1)) == false)
                    {
                        offsetXRight = 0f;
                    }
                    else
                    {
                        isOuterCornerRight = true;
                        offsetXRight = cornerSize;
                    }

                    bool isInnerCornerLeft   = IsHasCellAt(matrix, x - 1, y - 1) && IsHasCellAt(matrix, x - 1, y);
                    bool isInnerCornerRight  = IsHasCellAt(matrix, x + 1, y - 1) && IsHasCellAt(matrix, x + 1, y);

                    Vector3 tl = new Vector3(x +     offsetXLeft , y,               0);
                    Vector3 tr = new Vector3(x + 1 - offsetXRight, y,               0);
                    Vector3 bl = new Vector3(x +     offsetXLeft , y - borderWidth, 0);
                    Vector3 br = new Vector3(x + 1 - offsetXRight, y - borderWidth, 0);

                    Vector3 yOffset = new Vector3(0, offsetFromCell, 0);
                    tl -= yOffset;
                    tr -= yOffset;
                    br -= yOffset;
                    bl -= yOffset;

                    float rightOffsetFromCell = 0f;
                    if (isInnerCornerRight)
                    {
                        rightOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerRight)
                    {
                        rightOffsetFromCell = offsetFromCell;
                    }
                    
                    float leftOffsetFromCell = 0f;
                    if (isInnerCornerLeft)
                    {
                        leftOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerLeft)
                    {
                        leftOffsetFromCell = offsetFromCell;
                    }
                    
                    Vector3 xOffsetRight = new Vector3(rightOffsetFromCell, 0 , 0);
                    tr += xOffsetRight;
                    br += xOffsetRight;

                    Vector3 xOffsetLeft = new Vector3(leftOffsetFromCell, 0, 0);
                    tl -= xOffsetLeft;
                    bl -= xOffsetLeft;
                    
                    cornerVertices.Add(tl);
                    cornerVertices.Add(tr);
                    cornerVertices.Add(bl);
                    cornerVertices.Add(br);
                    
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

                    if (cornerVertices.Count != cornerUV.Count)
                    {
                        int asfasd = 0;
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }

                // TOP
                if (IsHasCellAt(matrix, x, y + 1) == false && IsHasCellAt(matrix, x, y))
                {
                    var borderTile = IconService.Current.GetSpriteById(skinDef["cell_tile_03"]);

                    float offsetXLeft  = 0f;
                    float offsetXRight = 0f;

                    bool isOuterCornerLeft = false; 
                    bool isOuterCornerRight = false;
                    
                    if ((IsHasCellAt(matrix, x - 1, y) || IsHasCellAt(matrix, x - 1, y + 1))
                     && (IsHasCellAt(matrix, x - 1, y) && IsHasCellAt(matrix, x - 1, y + 1)) == false)
                    {
                        offsetXLeft = 0f;
                    }
                    else
                    {
                        isOuterCornerLeft = true;
                        offsetXLeft = cornerSize;
                    }

                    if ((IsHasCellAt(matrix, x + 1, y) || IsHasCellAt(matrix, x + 1, y + 1))
                     && (IsHasCellAt(matrix, x + 1, y) && IsHasCellAt(matrix, x + 1, y + 1)) == false)
                    {
                        offsetXRight = 0f;
                    }
                    else
                    {
                        isOuterCornerRight = true;
                        offsetXRight = cornerSize;
                    }

                    bool isInnerCornerLeft   = IsHasCellAt(matrix, x - 1, y + 1) && IsHasCellAt(matrix, x - 1, y);
                    bool isInnerCornerRight  = IsHasCellAt(matrix, x + 1, y + 1) && IsHasCellAt(matrix, x + 1, y);

                    Vector3 tl = new Vector3(x +     offsetXLeft , y + 1 + borderWidth, 0);
                    Vector3 tr = new Vector3(x + 1 - offsetXRight, y + 1 + borderWidth, 0);
                    Vector3 bl = new Vector3(x +     offsetXLeft , y + 1,               0);
                    Vector3 br = new Vector3(x + 1 - offsetXRight, y + 1,               0);

                    Vector3 yOffset = new Vector3(0, offsetFromCell, 0);
                    tl += yOffset;
                    tr += yOffset;
                    br += yOffset;
                    bl += yOffset;

                    float rightOffsetFromCell = 0f;
                    if (isInnerCornerRight)
                    {
                        rightOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerRight)
                    {
                        rightOffsetFromCell = offsetFromCell;
                    }
                    
                    float leftOffsetFromCell = 0f;
                    if (isInnerCornerLeft)
                    {
                        leftOffsetFromCell = -offsetFromCell;
                    }
                    else if (isOuterCornerLeft)
                    {
                        leftOffsetFromCell = offsetFromCell;
                    }
                    
                    Vector3 xOffsetRight = new Vector3(rightOffsetFromCell, 0 , 0);
                    tr += xOffsetRight;
                    br += xOffsetRight;

                    Vector3 xOffsetLeft = new Vector3(leftOffsetFromCell, 0, 0);
                    tl -= xOffsetLeft;
                    bl -= xOffsetLeft;
                    
                    cornerVertices.Add(tl);
                    cornerVertices.Add(tr);
                    cornerVertices.Add(bl);
                    cornerVertices.Add(br);
                    
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

                    if (cornerVertices.Count != cornerUV.Count)
                    {
                        int asfasd = 0;
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }

#region Inner Corners
                
                // TOP RIGHT
                if (IsHasCellAt(matrix, x - 1, y - 1) == false
                 && IsHasCellAt(matrix, x - 1, y)
                 && IsHasCellAt(matrix, x,     y - 1))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_10"]);

                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y              - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              - offsetFromCell, y              - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y - cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              - offsetFromCell, y - cornerSize - offsetFromCell, 0));

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

                    if (cornerVertices.Count != cornerUV.Count)
                    {
                        int asfasd = 0;
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }

                //TOP LEFT
                if (IsHasCellAt(matrix, x, y - 1) == false
                 && IsHasCellAt(matrix, x - 1, y - 1)
                 && IsHasCellAt(matrix, x,     y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_20"]);

                    cornerVertices.Add(new Vector3(x              + offsetFromCell, y              - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + offsetFromCell, y              - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              + offsetFromCell, y - cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + offsetFromCell, y - cornerSize - offsetFromCell, 0));

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

                //BOTTOM LEFT
                if (IsHasCellAt(matrix, x, y + 1) == false
                 && IsHasCellAt(matrix, x - 1, y + 1)
                 && IsHasCellAt(matrix, x,     y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_30"]);

                    cornerVertices.Add(new Vector3(x              + offsetFromCell, y + 1 + cornerSize + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + offsetFromCell, y + 1 + cornerSize + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              + offsetFromCell, y + 1              + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + offsetFromCell, y + 1              + offsetFromCell, 0));

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

                    if (cornerVertices.Count != cornerUV.Count)
                    {
                        int asfasd = 0;
                    }

                    cornerCellIndex = cornerCellIndex + 4;
                }

                if (IsHasCellAt(matrix, x - 1, y) == false
                 && IsHasCellAt(matrix, x - 1, y - 1)
                 && IsHasCellAt(matrix, x,     y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_40"]);

                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y + cornerSize + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              - offsetFromCell, y + cornerSize + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y              + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x              - offsetFromCell, y              + offsetFromCell, 0));

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
#endregion

#region Outter Corners
                // BOTTOM LEFT
                if (IsHasCellAt(matrix, x - 1, y) == false
                 && IsHasCellAt(matrix, x,     y - 1) == false
                 && IsHasCellAt(matrix, x - 1, y - 1) == false
                 && IsHasCellAt(matrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_11"]);

                    cornerVertices.Add(new Vector3(x - offsetFromCell - cornerSize, y + cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - offsetFromCell + cornerSize, y + cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - offsetFromCell - cornerSize, y - cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - offsetFromCell + cornerSize, y - cornerSize - offsetFromCell, 0));

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

                // TOP LEFT
                if (IsHasCellAt(matrix, x - 1, y) == false
                 && IsHasCellAt(matrix, x - 1, y + 1) == false
                 && IsHasCellAt(matrix, x,     y + 1) == false
                 && IsHasCellAt(matrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_14"]);

                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y + cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize - offsetFromCell, y + cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize - offsetFromCell, y - cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize - offsetFromCell, y - cornerSize + 1 + offsetFromCell, 0));

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

                if (IsHasCellAt(matrix, x + 1, y) == false
                 && IsHasCellAt(matrix, x + 1, y + 1) == false
                 && IsHasCellAt(matrix, x,     y + 1) == false
                 && IsHasCellAt(matrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_13"]);

                    cornerVertices.Add(new Vector3(x - cornerSize + 1 + offsetFromCell, y + cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1 + offsetFromCell, y + cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize + 1 + offsetFromCell, y - cornerSize + 1 + offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1 + offsetFromCell, y - cornerSize + 1 + offsetFromCell, 0));

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

                // BOTTOM RIGHT
                if (IsHasCellAt(matrix, x + 1, y) == false
                 && IsHasCellAt(matrix, x + 1, y - 1) == false
                 && IsHasCellAt(matrix, x,     y - 1) == false
                 && IsHasCellAt(matrix, x, y))
                {
                    var cornerTile = IconService.Current.GetSpriteById(skinDef["cell_tile_12"]);

                    cornerVertices.Add(new Vector3(x - cornerSize + 1 + offsetFromCell, y + cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1 + offsetFromCell, y + cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x - cornerSize + 1 + offsetFromCell, y - cornerSize - offsetFromCell, 0));
                    cornerVertices.Add(new Vector3(x + cornerSize + 1 + offsetFromCell, y - cornerSize - offsetFromCell, 0));

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
#endregion
            }
        }

        cachedGridVertices = vertices;

        cellsMesh.vertices = vertices.ToArray();
        cellsMesh.triangles = tris.ToArray();
        cellsMesh.uv = uv.ToArray();
        cellsMesh.colors = colors.ToArray();

        cellsMesh.RecalculateBounds();


        var meshGO = new GameObject("Fill");
        var meshTransform = meshGO.transform;
        meshTransform.SetParent(def.Parent != null ? def.Parent : context.ViewRoot, false);
        meshTransform.localPosition = Vector3.zero;

        var meshRenderer = meshGO.AddComponent<MeshRenderer>();
        var meshFilter   = meshGO.AddComponent<MeshFilter>();

        cachedGridMeshRenderer = meshRenderer;

        //meshTransform.localScale = new Vector3(127.4f, 127.4f, 1f);

        meshFilter.mesh = cellsMesh;

        // apply material 
        var material = def.FillMaterial;

        // load texture
        var tileSprite  = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
        var tileTexture = tileSprite == null ? null : tileSprite.texture;
        material.mainTexture = tileTexture;

        meshRenderer.material = material;

        // size 
        // var boundsOffset = new Vector3(-meshRenderer.bounds.extents.x, -meshRenderer.bounds.extents.y, 0f);
        // var localOffset  = new Vector3(0f,                             -61f,                           0f);
        // meshTransform.localPosition = boundsOffset + localOffset;


        // corners
        cornerMesh.vertices = cornerVertices.ToArray();
        cornerMesh.triangles = cornerTris.ToArray();
        cornerMesh.uv = cornerUV.ToArray();
        cornerMesh.colors = cornerColors.ToArray();

        cornerMesh.RecalculateBounds();

        var cornerMeshGO        = new GameObject("Border");
        var cornerMeshTransform = cornerMeshGO.transform;
        cornerMeshTransform.SetParent(meshTransform);

        var cornerMeshRenderer = cornerMeshGO.AddComponent<MeshRenderer>();
        var cornerMeshFilter   = cornerMeshGO.AddComponent<MeshFilter>();

        cachedBorderMeshRenderer = cornerMeshRenderer;

        cornerMeshTransform.localPosition = new Vector3(0f, 0f, 0f);
        cornerMeshTransform.localScale    = new Vector3(1f, 1f, 1f);

        cornerMeshFilter.mesh = cornerMesh;

        // apply material 
        var cornerMaterial = def.BorderMaterial;

        // load texture
        var cornerTileSprite  = IconService.Current.GetSpriteById(skinDef["cell_tile_full_0"]);
        var cornerTileTexture = cornerTileSprite == null ? null : cornerTileSprite.texture;
        cornerMaterial.mainTexture = cornerTileTexture;

        cornerMeshRenderer.material = cornerMaterial;

        //	    cachedHighlightVectors = new float[cachedGridVertices.Count];
    }

    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
        //        UpdateHighlight();
    }

    public object GetDependency()
    {
        return context.Context;
    }
}