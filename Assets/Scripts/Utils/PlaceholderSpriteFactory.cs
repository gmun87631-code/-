using System.Collections.Generic;
using UnityEngine;

namespace SideScrollerPrototype.Utils
{
    public static class PlaceholderSpriteFactory
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite GetSolidSprite(Color color)
        {
            return GetSprite("solid", color, false);
        }

        public static Sprite GetCircleSprite(Color color)
        {
            return GetSprite("circle", color, true);
        }

        private static Sprite GetSprite(string shape, Color color, bool circularMask)
        {
            string key = shape + "_" + ColorUtility.ToHtmlStringRGBA(color);
            Sprite sprite;

            if (Cache.TryGetValue(key, out sprite))
            {
                return sprite;
            }

            const int size = 32;
            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float radius = size * 0.42f;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Color pixel = color;

                    if (circularMask && Vector2.Distance(new Vector2(x, y), center) > radius)
                    {
                        pixel = clear;
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }
    }
}
