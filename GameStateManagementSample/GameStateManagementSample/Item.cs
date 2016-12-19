using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagementSample
{
    public class Item : Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public int AttackBonus { get; set; }
        public int ArmorClass { get; set; }
        public int HealthBonus { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
        public float Scale { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            float multiplier = Scale * Sprite.Width;
            spriteBatch.Draw(Sprite, new Vector2(X * multiplier, Y * multiplier), null, null, null, 0.0f, new Vector2(Scale, Scale), Color.White, SpriteEffects.None, 0.5f);
        }
    }
}
