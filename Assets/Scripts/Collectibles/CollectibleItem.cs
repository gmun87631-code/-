using SideScrollerPrototype.Utils;
using UnityEngine;

namespace SideScrollerPrototype.Collectibles
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private int scoreValue = 5;

        public void Initialize(int newScoreValue)
        {
            scoreValue = newScoreValue;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = PlaceholderSpriteFactory.GetCircleSprite(new Color(0.25f, 0.95f, 0.95f));
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 4;
            transform.localScale = new Vector3(0.35f, 0.35f, 1f);

            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.5f;
        }

        public int ScoreValue
        {
            get { return scoreValue; }
        }
    }
}
