using TMPro;
using UnityEngine;

namespace Game.Scripts.New
{
    public class PuzzleGridManager : MonoBehaviour
    {
        public Vector2Int GridSize = new(10, 10);
        public PuzzleBlock[,] Grid { get; private set; }
        public PuzzleBlock[,] LightBlocks { get; private set; }
        public PuzzleBlock EndBlock { get; private set; }
        public PuzzleBlock PlayerBlock { get; private set; }
        public PuzzleBlock CurrentBlock { get; private set; }

        [SerializeField] private TextMeshProUGUI movesCountText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject gameWinPanel;

        private Camera _mainCamera;
        private LevelStateController _stateController;
        private BlockMover _blockMover;
        private CollisionChecker _collisionChecker;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _stateController = new LevelStateController(movesCountText, gameOverPanel, gameWinPanel, this);
            _collisionChecker = new CollisionChecker(this, _stateController);
            _blockMover = new BlockMover(this, _collisionChecker);
        }

        private void Update()
        {
            if (_stateController.IsGameFinished()) return;

            if (CurrentBlock != null)
                _blockMover.MoveWithMouse();
            else
                _blockMover.CheckSelection();
        }

        public void InitializeLevel(PuzzleBlock[,] grid, PuzzleBlock endLevelBlock, PuzzleBlock[,] lightBlocks, PuzzleBlock playerBlock, int movesCount)
        {
            DestroyBlocks(Grid);
            DestroyBlocks(LightBlocks);

            Grid = grid;
            LightBlocks = lightBlocks;
            EndBlock = endLevelBlock;
            PlayerBlock = playerBlock;
            CurrentBlock = null;

            _stateController.Reset(movesCount);
        }

        public void SetCurrentBlock(PuzzleBlock block, Vector2Int gridPos)
        {
            CurrentBlock = block;
            CurrentBlock.CurrentPos = gridPos;
            ClearBlockFromGrid(CurrentBlock);
        }

        public void PlaceCurrentBlock(Vector2Int newPos)
        {
            _blockMover.FillGridWithBlock(CurrentBlock, newPos);
            if (_stateController.TryCompleteLevel(CurrentBlock, EndBlock, newPos)) return;

            _stateController.SpendMove(CurrentBlock, newPos);

            _collisionChecker.CheckLightBlock(CurrentBlock, newPos);
            CurrentBlock.CurrentPos = newPos;
            CurrentBlock = null;
        }

        private void DestroyBlocks(PuzzleBlock[,] blocks)
        {
            if (blocks == null) return;
            foreach (var block in blocks)
                if (block != null)
                    Destroy(block.gameObject);
        }

        private void ClearBlockFromGrid(PuzzleBlock block)
        {
            for (int x = 0; x < block.Size.x; x++)
                for (int y = 0; y < block.Size.y; y++)
                    Grid[block.CurrentPos.x + x, block.CurrentPos.y + y] = null;
        }
    }
}