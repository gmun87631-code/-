using SideScrollerPrototype.Config;
using SideScrollerPrototype.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SideScrollerPrototype.Editor
{
    public static class PrototypeSetupMenu
    {
        [MenuItem("Tools/Side Scroller Prototype/Create Sample Prototype")]
        public static void CreateSamplePrototype()
        {
            EnsureFolder("Assets/Data");
            EnsureFolder("Assets/Data/Characters");
            EnsureFolder("Assets/Data/Enemies");
            EnsureFolder("Assets/Data/Levels");
            EnsureFolder("Assets/Scenes");

            CharacterDefinition explorer = CreateCharacter(
                "Assets/Data/Characters/SkyExplorer.asset",
                "sky_explorer",
                "Sky Explorer",
                new Color(0.18f, 0.72f, 0.96f),
                7.8f,
                13.8f,
                60f,
                72f,
                0.45f,
                3);

            EnemyBehaviorDefinition emberCrawler = CreateEnemy(
                "Assets/Data/Enemies/EmberCrawler.asset",
                "ember_crawler",
                "Ember Crawler",
                new Color(0.88f, 0.33f, 0.18f),
                2.4f,
                2.5f,
                8f,
                10,
                15);

            LevelDefinition level = CreateLevel("Assets/Data/Levels/StarfallCrossing.asset", emberCrawler);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject bootstrapObject = new GameObject("PrototypeBootstrap");
            RuntimeGameBootstrap bootstrap = bootstrapObject.AddComponent<RuntimeGameBootstrap>();
            bootstrap.playerCharacter = explorer;
            bootstrap.startingLevel = level;

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/PrototypeScene.unity");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Prototype Created",
                "Sample assets and the playable scene were created at Assets/Scenes/PrototypeScene.unity",
                "OK");
        }

        private static CharacterDefinition CreateCharacter(
            string assetPath,
            string characterId,
            string displayName,
            Color tint,
            float moveSpeed,
            float jumpForce,
            float acceleration,
            float deceleration,
            float jumpCutMultiplier,
            int maxHealth)
        {
            CharacterDefinition definition = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<CharacterDefinition>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            definition.characterId = characterId;
            definition.displayName = displayName;
            definition.tint = tint;
            definition.moveSpeed = moveSpeed;
            definition.acceleration = acceleration;
            definition.deceleration = deceleration;
            definition.jumpForce = jumpForce;
            definition.jumpCutMultiplier = jumpCutMultiplier;
            definition.maxHealth = maxHealth;
            definition.colliderSize = new Vector2(0.9f, 1.35f);

            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static EnemyBehaviorDefinition CreateEnemy(
            string assetPath,
            string enemyId,
            string displayName,
            Color tint,
            float moveSpeed,
            float patrolDistance,
            float contactKnockback,
            int touchPenalty,
            int stompBonus)
        {
            EnemyBehaviorDefinition definition = AssetDatabase.LoadAssetAtPath<EnemyBehaviorDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<EnemyBehaviorDefinition>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            definition.enemyId = enemyId;
            definition.displayName = displayName;
            definition.tint = tint;
            definition.moveSpeed = moveSpeed;
            definition.patrolDistance = patrolDistance;
            definition.contactKnockback = contactKnockback;
            definition.touchPenalty = touchPenalty;
            definition.stompBonus = stompBonus;

            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static LevelDefinition CreateLevel(string assetPath, EnemyBehaviorDefinition enemyDefinition)
        {
            LevelDefinition definition = AssetDatabase.LoadAssetAtPath<LevelDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<LevelDefinition>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            definition.playerSpawn = new Vector2(-9f, -1f);
            definition.goalPosition = new Vector2(12.8f, -1.3f);
            definition.fallLimitY = -8f;
            definition.platforms = new System.Collections.Generic.List<PlatformSegment>
            {
                new PlatformSegment { center = new Vector2(-9f, -2.85f), size = new Vector2(5.2f, 1f), isHazard = false },
                new PlatformSegment { center = new Vector2(-3.4f, -2.85f), size = new Vector2(3.3f, 1f), isHazard = false },
                new PlatformSegment { center = new Vector2(1.1f, -2.85f), size = new Vector2(2.4f, 1f), isHazard = false },
                new PlatformSegment { center = new Vector2(4.7f, -2.85f), size = new Vector2(2.8f, 1f), isHazard = true },
                new PlatformSegment { center = new Vector2(8.9f, -2.85f), size = new Vector2(3.6f, 1f), isHazard = false },
                new PlatformSegment { center = new Vector2(12.4f, -2.85f), size = new Vector2(2.5f, 1f), isHazard = false },
                new PlatformSegment { center = new Vector2(-5.3f, -0.9f), size = new Vector2(2.1f, 0.6f), isHazard = false },
                new PlatformSegment { center = new Vector2(-1.7f, 0.25f), size = new Vector2(1.9f, 0.6f), isHazard = false },
                new PlatformSegment { center = new Vector2(2.2f, 1.2f), size = new Vector2(2.2f, 0.6f), isHazard = false },
                new PlatformSegment { center = new Vector2(6.3f, 0.45f), size = new Vector2(2.4f, 0.6f), isHazard = false },
                new PlatformSegment { center = new Vector2(9.8f, 1.55f), size = new Vector2(2f, 0.6f), isHazard = false },
                new PlatformSegment { center = new Vector2(1.8f, 3.2f), size = new Vector2(1.8f, 0.5f), isHazard = false },
                new PlatformSegment { center = new Vector2(4.7f, 3.9f), size = new Vector2(1.8f, 0.5f), isHazard = false }
            };

            definition.collectibles = new System.Collections.Generic.List<CollectibleSpawnData>
            {
                new CollectibleSpawnData { position = new Vector2(-7.7f, -1.45f), scoreValue = 5 },
                new CollectibleSpawnData { position = new Vector2(-5.3f, 0.0f), scoreValue = 10 },
                new CollectibleSpawnData { position = new Vector2(-1.7f, 1.1f), scoreValue = 10 },
                new CollectibleSpawnData { position = new Vector2(2.2f, 2.1f), scoreValue = 15 },
                new CollectibleSpawnData { position = new Vector2(4.7f, 4.75f), scoreValue = 20 },
                new CollectibleSpawnData { position = new Vector2(6.3f, 1.3f), scoreValue = 10 },
                new CollectibleSpawnData { position = new Vector2(9.8f, 2.4f), scoreValue = 15 },
                new CollectibleSpawnData { position = new Vector2(12.2f, -1.3f), scoreValue = 5 }
            };

            definition.enemies = new System.Collections.Generic.List<EnemySpawnData>
            {
                new EnemySpawnData { position = new Vector2(-2.8f, -1.95f), behavior = enemyDefinition },
                new EnemySpawnData { position = new Vector2(1.2f, -1.95f), behavior = enemyDefinition },
                new EnemySpawnData { position = new Vector2(8.9f, -1.95f), behavior = enemyDefinition }
            };

            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            int splitIndex = folderPath.LastIndexOf('/');
            string parent = folderPath.Substring(0, splitIndex);
            string child = folderPath.Substring(splitIndex + 1);
            AssetDatabase.CreateFolder(parent, child);
        }
    }
}
