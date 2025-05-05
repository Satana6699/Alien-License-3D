using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.New;
using Game.Scripts.Tags;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button backLevelButton;
        [SerializeField] private Button[] restartButtons;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private PuzzleGridManager puzzleGrid;
        [SerializeField] private int maxLevels = 3;

        private int _currentLevel = 1;

        private void Awake()
        {
            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(NextLevel);
            
            if (backLevelButton != null)
                backLevelButton.onClick.AddListener(BackLevel);
            
            if (restartButtons != null)
            {
                foreach (var restartButton in restartButtons)
                {
                    restartButton.onClick.AddListener(RestartLevel);
                }
            }

            LoadLevel();
        }

        private void NextLevel()
        {
            if (_currentLevel >= maxLevels)
                return;

            _currentLevel++;
            LoadLevel();
        }

        private void BackLevel()
        {
            if (_currentLevel == 1)
                return;

            _currentLevel--;
            LoadLevel();
        }

        private void LoadLevel()
        {
            if (currentLevelText != null)
                currentLevelText.text = _currentLevel.ToString();

            InitializeLevel();
        }

        private void InitializeLevel()
        {
            var data = JsonLoader.LoadLevel($"Levels/Level{_currentLevel}");
            puzzleGrid.GridSize = new Vector2Int(data.size.x, data.size.y);

            PuzzleBlock[,] puzzleBlocks = new PuzzleBlock[data.size.x, data.size.y];

            PuzzleBlock endLevelBlock = null;
            PuzzleBlock playerBlock = null;

            InitPuzzleBlock(data, puzzleBlocks, ref endLevelBlock, ref playerBlock);
            
            PuzzleBlock[,] lightBlocks = new PuzzleBlock[data.size.x, data.size.y];
            InitLightBlocks(data, lightBlocks);
            
            puzzleGrid.InitializeLevel(
                grid: puzzleBlocks, 
                endLevelBlock: endLevelBlock, 
                lightBlocks: lightBlocks, 
                playerBlock: playerBlock, 
                movesCount: data.movesCount);
            
            AdjustGroundToGrid(data);
        }

        private void InitLightBlocks(LevelData data, PuzzleBlock[,] lightBlocks)
        {
            foreach (var block in data.lightBlocks)
            {
                var prefab = PlaceBlock(lightBlocks, block.positions, $"Furnitures/{block.blockType}");
            }
        }

        private void InitPuzzleBlock(LevelData data, PuzzleBlock[,] puzzleBlocks, ref PuzzleBlock endLevelBlock,
            ref PuzzleBlock playerBlock)
        {
            foreach (var block in data.puzzleBlocks)
            {
                var prefab = PlaceBlock(puzzleBlocks, block.positions, $"Furnitures/{block.blockType}");

                if (prefab.TryGetComponent(out Exit exit))
                {
                    endLevelBlock = exit.GetComponent<PuzzleBlock>();
                }
                
                if (prefab.TryGetComponent(out Player player))
                {
                    playerBlock = player.GetComponent<PuzzleBlock>();
                }
            }
        }

        private GameObject PlaceBlock(PuzzleBlock[,] puzzleBlocks, List<Vector2Int> positions, string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            if (prefab == null) return null;

            var instantiate = Instantiate(prefab);

            if (instantiate.TryGetComponent(out PuzzleBlock puzzleBlock))
            {
                Vector2Int basePos = positions[0];
                puzzleBlock.CurrentPos = basePos;
                puzzleBlock.transform.position = new Vector3(basePos.x, 0, basePos.y);

                if (!instantiate.TryGetComponent(out Exit exit))
                {
                    foreach (var pos in positions)
                    {
                        puzzleBlocks[pos.x, pos.y] = puzzleBlock;
                    }
                }
            }

            return instantiate;
        }

        private void AdjustGroundToGrid(LevelData data)
        {
            // GridSize напрямую отражает размер сетки в юнитах
            puzzleGrid.transform.localScale = new Vector3(data.size.x, 0.1f, data.size.y);

            // Смещение, чтобы левый нижний угол имел позицию 0,0
            puzzleGrid.transform.position = new Vector3(data.size.x / 2f - 0.5f, 0f, data.size.y / 2f - 0.5f);
        }

        private void RestartLevel()
        {
            InitializeLevel();
        }
    }
}