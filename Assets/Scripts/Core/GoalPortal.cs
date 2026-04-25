using UnityEngine;

namespace SideScrollerPrototype.Core
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class GoalPortal : MonoBehaviour
    {
        public void Initialize()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Utils.PlaceholderSpriteFactory.GetSolidSprite(new Color(0.96f, 0.88f, 0.28f));
            spriteRenderer.sortingOrder = 2;
            transform.localScale = new Vector3(1f, 2f, 1f);

            BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
            collider2D.isTrigger = true;
            collider2D.size = Vector2.one;
        }
    }
}
