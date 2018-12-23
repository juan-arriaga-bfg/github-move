using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GridMeshArea
{
    public int[,] Matrix;
    public int X;
    public int Y;
    public bool Exclude;
}

public class GridMeshBuilderDef
{
    public int FieldWidth;
    public int FieldHeight;
    public List<GridMeshArea> Areas;
    
    public Sprite LineSprite;
    public Sprite Corner2Sprite;
    public Sprite Corner3Sprite;
    public Sprite Corner4Sprite;

    public float LineWidth;

    public bool FadeNearExcluded;
}

public class GridMeshBuilder
{
    private GridMeshBuilderDef def;
    private int[,] edgesData;

    private int BIT_TOP = 0;
    private int BIT_RIGHT = 1;
    private int BIT_BOTTOM = 2;
    private int BIT_LEFT = 3;
    private int BIT_EXCLUDED_VERTEX = 4;
   
    private readonly Color WHITE_COLOR = new Color(1, 1, 1, 1);
    private readonly Color TRANSPARENT_COLOR = new Color(1, 1, 1, 0);
    
    private void DebugRenderLine(float fromX, float fromY, float toX, float toY, Color color, string name)
    {
        var lrGoPrefab = GameObject.Find("LineRendererPrefab");
        var lrGoHost = GameObject.Find(name);

        var go = GameObject.Instantiate(lrGoPrefab);;
        go.name = name;
        go.transform.SetParent(lrGoHost.transform);

        var lr = go.GetComponent<LineRenderer>();
        lr.SetWidth(0.05f, 0.05f);
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(fromX, fromY, 0));
        lr.SetPosition(1, new Vector3(toX, toY, 0));
        lr.startColor = color;
        lr.endColor = color;
    }

    private void DebugDrawEdges(int index)
    {
        for (int x = 0; x < edgesData.GetLength(0); x++)
        {
            for (int y = 0; y < edgesData.GetLength(1); y++)
            {
                int data = edgesData[x, y];
                if (index == 0 && BitsHelper.IsBitSet(data, BIT_TOP))    { DebugRenderLine(x,  y,  x,     y + 1, Color.red,    "up"); }
                if (index == 1 && BitsHelper.IsBitSet(data, BIT_RIGHT))  { DebugRenderLine(x,  y,  x + 1, y,     Color.blue,   "right"); }
                if (index == 2 && BitsHelper.IsBitSet(data, BIT_BOTTOM)) { DebugRenderLine(x , y,  x ,    y - 1, Color.green,  "down"); }
                if (index == 3 && BitsHelper.IsBitSet(data, BIT_LEFT))   { DebugRenderLine(x,  y , x - 1, y ,    Color.yellow, "left"); }
            }
        }
    }
    
    public Mesh Build(GridMeshBuilderDef def)
    {
        var sw = new Stopwatch();
        sw.Start();
        
        this.def = def;
        CreateEdgesData();
        //
        // for (int i = 0; i <= 3; i++)
        // {
        //     DebugDrawEdges(i);
        // }
        // GameObject.Find("LineRendererPrefab").SetActive(false);

        var mesh = CreateMesh();
        
        sw.Stop();
        Debug.Log($"[GridMeshBuilder] => Mesh created in {sw.ElapsedMilliseconds}ms");
        
        return mesh;
    }

    private bool HasCell(ref int[,] data, int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x >= data.GetLength(0) || y >= data.GetLength(1))
        {
            return false;
        }

        return data[x, y] > 0;
    }

    private bool HasLeftNeighbor(ref int[,] data, int x, int y)
    {
        return HasCell(ref data, x - 1, y);
    }
    
    private bool HasRightNeighbor(ref int[,] data, int x, int y)
    {
        return HasCell(ref data, x + 1, y);
    }
    
    private bool HasTopNeighbor(ref int[,] data, int x, int y)
    {
        return HasCell(ref data, x, y + 1);
    }
    
    private bool HasBottomNeighbor(ref int[,] data, int x, int y)
    {
        return HasCell(ref data, x, y - 1);
    }
    
    private bool HasBottomLeftNeighbor(ref int[,] data, int x, int y)
    {
        return HasCell(ref data, x - 1, y - 1);
    }

    private void CreateEdgesData()
    {
        edgesData = new int[def.FieldWidth + 1,def.FieldHeight + 1];

        def.Areas.Sort (delegate(GridMeshArea area1, GridMeshArea area2)
        {
            if (area1.Exclude == area2.Exclude) return 0;
            if (area1.Exclude && !area2.Exclude) return 1;
            return -1;
        });
        
        for (var areaIndex = 0; areaIndex < def.Areas.Count; areaIndex++)
        {
            GridMeshArea area  = def.Areas[areaIndex];
            
            var  matrix  = area.Matrix;
            int  areaW = matrix.GetLength(0);
            int  areaH = matrix.GetLength(1);

            bool enable = !area.Exclude;
            
            for (int areaX = 0; areaX < areaW + 1; areaX++)
            {
                for (int areaY = 0; areaY < areaH + 1; areaY++)
                {
                    int worldX = areaX + area.X;
                    int worldY = areaY + area.Y;
                    
                    int edgeData = edgesData[worldX, worldY];

                    int edgeDataOld = edgeData;
                    
                    bool hasLeftNeighbor       = HasLeftNeighbor(ref matrix, areaX, areaY);
                    bool hasBottomNeighbor     = HasBottomNeighbor(ref matrix, areaX, areaY);
                    bool hasBottomLeftNeighbor = HasBottomLeftNeighbor(ref matrix, areaX, areaY);
                    
#region TOP and RIGHT

                    if (HasCell(ref matrix, areaX, areaY))
                    {
                        if (!hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_TOP, enable);
                        }

                        if (!hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_RIGHT, enable);
                        }
                    }
                    else if (areaX == areaW)
                    {
                        if (hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_TOP, enable);
                        }
                    }
                    else if (areaY == areaH)
                    {
                        if (hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_RIGHT, enable);
                        }
                    }
                    else
                    {
                        if (hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_RIGHT, enable);
                        }

                        if (hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_TOP, enable);
                        }
                    }

