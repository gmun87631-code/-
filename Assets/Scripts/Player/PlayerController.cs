using SideScrollerPrototype.Config;
using SideScrollerPrototype.Collectibles;
using SideScrollerPrototype.Core;
using SideScrollerPrototype.Utils;
using UnityEngine;

namespace SideScrollerPrototype.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        private const float GroundCheckInset = 0.05f;
        private const float GroundCheckDistance = 0.08f;

        private CharacterDefinition definition;
        private GameManager gameManager;
        private Rigidbody2D body;
        private BoxCollider2D boxCollider;
        private float horizontalInput;
        private bool jumpPressed;
        private bool jumpReleased;

        public void Initialize(CharacterDefinition characterDefinition, GameManager manager)
        {
            definition = characterDefinition;
            gameManager = manager;

            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            body.gravityScale = 3.5f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            boxCollider.size = Vector2.one;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = definition.sprite != null
                ? definition.sprite
                : PlaceholderSpriteFactory.GetSolidSprite(definition.tint);
            spriteRenderer.color = definition.sprite == null ? Color.white : definition.tint;
            spriteRenderer.sortingOrder = 5;
            transform.localScale = new Vector3(definition.colliderSize.x, definition.colliderSize.y, 1f);
        }

        private void Update()
        {
            if (definition == null || gameManager == null || !gameManager.IsAcceptingInput)
            {
                return;
            }

            horizontalInput = Input.GetAxisRaw("Horizontal");
            jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            jumpReleased = Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);

            if (transform.position.y < gameManager.FallLimitY)
            {
                gameManager.OnPlayerFellOutOfBounds();
            }
        }

        private void FixedUpdate()
        {
            if (definition == null || body == null || gameManager == null || !gameManager.IsAcceptingInput)
            {
                return;
            }

            float targetVelocityX = horizontalInput * definition.moveSpeed;
            float accelerationRate = Mathf.Abs(targetVelocityX) > 0.01f ? definition.acceleration : definition.deceleration;
            float movementStep = accelerationRate * Time.fixedDeltaTime;
            float newVelocityX = Mathf.MoveTowards(body.velocity.x, targetVelocityX, movementStep);
            body.velocity = new Vector2(newVelocityX, body.velocity.y);

            bool grounded = IsGrounded();

            if (jumpPressed && grounded)
            {
                body.velocity = new Vector2(body.velocity.x, definition.jumpForce);
            }

            if (jumpReleased && !grounded && body.velocity.y > 0f)
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * definition.jumpCutMultiplier);
            }

            jumpPressed = false;
            jumpReleased = false;
        }

        private bool IsGrounded()
        {
            Bounds bounds = boxCollider.bounds;
            Vector2 boxSize = new Vector2(bounds.size.x - GroundCheckInset, bounds.size.y);
            RaycastHit2D hit = Physics2D.BoxCast(bounds.center, boxSize, 0f, Vector2.down, GroundCheckDistance);
            return hit.collider != null && !hit.collider.isTrigger;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CollectibleItem collectible = other.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                gameManager.CollectItem(collectible.ScoreValue, collectible.gameObject);
                return;
            }

            if (other.GetComponent<HazardZone>() != null)
            {
                gameManager.OnPlayerFellOutOfBounds();
                return;
            }

            if (other.GetComponent<GoalPortal>() != null)
            {
                gameManager.OnGoalReached();
            }
        }

        public void BounceFromEnemy()
        {
            if (body != null)
            {
                body.velocity = new Vector2(body.velocity.x, definition.jumpForce * 0.65f);
            }
        }

        public void ApplyKnockback(Vector2 force)
        {
            if (body != null)
            {
                body.velocity = force;
            }
        }

        public float VerticalVelocity
        {
            get { return body == null ? 0f : body.velocity.y; }
        }

        public float HorizontalInput
        {
            get { return horizontalInput; }
        }
    }
}
