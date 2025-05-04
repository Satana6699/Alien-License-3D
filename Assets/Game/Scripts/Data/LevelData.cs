using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Data
{
    [System.Serializable]
    public class LevelData
    {
        public Vector2Int size;
        public int movesCount;
        public List<Block> puzzleBlocks;
        public List<Block> lightBlocks;
    }
}
