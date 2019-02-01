using DG.Tweening;
using UnityEngine;

public class FogSectorsView// : BoardElementView
{
    private static GameObject fogSectors;

    // public override void Init(BoardRenderer context)
    // {
    //     base.Init(context);
    // }
    
    public void Init(BoardRenderer context)
    {
        Context = context;
    }

    public BoardRenderer Context { get; set; }

    public void UpdateFogSectorsMesh()
    {
        GameObject oldFogSectors = fogSectors;
        
        Sprite lineSprite    = IconService.Current.GetSpriteById("LINE");
        Sprite corner2Sprite = IconService.Current.GetSpriteById("CORNER_0_1");
        Sprite corner3Sprite = IconService.Current.GetSpriteById("CORNER_0_1_2");
        Sprite corner4Sprite = IconService.Current.GetSpriteById("CORNER_0_1_2_3");
        
        var boardDef = BoardService.Current.FirstBoard.BoardDef;
        int boardW   = boardDef.Width;
        int boardH   = boardDef.Height;
        
        var meshBuilder = new GridMeshBuilder();
        var def = new GridMeshBuilderDef
        {
            FieldWidth = boardW,
            FieldHeight = boardH,
            Areas = GameDataService.Current.FogsManager.GetFoggedAreas(),
            LineSprite = lineSprite,
            Corner2Sprite = corner2Sprite,
            Corner3Sprite = corner3Sprite,
            Corner4Sprite = corner4Sprite,
            LineWidth = 0.5f,
            FadeNearExcluded = true,
        };
        
        var mesh = meshBuilder.Build(def);

        fogSectors = new GameObject("FogSectors");
        var meshTransform = fogSectors.transform;
        meshTransform.SetParent(Context.SectorsContainer, false);
        meshTransform.localPosition = new Vector3(-0.71f, 0.56f, 0);
        meshTransform.localScale = Vector3.one * 1.8f;

        var meshRenderer = fogSectors.AddComponent<MeshRenderer>();
        var meshFilter   = fogSectors.AddComponent<MeshFilter>();

        meshFilter.mesh = mesh;

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            renderQueue = 3000
        };

        meshRenderer.material = mat;
        meshRenderer.material.mainTexture = lineSprite.texture;

        meshRenderer.sortingOrder =  BoardLayer.GetDefaultLayerIndexBy(new BoardPosition(0, 0, BoardLayer.PieceUP1.Layer), Context.Context.BoardDef.Width, Context.Context.BoardDef.Height);
        meshRenderer.sortingLayerName = "Default";

        if (oldFogSectors != null)
        {
            Animate(oldFogSectors, fogSectors);
        }
    }

    private void Animate(GameObject oldSectors, GameObject newSectors)
    {
        MeshRenderer oldRenderer = oldSectors.GetComponent<MeshRenderer>();
        MeshRenderer newRenderer = newSectors.GetComponent<MeshRenderer>();

        oldRenderer.material.DOFade(0, 0.5f)
                    .OnComplete(() => { GameObject.Destroy(oldSectors); });
        
        newRenderer.material.color = new Color(1,1,1,0);

        newRenderer.material.DOFade(1, 0.5f);
    }
}