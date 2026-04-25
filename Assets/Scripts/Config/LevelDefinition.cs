using System;
using System.Collections.Generic;
using UnityEngine;

namespace SideScrollerPrototype.Config
{
    [CreateAssetMenu(menuName = "Side Scroller Prototype/Level Definition", fileName = "LevelDefinition")]
    public class LevelDefinition : ScriptableObject
    {
        public Vector2 playerSpawn = new Vector2(-7f, -1f);
        public Vector2 goalPosition = new Vector2(11f, -1.2f);
        public float fallLimitY = -8f;
        public List<PlatformSegment> platforms = new List<PlatformSegment>();
        public List<EnemySpawnData> enemies = new List<EnemySpawnData>();
        public List<CollectibleSpawnData> collectibles = new List<CollectibleSpawnData>();
    }

    [Serializable]
    public struct PlatformSegment
    {
        public Vector2 center;
        public Vector2 size;
        public bool isHazard;
    }

    [Serializable]
    public struct EnemySpawnData
    {
        public Vector2 position;
        public EnemyBehaviorDefinition behavior;
    }

    [Serializable]
    public struct CollectibleSpawnData
    {
        public Vector2 position;
        public int scoreValue;
    }
}
