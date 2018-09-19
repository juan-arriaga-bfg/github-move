namespace Quests
{
    public class SubtaskBuildPiece: SubtaskCounter, IBoardEventListener
    {
        public override void Init()
        {
            BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
        }

        public void OnBoardEvent(int code, object context)
        {
            if (code == GameEventsCodes.CreatePiece)
            {
                currentValue += 1;
                OnChanged(this);
            }
        }
    }
}