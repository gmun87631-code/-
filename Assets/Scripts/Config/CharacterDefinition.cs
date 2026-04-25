using UnityEngine;

namespace SideScrollerPrototype.Config
{
    [CreateAssetMenu(menuName = "Side Scroller Prototype/Character Definition", fileName = "CharacterDefinition")]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string characterId = "hero";
        public string displayName = "Hero";

        [Header("Presentation")]
        public Sprite sprite;
        public Color tint = Color.white;
        public Vector2 colliderSize = new Vector2(0.8f, 1.2f);

        [Header("Stats")]
        public float moveSpeed = 7f;
        public float acceleration = 55f;
        public float deceleration = 65f;
        public float jumpForce = 13f;
        [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.45f;
        public int maxHealth = 3;
    }
}
