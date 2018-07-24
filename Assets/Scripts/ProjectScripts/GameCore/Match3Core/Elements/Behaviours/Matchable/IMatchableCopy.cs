public interface IMatchableCopy
{
	bool IsMatchable(BoardPosition at);

	bool OnMatchStart(BoardPosition at);

	bool OnMatchFinish(BoardPosition at);
}