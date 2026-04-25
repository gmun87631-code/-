using SideScrollerPrototype.Config;
using SideScrollerPrototype.Core;
using SideScrollerPrototype.Player;
using SideScrollerPrototype.Utils;
using UnityEngine;

namespace SideScrollerPrototype.Enemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyController : MonoBehaviour
    {
        private EnemyBehaviorDefinition behavior;
        private GameManager gameManager;
        private Rigidbody2D body;
        private Vector3 startPosition;
        private int moveDirection = 1;

        public void Initialize(EnemyBehaviorDefinition definition, GameManager manager)
        {
            behavior = definition;
            gameManager = manager;
            startPosition = transform.position;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = behavior.sprite != null
                ? behavior.sprite
                : PlaceholderSpriteFactory.GetSolidSprite(behavior.tint);
            spriteRenderer.color = behavior.sprite == null ? Color.white : behavior.tint;
            spriteRenderer.sortingOrder = 3;
            transform.localScale = new Vector3(behavior.colliderSize.x, behavior.colliderSize.y, 1f);

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = Vector2.one;

            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }

        private void Update()
        {
            if (behavior == null)
            {
                return;
            }

            float movement = behavior.moveSpeed * moveDirection;
            body.velocity = new Vector2(movement, 0f);

            float traveled = transform.position.x - startPosition.x;
            if (Mathf.Abs(traveled) >= behavior.patrolDistance)
            {
                moveDirection *= -1;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null || behavior == null)
            {
                return;
            }

            if (player.transform.position.y > transform.position.y + 0.35f && player.VerticalVelocity < 0f)
            {
                gameManager.AddScore(behavior.stompBonus);
                player.BounceFromEnemy();
                Destroy(gameObject);
                return;
            }

            gameManager.AddScore(-behavior.touchPenalty);
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            if (Mathf.Approximately(direction, 0f))
            {
                direction = 1f;
            }

            Vector2 knockback = new Vector2(direction * behavior.contactKnockback, behavior.contactKnockback * 0.6f);
            gameManager.DamagePlayer(1, knockback);
        }
    }
}
