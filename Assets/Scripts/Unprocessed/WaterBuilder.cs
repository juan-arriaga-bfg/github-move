using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WaterBuilder : MonoBehaviour
{
    private void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var boardController = BoardService.Current.FirstBoard;

        meshFilter.mesh = GenerateWaterMesh(
            boardController.BoardDef.Width,
            boardController.BoardDef.Height,
            boardController.BoardDef.UnitSize,
            GameDataService.Current.FieldManager.LayoutData,
            1,
            10,
            10,
            30
        );

        transform.SetParent(boardController.RendererContext.SectorsContainer.transform, false);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        
        //MeshUtility.Optimize(meshFilter.mesh);
    }

    private Mesh GenerateWaterMesh(int width, 
                                   int height, 
                                   float size, 
                                   int[] layout, 
                                   int filledId, 
                                   int borderW, 
                                   int borderH,
                                   int cropCorners)
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
        float cellSize = size;
        var defaultColor = new Color(1f, 1f, 1f, 1f);

        float meshW = width  + borderW * 2;
        float meshH = height + borderH * 2;

        int fullWidth = width + borderW;
        int fullHeight = height + borderH;
        
        // Build layout
        int layoutIndex = 0;
        for (int x = -borderW; x < fullWidth; x++)
        {
            for (int y = -borderH; y < fullHeight; y++)
            {
                int idInLayout;
                if (x < 0 || y < 0 || x >= width || y >= height)
                {
                    idInLayout = filledId;
                }
                else
                {
                    idInLayout = layout[layoutIndex];
                    layoutIndex++;
                }
                
                if (cropCorners > 0)
                {
                    int left = x + borderW;
                    int right = fullWidth - x;
                    int bottom = y + borderH;
                    int top = (fullHeight - y);
                    
                    if (left + bottom < cropCorners)
                    {
                        continue;
                    }

                    if (right + bottom < cropCorners)
                    {
                        continue;
                    }

                    if (left + top < cropCorners)
                    {
                        continue;
                    }

                    if (right + top < cropCorners)
                    {
                        continue;
                    }
                }

                if (idInLayout == filledId)
                {
                    
                    Vector2 tl = new Vector2(x + 0, y + 1); 
                    Vector2 tr = new Vector2(x + 1, y + 1); 
                    Vector2 bl = new Vector2(x + 0, y + 0); 
                    Vector2 br = new Vector2(x + 1, y + 0);

                    vertices.Add(tl * cellSize);
                    vertices.Add(tr * cellSize);
                    vertices.Add(bl * cellSize);
                    vertices.Add(br * cellSize);

                    tris.Add(cellIndex);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 2);

                    tris.Add(cellIndex + 2);
                    tris.Add(cellIndex + 1);
                    tris.Add(cellIndex + 3);                         

                    var uvBl = new Vector2((bl.x + borderW) / meshW, (bl.y + borderH) / meshH);
                    var uvTl = new Vector2((tl.x + borderW) / meshW, (tl.y + borderH) / meshH);
                    var uvTr = new Vector2((tr.x + borderW) / meshW, (tr.y + borderH) / meshH);
                    var uvBr = new Vector2((br.x + borderW) / meshW, (br.y + borderH) / meshH);                    
                    
                    uv.Add(uvTl);                    
                    uv.Add(uvTr);  
                    uv.Add(uvBl);                                      
                    uv.Add(uvBr);                    
                    
                    // colors.Add(Color.red);
                    // colors.Add(Color.blue);
                    // colors.Add(Color.green);
                    // colors.Add(Color.cyan);
                    
                    
                    for (int i = 0; i < 4; i++)
                    {
                        colors.Add(defaultColor);
                    }
                    
                    cellIndex = cellIndex + 4;
                }
            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateBounds();

#if DEBUG
        sw.Stop();
        Debug.Log($"[WaterBuilder] => GenerateMesh: Done in {sw.ElapsedMilliseconds}ms");
#endif
        
        return mesh;
    }
}