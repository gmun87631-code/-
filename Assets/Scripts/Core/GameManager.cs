using System.Text;
using SideScrollerPrototype.Config;
using SideScrollerPrototype.Player;
using SideScrollerPrototype.UI;
using UnityEngine;

namespace SideScrollerPrototype.Core
{
    public class GameManager : MonoBehaviour
    {
        private CharacterDefinition playerCharacter;
        private LevelDefinition levelDefinition;
        private LevelBuilder levelBuilder;
        private GameUIController uiController;
        private CameraFollow2D cameraFollow;
        private PlayerController activePlayer;

        private int currentScore;
        private int currentHealth;
        private int remainingCollectibles;
        private bool hasGameStarted;
        private bool isPlaying;
        private bool isInvulnerable;
        private float invulnerabilityTimer;

        public void Configure(
            CharacterDefinition character,
            LevelDefinition level,
            LevelBuilder builder,
            GameUIController ui,
            CameraFollow2D cameraController)
        {
            playerCharacter = character;
            levelDefinition = level;
            levelBuilder = builder;
            uiController = ui;
            cameraFollow = cameraController;
            currentHealth = playerCharacter == null ? 0 : Mathf.Max(1, playerCharacter.maxHealth);
            RefreshUi();
        }

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            if (isInvulnerable)
            {
                invulnerabilityTimer -= Time.deltaTime;
                if (invulnerabilityTimer <= 0f)
                {
                    isInvulnerable = false;
                }
            }
        }

        public void StartGame()
        {
            if (playerCharacter == null || levelDefinition == null)
            {
                Debug.LogWarning("GameManager needs a player character and a level definition.");
                return;
            }

            hasGameStarted = true;
            currentScore = 0;
            currentHealth = Mathf.Max(1, playerCharacter.maxHealth);
            StartLevel();
        }

        public void RestartGame()
        {
            if (playerCharacter == null || levelDefinition == null)
            {
                return;
            }

            hasGameStarted = true;
            currentScore = 0;
            currentHealth = Mathf.Max(1, playerCharacter.maxHealth);
            StartLevel();
        }

        public void CollectItem(int scoreValue, GameObject collectibleObject)
        {
            AddScore(scoreValue);
            remainingCollectibles = Mathf.Max(0, remainingCollectibles - 1);
            Destroy(collectibleObject);
            RefreshUi();
        }

        public void AddScore(int amount)
        {
            currentScore = Mathf.Max(0, currentScore + amount);
            RefreshUi();
        }

        public void DamagePlayer(int amount, Vector2 knockback)
        {
            if (!isPlaying || activePlayer == null || isInvulnerable)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);
            activePlayer.ApplyKnockback(knockback);

            if (currentHealth <= 0)
            {
                LoseGame("The explorer ran out of health.");
                return;
            }

            isInvulnerable = true;
            invulnerabilityTimer = 1.1f;
            RefreshUi();
        }

        public void OnPlayerFellOutOfBounds()
        {
            if (!isPlaying)
            {
                return;
            }

            LoseGame("You fell out of the level.");
        }

        public void OnGoalReached()
        {
            if (!isPlaying)
            {
                return;
            }

            isPlaying = false;
            CleanupActivePlayer();
            RefreshUi("Stage Clear", BuildWinText());
        }

        public void SetRemainingCollectibles(int count)
        {
            remainingCollectibles = Mathf.Max(0, count);
            RefreshUi();
        }

        private void StartLevel()
        {
            isPlaying = true;
            isInvulnerable = false;
            invulnerabilityTimer = 0f;
            CleanupActivePlayer();
            levelBuilder.Build(levelDefinition, this);
            SpawnPlayer(playerCharacter, levelDefinition.playerSpawn);
            RefreshUi();
        }

        private void LoseGame(string message)
        {
            isPlaying = false;
            CleanupActivePlayer();
            RefreshUi("Try Again", message + "\n\nPress Restart to run the level again.");
        }

        private void SpawnPlayer(CharacterDefinition characterDefinition, Vector2 spawnPosition)
        {
            GameObject playerObject = new GameObject(characterDefinition.displayName);
            playerObject.transform.position = spawnPosition;

            activePlayer = playerObject.AddComponent<PlayerController>();
            activePlayer.Initialize(characterDefinition, this);

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(playerObject.transform);
            }
        }

        private void CleanupActivePlayer()
        {
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(null);
            }

            if (activePlayer != null)
            {
                Destroy(activePlayer.gameObject);
                activePlayer = null;
            }
        }

        private void RefreshUi(string titleOverride = null, string infoOverride = null)
        {
            if (uiController == null)
            {
                return;
            }

            GameUiSnapshot snapshot = new GameUiSnapshot();
            snapshot.showMenu = !isPlaying;
            snapshot.showRestartButton = hasGameStarted;
            snapshot.showStartButton = !hasGameStarted;
            snapshot.hudText = BuildHudText();

            if (titleOverride != null || infoOverride != null)
            {
                snapshot.titleText = titleOverride ?? string.Empty;
                snapshot.infoText = infoOverride ?? string.Empty;
            }
            else if (!hasGameStarted)
            {
                snapshot.titleText = "Starfall Crossing";
                snapshot.infoText = "Run, jump, collect crystals, and reach the sun gate.\nHold jump for higher leaps. Release early for shorter hops.";
            }
            else
            {
                snapshot.titleText = string.Empty;
                snapshot.infoText = string.Empty;
            }

            uiController.Refresh(snapshot);
        }

        private string BuildHudText()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Explorer: ");
            builder.Append(playerCharacter == null ? "None" : playerCharacter.displayName);
            builder.AppendLine();
            builder.Append("Score: ");
            builder.Append(currentScore);
            builder.AppendLine();
            builder.Append("Health: ");
            builder.Append(currentHealth);
            builder.Append(" / ");
            builder.Append(playerCharacter == null ? 0 : playerCharacter.maxHealth);
            builder.AppendLine();
            builder.Append("Crystals Left: ");
            builder.Append(remainingCollectibles);
            builder.AppendLine();
            builder.Append("Goal: Reach the sun gate");
            return builder.ToString();
        }

        private string BuildWinText()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("You reached the sun gate.\n");
            builder.Append("Final Score: ");
            builder.Append(currentScore);
            builder.Append("\n");
            builder.Append("Health Remaining: ");
            builder.Append(currentHealth);
            builder.Append("\n\nPress Restart to play again.");
            return builder.ToString();
        }

        public bool IsAcceptingInput
        {
            get { return isPlaying; }
        }

        public float FallLimitY
        {
            get { return levelDefinition == null ? -8f : levelDefinition.fallLimitY; }
        }
    }
}
