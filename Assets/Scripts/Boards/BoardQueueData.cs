namespace Boards
{
    public class BoardQueueData
    {
        public BoardQueueData(AbstractBoard board, object param, bool blackScreen)
        {
            Board = board;
            Param = param;
            BlackScreen = blackScreen;
        }
        
        public AbstractBoard Board { get; }
        public object Param { get; }
        public bool BlackScreen { get; }
    }
}