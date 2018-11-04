using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevTools : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Start()
    {
        panel.SetActive(false);
    }
    
    public void OnToggleValueChanged(bool isChecked)
    {
        panel.SetActive(!isChecked);
    }

    public static void ReloadScene(bool resetProgress)
    {
        var manager = GameDataService.Current.QuestsManager;
        manager.DisconnectFromBoard();
            
        BoardService.Instance.SetManager(null);

        if (resetProgress)
        {
            var profileBuilder = new DefaultProfileBuilder();
            ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
        }

        GameDataService.Current.Reload();
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }
    }
    
    public void OnResetProgressClick()
    {
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Reset the progress";
        model.Message = "Do you want to reset the progress and start playing from the beginning?";
        model.AcceptLabel = "<size=30>Reset progress!</size>";
        model.CancelLabel = "No!";
        
        model.OnAccept = () =>
        {
            ReloadScene(true);
        };
        
        model.OnCancel = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }

    public void OnCurrencyCheatSheetClick()
    {
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.CurrencyCheatSheetWindow);
        UIService.Get.ShowWindow(UIWindowType.CurrencyCheatSheetWindow);
    }
    
    public void OnPiecesCheatSheetClick()
    {
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.PiecesCheatSheetWindow);
        UIService.Get.ShowWindow(UIWindowType.PiecesCheatSheetWindow);
    }
    
    private List<DebugCellView> cells = new List<DebugCellView>();
    
    public void OnToggleCells(bool isChecked)
    {
        var board = BoardService.Current.GetBoardById(0);

        if (isChecked)
        {
            foreach (var cell in cells)
            {
                board.RendererContext.DestroyElement(cell);
            }
            
            cells = new List<DebugCellView>();
            return;
        }
        
        for (var i = 0; i < board.BoardDef.Width; i++)
        {
            for (var j = 0; j < board.BoardDef.Height; j++)
            {
                if(board.BoardLogic.IsLockedCell(new BoardPosition(i, j, 1))) continue;
                
                var cell = board.RendererContext.CreateBoardElementAt<DebugCellView>(R.DebugCell, new BoardPosition(i, j, 20));
                cell.SetIndex(i, j);
                cells.Add(cell);
            }
        }
    }

    public void OnCompleteFirstQuestClick()
    {
        var manager = GameDataService.Current.QuestsManager;
        if (manager.ActiveQuests.Count == 0)
        {
            return;
        }

        var quest = manager.ActiveQuests[0];
        quest.ForceComplete();
    }

    public void OnDebug1Click()
    {
        Debug.Log("OnDebug1Click");
        //
        // var skinDef = new Dictionary<string, string>
        // {
        //     {"cell_tile_full_0", "cell_tile_full_0"},
        //     {"cell_tile_full_1", "cell_tile_full_1"},
        //     {"cell_tile_00", "cell_tile_00"},
        //     {"cell_tile_01", "cell_tile_01"},
        //     {"cell_tile_02", "cell_tile_02"},
        //     {"cell_tile_03", "cell_tile_03"},
        //     {"cell_tile_10", "cell_tile_10"},
        //     {"cell_tile_20", "cell_tile_20"},
        //     {"cell_tile_30", "cell_tile_30"},
        //     {"cell_tile_40", "cell_tile_40"},
        //     {"cell_tile_11", "cell_tile_11"},
        //     {"cell_tile_14", "cell_tile_14"},
        //     {"cell_tile_13", "cell_tile_13"},
        //     {"cell_tile_12", "cell_tile_12"},
        // };
        //
        // // int[,] fieldDef = new[,]
        // // {
        // //     {1,1,0,0},
        // //     {1,1,0,0},
        // //     {0,0,1,1},
        // //     {1,1,1,1},
        // //     {1,0,0,0},
        // // };
        //
        // // int[,] fieldDef = new[,]
        // // {
        // //     {0,0,1,1,0,0},
        // //     {0,1,1,1,1,0},
        // //     {1,1,0,0,1,1},
        // //     {1,1,0,0,1,1},
        // //     {0,1,1,1,1,0},
        // //     {0,0,1,1,0,0},
        // // };
        //
        // // int[,] fieldDef = new[,]
        // // {
        // //     {1,1,0,0,1,1},
        // //     {1,1,0,0,1,1},
        // //     {0,0,1,1,0,0},
        // //     {0,0,1,1,0,0},
        // //     {1,1,0,0,1,1},
        // //     {1,1,0,0,1,1},
        // // };
        //
        // var fieldDef = GameDataService.Current.FogsManager.GetFoggedArea();
        //
        // //var material = new Material(Shader.Find("Sprites/Opaque"));
        // Material fillMaterial = new Material(Shader.Find("Sprites/Default"))
        // {
        //     renderQueue = 999
        // };
        //
        // Material borderMaterial = new Material(Shader.Find("Sprites/Default"))
        // {
        //     renderQueue = 1000
        // };
        //
        // CustomMeshBuilder.LayoutDef def = new CustomMeshBuilder.LayoutDef
        // {
        //     SkinDef = skinDef,
        //     BorderWidth = 0.2f,
        //     Matrix = fieldDef,
        //     Parent = GameObject.Find("PARAMPAMPAM").transform,
        //     FillMaterial = fillMaterial,
        //     BorderMaterial = borderMaterial,
        //     Fill = true
        // };
        //
        // new CustomMeshBuilder().Build(def);

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
            LineWidth = 0.3f
        };
        
        var mesh = meshBuilder.Build(def);

        var meshGo = new GameObject("Grid");
        var meshTransform = meshGo.transform;
        meshTransform.SetParent(GameObject.Find("PARAMPAMPAM").transform);
        meshTransform.localPosition = Vector3.zero;

        var meshRenderer = meshGo.AddComponent<MeshRenderer>();
        var meshFilter   = meshGo.AddComponent<MeshFilter>();

        meshFilter.mesh = mesh;

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            renderQueue = 1000
        };

        meshRenderer.material = mat;
        meshRenderer.material.mainTexture = lineSprite.texture;
    }

    public void OnDebug2Click()
    {
        Debug.Log("OnDebug2Click");
        
        //BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.CreatePiece, PieceType.A1.Id);
        
#if LEAKWATCHER
        GC.Collect();
        GC.WaitForPendingFinalizers();
    
        Debug.Log(LeakWatcher.Instance.DataAsString(false));
#endif

        // QuestService.Current.Load();
        // BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, null);

        // string text = File.ReadAllText(@"D:/save.json");
        // QuestSaveComponent q = JsonConvert.DeserializeObject<QuestSaveComponent>(text);
        //
        // string i = "";
    }
    
    #if DEBUG
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnDebug1Click();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnDebug2Click();
            }
        }
    }
#endif
}