using Game.Scripts.New;
using Game.Scripts.Tags;
using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class LevelStateController
    {
        private int _movesCount;
        private readonly TextMeshProUGUI _movesText;
        private readonly PuzzleGridManager _manager;
        private readonly GameObject _gameOverPanel, _gameWinPanel;
        private bool _isGameOver, _isGameWin;

        public LevelStateController(TextMeshProUGUI movesText, GameObject gameOverPanel, GameObject gameWinPanel, PuzzleGridManager puzzleGridManager)
        {
            _movesText = movesText;
            _gameOverPanel = gameOverPanel;
            _gameWinPanel = gameWinPanel;
            _manager = puzzleGridManager;
        }

        public void Reset(int moves)
        {
            _movesCount = moves;
            _movesText.text = moves.ToString();
            _isGameOver = false;
            _isGameWin = false;
        }

        public void SpendMove(PuzzleBlock block, Vector2Int newPos)
        {
            if (block.CurrentPos == newPos) return;

            _movesCount--;
            _movesText.text = _movesCount.ToString();

            if (_movesCount <= 0)
                GameOver(block);
        }

        public bool TryCompleteLevel(PuzzleBlock block, PuzzleBlock endBlock, Vector2Int newPos)
        {
            if (!block.TryGetComponent(out Player _)) return false;
            if (endBlock.CurrentPos != newPos) return false;

            GameWin(block);
            return true;
        }

        public bool IsGameFinished() => _isGameOver || _isGameWin;

        public void GameOver(PuzzleBlock block)
        {
            Object.Destroy(block.gameObject);
            Object.Destroy(_manager.EndBlock.gameObject);
            _gameOverPanel.SetActive(true);
            _isGameOver = true;
        }

        private void GameWin(PuzzleBlock block)
        {
            Object.Destroy(block.gameObject);
            Object.Destroy(_manager.EndBlock.gameObject);
            _gameWinPanel.SetActive(true);
            _isGameWin = true;
        }
    }

}