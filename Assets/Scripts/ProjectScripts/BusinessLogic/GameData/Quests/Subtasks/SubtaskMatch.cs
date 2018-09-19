namespace Quests
{
    public class SubtaskMatch: SubtaskCounter, IBoardEventListener
    {
        public override void Init()
        {
            BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
        }

        public void OnBoardEvent(int code, object context)
        {
            if (code == GameEventsCodes.Match)
            {
                currentValue += 1;
                OnChanged(this);
            }
        }
    }
}