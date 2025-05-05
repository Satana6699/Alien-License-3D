using Game.Scripts.New;
using Game.Scripts.Tags;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts
{
    public class CollisionChecker
    {
        private readonly PuzzleGridManager _manager;
        private readonly LevelStateController _levelStateController;

        public CollisionChecker(PuzzleGridManager manager, LevelStateController levelStateController)
        {
            _manager = manager;
            _levelStateController = levelStateController;
        }

        public void CheckLightBlock(PuzzleBlock block, Vector2Int pos)
        {
            var light = _manager.LightBlocks[pos.x, pos.y];
            if (light != null && light.Size.Equals(block.Size) && !block.TryGetComponent(out Player _))
            {
                Object.Destroy(light.gameObject);
                Object.Destroy(block.gameObject);
            }
        }
        
        public void CheckWakeUpCollision(PuzzleBlock currentBlock, PuzzleBlock[,] grid, PuzzleBlock playerBlock)
        {
            if (currentBlock.TryGetComponent(out Player _))
            {
                foreach (var block in grid)
                {
                    if (block != null && CheckWakeUpSourceCollision(block, playerBlock))
                        _levelStateController.GameOver(currentBlock);
                    
                    return;
                }
            }

            if (CheckWakeUpSourceCollision(currentBlock, playerBlock))
                _levelStateController.GameOver(currentBlock);
        }

        private bool CheckWakeUpSourceCollision(PuzzleBlock source, PuzzleBlock playerBlock)
        {
            if (source.TryGetComponent(out WakeUpSource wakeUpSource))
            {
                foreach (var point in wakeUpSource.GetPoints())
                {
                    for (int x = 0; x < playerBlock.Size.x; x++)
                    {
                        for (int y = 0; y < playerBlock.Size.y; y++)
                        {
                            if (Mathf.Approximately(playerBlock.transform.position.x + x, point.x) &&
                                Mathf.Approximately(playerBlock.transform.position.z + y, point.z))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }

    }

}