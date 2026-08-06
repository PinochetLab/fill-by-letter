using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boards
{
    public class BoardManager : MonoBehaviour
    {
        private readonly Queue<BoardQueueData> _queue = new ();

        public void Show(AbstractBoard board, object param, bool blackScreen)
        {
            var data = new BoardQueueData(board, param, blackScreen);
            _queue.Enqueue(data);
            if (_queue.Count == 1)
            {
                ShowNext();
            }
        }

        private void ShowNext()
        {
            var data = _queue.Peek();
            var board = data.Board;
            board.Open(data.Param, data.BlackScreen);
        }

        public void Dequeue()
        {
            if (_queue.Count == 0)
            {
                throw new Exception("Board Queue is empty!");
            }
            
            _queue.Dequeue();

            if (_queue.Count > 0)
            {
                ShowNext();
            }
        }
    }
}