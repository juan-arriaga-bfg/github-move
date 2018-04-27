using UnityEngine;

public class ChestPieceView : PieceBoardElementView
{
    [SerializeField] private Transform cap;
    
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject timerGo;
    
    [SerializeField] private NSText timerLabel;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private ChestPieceComponent chestComponent;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
        UpdateView();
        chestComponent.Timer.OnComplete += UpdateView;
        chestComponent.Timer.OnExecute += UpdateProgress;
    }

    public override void UpdateView()
    {
        if(chestComponent == null || chestComponent.Chest == null) return;
        
        var isOpen = chestComponent.Chest.State == ChestState.Open;
        
        shine.SetActive(isOpen);
        timerGo.SetActive(chestComponent.Chest.State == ChestState.InProgress);
        
        cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? 0.66f : 0.53f);
        
        if(isOpen == false) return;
        
        chestComponent.Timer.OnComplete -= UpdateView;
        chestComponent.Timer.OnExecute -= UpdateProgress;
        chestComponent.Timer.Stop();
        
        if(GameDataService.Current.ChestsManager.ActiveChest == chestComponent.Chest)
        {
            GameDataService.Current.ChestsManager.ActiveChest = null;
        }
    }

    private void UpdateProgress()
    {
        timerLabel.Text = string.Format("<mspace=3em>{0}</mspace>", chestComponent.Timer.GetTimeLeftText(null));
        if(progressBar != null) progressBar.SetProgress((float)chestComponent.Timer.GetTime().TotalSeconds / chestComponent.Timer.Delay);
    }
}