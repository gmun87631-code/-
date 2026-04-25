using SideScrollerPrototype.Config;
using SideScrollerPrototype.UI;
using UnityEngine;

namespace SideScrollerPrototype.Core
{
    public class RuntimeGameBootstrap : MonoBehaviour
    {
        [Header("Game Setup")]
        public CharacterDefinition playerCharacter;
        public LevelDefinition startingLevel;

        private void Awake()
        {
            SetupCamera();

            LevelBuilder levelBuilder = GetComponent<LevelBuilder>();
            if (levelBuilder == null)
            {
                levelBuilder = gameObject.AddComponent<LevelBuilder>();
            }

            GameManager gameManager = GetComponent<GameManager>();
            if (gameManager == null)
            {
                gameManager = gameObject.AddComponent<GameManager>();
            }

            GameUIController uiController = FindObjectOfType<GameUIController>();
            if (uiController == null)
            {
                GameObject uiObject = new GameObject("GameUI");
                uiController = uiObject.AddComponent<GameUIController>();
            }

            uiController.Initialize(gameManager);

            CameraFollow2D cameraFollow = Camera.main.GetComponent<CameraFollow2D>();
            if (cameraFollow == null)
            {
                cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow2D>();
            }

            gameManager.Configure(
                playerCharacter,
                startingLevel,
                levelBuilder,
                uiController,
                cameraFollow);
        }

        private void SetupCamera()
        {
            if (Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = 4.5f;
                Camera.main.backgroundColor = new Color(0.65f, 0.86f, 0.98f);
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraComponent.orthographic = true;
            cameraComponent.orthographicSize = 4.5f;
            cameraComponent.backgroundColor = new Color(0.65f, 0.86f, 0.98f);
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }
    }
}
