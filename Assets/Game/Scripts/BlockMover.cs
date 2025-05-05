using Game.Scripts.New;
using UnityEngine;

namespace Game.Scripts
{
    public class BlockMover
    {
        private readonly PuzzleGridManager _manager;
        private readonly Camera _camera;
        private readonly CollisionChecker _collisionChecker;

        public BlockMover(PuzzleGridManager manager, CollisionChecker collisionChecker)
        {
            _manager = manager;
            _camera = Camera.main;
            _collisionChecker = collisionChecker;
        }

        public void MoveWithMouse()
        {
            var block = _manager.CurrentBlock;
            if (!block) return;

            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!groundPlane.Raycast(ray, out float dist)) return;

            Vector3 worldPos = ray.GetPoint(dist);
            Vector2Int newPos = GetTargetPosition(block, worldPos);

            if (!IsValidPlacement(block, newPos))
            {
                block.transform.position = new Vector3(newPos.x, 0, newPos.y);
                if (Input.GetMouseButtonDown(0))
                    _manager.PlaceCurrentBlock(newPos);
            }
            
            _collisionChecker.CheckWakeUpCollision(block, _manager.Grid, _manager.PlayerBlock);
        }

        private Vector2Int GetTargetPosition(PuzzleBlock block, Vector3 worldPos)
        {
            float offsetX = Mathf.Abs(block.CurrentPos.x - worldPos.x);
            float offsetY = Mathf.Abs(block.CurrentPos.y - worldPos.z);

            int x = offsetX > offsetY ? Mathf.RoundToInt(worldPos.x) : block.CurrentPos.x;
            int y = offsetX > offsetY ? block.CurrentPos.y : Mathf.RoundToInt(worldPos.z);

            return new Vector2Int(x, y);
        }

        private bool IsValidPlacement(PuzzleBlock block, Vector2Int pos)
        {
            Vector2Int currentPos = block.CurrentPos;

            if (pos.x != currentPos.x && CheckMovementAxis(
                    block: block,
                    isXAxis: true,
                    current: currentPos.x,
                    target: pos.x,
                    fixedAxisPos: currentPos.y,
                    targetFixedAxis: pos.y))
            {
                return true;
            }

            if (pos.y != currentPos.y && CheckMovementAxis(
                    block: block,
                    isXAxis: false,
                    current: currentPos.y,
                    target: pos.y,
                    fixedAxisPos: currentPos.x,
                    targetFixedAxis: pos.x))
            {
                return true;
            }

            return false;
        }

        private bool CheckMovementAxis(PuzzleBlock block, bool isXAxis, int current, int target, int fixedAxisPos, int targetFixedAxis)
        {
            int direction = target > current ? 1 : -1;
            int steps = Mathf.Abs(target - current);

            for (int step = 1; step <= steps; step++)
            {
                int movingAxisPos = current + direction * step;

                for (int x = 0; x < block.Size.x; x++)
                {
                    for (int y = 0; y < block.Size.y; y++)
                    {
                        // Вычисляем координаты в зависимости от оси движения
                        int checkX = isXAxis ? movingAxisPos + x : targetFixedAxis + x;
                        int checkY = isXAxis ? fixedAxisPos + y : movingAxisPos + y;

                        if (checkX < 0 || checkX >= _manager.GridSize.x || checkY < 0 || checkY >= _manager.GridSize.y)
                            return true;

                        if (_manager.Grid[checkX, checkY] != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        public void CheckSelection()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var block = hit.collider.GetComponent<PuzzleBlock>();
                if (block == null) return;

                Vector3 pos = block.transform.position;
                int x = Mathf.RoundToInt(pos.x);
                int y = Mathf.RoundToInt(pos.z);

                if (_manager.Grid[x, y] == block)
                {
                    _manager.SetCurrentBlock(block, new Vector2Int(x, y));
                }
            }
        }

        public void FillGridWithBlock(PuzzleBlock block, Vector2Int pos)
        {
            for (int dx = 0; dx < block.Size.x; dx++)
                for (int dy = 0; dy < block.Size.y; dy++)
                    _manager.Grid[pos.x + dx, pos.y + dy] = block;
        }
    }

}