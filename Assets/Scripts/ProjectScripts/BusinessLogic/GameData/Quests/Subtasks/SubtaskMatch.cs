using System;

namespace Quests
{
    [Serializable]
    public class SubtaskMatch: SubtaskCurrencyCounter, IBoardEventListener
    {
        public override void Init()
        {
            BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
        }

        public void OnBoardEvent(int code, object context)
        {
            if (code == GameEventsCodes.Match)
            {
                CurrentValue += 1;
                OnChanged(this);
            }
        }
    }
}