#endregion

#region BOTTOM and LEFT

                    if (hasBottomLeftNeighbor)
                    {
                        if (!hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_LEFT, enable);
                        }

                        if (!hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_BOTTOM, enable);
                        }
                    }
                    else if (areaX == 0)
                    {
                        if (hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_BOTTOM, enable);
                        }
                    }
                    else if (areaY == 0)
                    {
                        if (hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_LEFT, enable);
                        }
                    }
                    else
                    {
                        if (hasBottomNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_BOTTOM, enable);
                        }

                        if (hasLeftNeighbor)
                        {
                            edgeData = BitsHelper.ToggleBit(edgeData, BIT_LEFT, enable);
                        }
                    }

#endregion
                    if (edgeData != edgeDataOld && !enable)
                    {
                        edgeData = BitsHelper.ToggleBit(edgeData, BIT_EXCLUDED_VERTEX, true);
                    }

                    edgesData[worldX, worldY] = edgeData;
                }
            }
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
    }
 
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        return rotation * (point - pivot) + pivot;
    }
    
    private void GenerateQuad(float x, 
                              float y, 
                              Quaternion rotation,
                              Vector3 rotationPivot,
                              float width,
                              float height,
                              float cropNear,
                              float cropFar,
                              Sprite sprite,
                              ref int triangleIndex,
                              ref List<Vector3> vertices,
                              ref List<int> triangles,
                              ref List<Vector2> uvs,
                              ref List<Color> colors)
    {
        Color  defaultColor = Color.white;

        float halfWidth = width / 2f;

        Vector3 tl = new Vector3(x - halfWidth, y + height - cropFar, 0);
        Vector3 tr = new Vector3(x + halfWidth, y + height - cropFar, 0);
        Vector3 br = new Vector3(x + halfWidth, y + cropNear         ,0);
        Vector3 bl = new Vector3(x - halfWidth, y + cropNear         ,0);

        tl = RotatePointAroundPivot(tl, rotationPivot, rotation);
        tr = RotatePointAroundPivot(tr, rotationPivot, rotation);
        br = RotatePointAroundPivot(br, rotationPivot, rotation);
        bl = RotatePointAroundPivot(bl, rotationPivot, rotation);

        vertices.Add(tl);
        vertices.Add(tr);
        vertices.Add(bl);
        vertices.Add(br);

        triangles.Add(triangleIndex + 0);
        triangles.Add(triangleIndex + 1);
        triangles.Add(triangleIndex + 2);
        //
        triangles.Add(triangleIndex + 3);
        triangles.Add(triangleIndex + 2);
        triangles.Add(triangleIndex + 1);

        triangleIndex += 4;

        for (int i = 0; i < sprite.uv.Length; i++)
        {
            uvs.Add(sprite.uv[i]);
            colors.Add(defaultColor);
        }
    }
    
    private Mesh CreateMesh()
    {
        float lineWidth = def.LineWidth;
        float lineHeight = 1f;
        float halfLineWidth = lineWidth / 2f;
        
        Mesh mesh = new Mesh();
        var  vertices  = new List<Vector3>();
        var  triangles = new List<int>();
        var  uvs       = new List<Vector2>();
        var  colors    = new List<Color>();

        int triangleIndex = 0;

        Quaternion rotation0   = Quaternion.Euler(new Vector3(0, 0, 0));
        Quaternion rotation90  = Quaternion.Euler(new Vector3(0, 0, -90));
        Quaternion rotation180 = Quaternion.Euler(new Vector3(0, 0, -180));
        Quaternion rotation270 = Quaternion.Euler(new Vector3(0, 0, -270));

        int edgesArrayW = edgesData.GetLength(0);
        int edgesArrayH = edgesData.GetLength(1);
        
        for (int x = 0; x < edgesArrayW; x++)
        {
            for (int y = 0; y < edgesArrayH; y++)
            {
                int edgeData = edgesData[x, y];

                if (edgeData == 0)
                {
                    continue;
                }

                bool isTopEdge    = BitsHelper.IsBitSet(edgeData, BIT_TOP);
                bool isRightEdge  = BitsHelper.IsBitSet(edgeData, BIT_RIGHT);
                bool isBottomEdge = BitsHelper.IsBitSet(edgeData, BIT_BOTTOM);
                bool isLeftEdge   = BitsHelper.IsBitSet(edgeData, BIT_LEFT);
               
                // TOP EDGE
                if (isTopEdge)
                {
                    float near = isRightEdge || isLeftEdge ? halfLineWidth : 0;
                    float far = 0;
                    if (y + 1 < edgesArrayH)
                    {
                        int nextEdgeData = edgesData[x, y + 1];
                        bool isNextRightEdge  = BitsHelper.IsBitSet(nextEdgeData, BIT_RIGHT);
                        bool isNextLeftEdge   = BitsHelper.IsBitSet(nextEdgeData, BIT_LEFT);
                        if (isNextLeftEdge || isNextRightEdge)
                        {
                            far = halfLineWidth;
                        }
                    }
                    
                    GenerateQuad(x, y, rotation0, new Vector3(x, y, 0), lineWidth, lineHeight, near, far, def.LineSprite, ref triangleIndex, ref vertices, ref triangles, ref uvs, ref colors);

                    if (def.FadeNearExcluded)
                    {
                        if (BitsHelper.IsBitSet(edgeData, BIT_EXCLUDED_VERTEX))
                        {
                            colors[colors.Count - 1] = TRANSPARENT_COLOR;
                            colors[colors.Count - 2] = TRANSPARENT_COLOR;
                        }

                        var topEdge = edgesData[x, y + 1];
                        if (BitsHelper.IsBitSet(topEdge, BIT_EXCLUDED_VERTEX))
                        {
                            colors[colors.Count - 3] = TRANSPARENT_COLOR;
                            colors[colors.Count - 4] = TRANSPARENT_COLOR;
                        }
                    }
                }
                
                // RIGHT EDGE
                if (isRightEdge)
                {
                    float near = isBottomEdge || isTopEdge ? halfLineWidth : 0;
                    float far = 0;
                    if (x + 1 < edgesArrayW)
                    {
                        int  nextEdgeData    = edgesData[x + 1, y];
                        bool isNextTopEdge = BitsHelper.IsBitSet(nextEdgeData, BIT_TOP);
                        bool isNextBottomEdge  = BitsHelper.IsBitSet(nextEdgeData, BIT_BOTTOM);
                        if (isNextTopEdge || isNextBottomEdge)
                        {
                            far = halfLineWidth;
                        }
                    }
                    
                    GenerateQuad(x, y, rotation90, new Vector3(x, y, 0), lineWidth, lineHeight, near, far, def.LineSprite, ref triangleIndex, ref vertices, ref triangles, ref uvs, ref colors);

                    if (def.FadeNearExcluded)
                    {
                        if (BitsHelper.IsBitSet(edgeData, BIT_EXCLUDED_VERTEX))
                        {
                            colors[colors.Count - 1] = TRANSPARENT_COLOR;
                            colors[colors.Count - 2] = TRANSPARENT_COLOR;
                        }

                        var rightEdge = edgesData[x + 1, y];
                        if (BitsHelper.IsBitSet(rightEdge, BIT_EXCLUDED_VERTEX))
                        {
                            colors[colors.Count - 3] = TRANSPARENT_COLOR;
                            colors[colors.Count - 4] = TRANSPARENT_COLOR;
                        }
                    }
                }

                // CORNERS
                int countOfEdges = BitsHelper.CountOfSetBits(edgeData & 15); // Get the first 4 bits and count how many of them is set to 1  

                if (def.FadeNearExcluded && BitsHelper.IsBitSet(edgeData, BIT_EXCLUDED_VERTEX))
                {
                    continue;
                }

                // Skip if no corners
                if (countOfEdges == 0 || countOfEdges == 1)
                {
                    continue;
                }
                
                // Skip if no corners
                if (countOfEdges == 2)
                {
                    if ((isTopEdge && isBottomEdge) || (isRightEdge && isLeftEdge))
                    {
                        continue;
                    }
                }
                
                Sprite cornerSprite = null;
                Quaternion rotation = rotation0;
                
                // Select sprite
                switch (countOfEdges)
                {
                    case 2:
                        cornerSprite = def.Corner2Sprite;
                        break;
                    
                    case 3:
                        cornerSprite = def.Corner3Sprite;
                        break;
                    
                    case 4:
                        cornerSprite = def.Corner4Sprite;
                        break;
                }
                
                // Select rotation
                switch (countOfEdges)
                {
                    case 2:
                        if (isTopEdge && isRightEdge)
                        {
                            rotation = rotation0;
                        }
                        else if (isRightEdge && isBottomEdge)
                        {
                            rotation = rotation90;
                        }
                        else if (isBottomEdge && isLeftEdge)
                        {
                            rotation = rotation180;
                        }
                        else if (isTopEdge && isLeftEdge)
                        {
                            rotation = rotation270;
                        }
                        break;
                    
                    case 3:
                        if (isTopEdge && isRightEdge && isBottomEdge)
                        {
                            rotation = rotation0;
                        }
                        else if (isRightEdge && isBottomEdge && isLeftEdge)
                        {
                            rotation = rotation90;
                        }
                        else if (isBottomEdge && isLeftEdge && isTopEdge)
                        {
                            rotation = rotation180;
                        }
                        else if (isTopEdge && isLeftEdge && isRightEdge)
                        {
                            rotation = rotation270;
                        }
                        
                        break;
                }

                GenerateQuad(x, y - halfLineWidth, rotation, new Vector3(x, y, 0), lineWidth, lineWidth, 0, 0, cornerSprite, ref triangleIndex, ref vertices, ref triangles, ref uvs, ref colors);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateBounds();

        return mesh;
    }
}