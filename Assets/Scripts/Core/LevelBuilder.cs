using SideScrollerPrototype.Collectibles;
using SideScrollerPrototype.Config;
using SideScrollerPrototype.Enemy;
using SideScrollerPrototype.Utils;
using UnityEngine;

namespace SideScrollerPrototype.Core
{
    public class LevelBuilder : MonoBehaviour
    {
        private Transform worldRoot;
        private int collectibleCount;

        public void Build(LevelDefinition levelDefinition, GameManager gameManager)
        {
            ClearWorld();

            worldRoot = new GameObject("GeneratedLevel").transform;
            worldRoot.SetParent(transform, false);
            collectibleCount = 0;

            for (int i = 0; i < levelDefinition.platforms.Count; i++)
            {
                CreatePlatform(levelDefinition.platforms[i]);
            }

            for (int i = 0; i < levelDefinition.collectibles.Count; i++)
            {
                CreateCollectible(levelDefinition.collectibles[i]);
                collectibleCount++;
            }

            for (int i = 0; i < levelDefinition.enemies.Count; i++)
            {
                CreateEnemy(levelDefinition.enemies[i], gameManager);
            }

            CreateGoal(levelDefinition.goalPosition);
            gameManager.SetRemainingCollectibles(collectibleCount);
        }

        private void ClearWorld()
        {
            if (worldRoot != null)
            {
                Destroy(worldRoot.gameObject);
                worldRoot = null;
            }
        }

        private void CreatePlatform(PlatformSegment platform)
        {
            GameObject platformObject = new GameObject(platform.isHazard ? "Hazard" : "Platform");
            platformObject.transform.SetParent(worldRoot, false);
            platformObject.transform.position = platform.center;

            SpriteRenderer renderer = platformObject.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteFactory.GetSolidSprite(platform.isHazard
                ? new Color(0.85f, 0.25f, 0.2f)
                : new Color(0.22f, 0.32f, 0.48f));
            renderer.sortingOrder = 1;
            platformObject.transform.localScale = new Vector3(platform.size.x, platform.size.y, 1f);

            BoxCollider2D collider = platformObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = platform.isHazard;

            if (platform.isHazard)
            {
                platformObject.AddComponent<HazardZone>();
            }
        }

        private void CreateCollectible(CollectibleSpawnData collectibleData)
        {
            GameObject collectibleObject = new GameObject("Crystal");
            collectibleObject.transform.SetParent(worldRoot, false);
            collectibleObject.transform.position = collectibleData.position;

            CollectibleItem collectible = collectibleObject.AddComponent<CollectibleItem>();
            collectible.Initialize(collectibleData.scoreValue);
        }

        private void CreateEnemy(EnemySpawnData enemyData, GameManager gameManager)
        {
            if (enemyData.behavior == null)
            {
                return;
            }

            GameObject enemyObject = new GameObject(enemyData.behavior.displayName);
            enemyObject.transform.SetParent(worldRoot, false);
            enemyObject.transform.position = enemyData.position;

            EnemyController enemy = enemyObject.AddComponent<EnemyController>();
            enemy.Initialize(enemyData.behavior, gameManager);
        }

        private void CreateGoal(Vector2 goalPosition)
        {
            GameObject goalObject = new GameObject("GoalPortal");
            goalObject.transform.SetParent(worldRoot, false);
            goalObject.transform.position = goalPosition;

            GoalPortal goalPortal = goalObject.AddComponent<GoalPortal>();
            goalPortal.Initialize();
        }
    }
}
