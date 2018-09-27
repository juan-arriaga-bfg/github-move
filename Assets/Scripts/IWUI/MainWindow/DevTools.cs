﻿using System;
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
        QuestService.Current.DisconnectFromBoard();
        QuestService.Instance.SetManager(null);
            
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

    public void OnDebug1Click()
    {
        Debug.Log("OnDebug1Click");
        
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, null);
        // BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.CreatePiece, PieceType.A1.Id);
        // BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.CreatePiece, PieceType.A1.Id);
        
        // QuestService.Current.Serialize();
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

        string text = File.ReadAllText(@"D:/save.json");
        QuestSaveComponent q = JsonConvert.DeserializeObject<QuestSaveComponent>(text);

        string i = "";
    }
}