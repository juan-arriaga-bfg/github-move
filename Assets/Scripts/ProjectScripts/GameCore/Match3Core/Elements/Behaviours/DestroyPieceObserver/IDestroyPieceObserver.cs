public interface IDestroyPieceObserverCopy 
{
	void OnDestroyPieceStart(BoardPosition at, BoardPosition current);

	void OnDestroyPieceFinish(BoardPosition at, BoardPosition current);
}
