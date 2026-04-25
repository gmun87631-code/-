using UnityEngine;

namespace SideScrollerPrototype.Config
{
    [CreateAssetMenu(menuName = "Side Scroller Prototype/Enemy Behavior", fileName = "EnemyBehaviorDefinition")]
    public class EnemyBehaviorDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId = "slime";
        public string displayName = "Slime";

        [Header("Presentation")]
        public Sprite sprite;
        public Color tint = new Color(0.75f, 0.2f, 0.2f);
        public Vector2 colliderSize = new Vector2(0.9f, 0.9f);

        [Header("Behavior")]
        public float moveSpeed = 2f;
        public float patrolDistance = 2.5f;
        public float contactKnockback = 8f;

        [Header("Scoring")]
        public int touchPenalty = 10;
        public int stompBonus = 15;
    }
}
