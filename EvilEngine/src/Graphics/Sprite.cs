using EvilEngine.Physics;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvilEngine.Graphics
{
    public class Sprite
    {
        public Sprite(Texture2D texture, [CanBeNull] Transform hitbox = null, Rectangle? textureClip = null, Vector2? textureOffset = null, Color? textureColorFilter = null)
        {
            Texture = texture;
            TextureClip = textureClip;
            TextureOffset = textureOffset ?? Vector2.Zero;
            TextureColorFilter = textureColorFilter ?? Color.White;
            Hitbox = hitbox ?? new Transform();
        }

        public Texture2D Texture { get; set; }

        public Rectangle? TextureClip { get; set; }

        public Vector2 TextureOffset { get; set; }

        public Color TextureColorFilter { get; set; }
        
        public Transform Hitbox { get; set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture,
                position + TextureOffset,
                TextureClip,
                TextureColorFilter);
        }
    }
}