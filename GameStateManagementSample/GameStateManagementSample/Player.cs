using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System.Collections.Generic;

namespace GameStateManagementSample
{
    public class Player : Entity
    {
        public float Scale { get; set; }

        public int Experience { get; set; }


        public void Draw(SpriteBatch spriteBatch)
        {
            float multiplier = Scale * Sprite.Width;
            spriteBatch.Draw(Sprite, new Vector2(X * multiplier, Y * multiplier), null, null, null, 0.0f, new Vector2(Scale, Scale), Color.White, SpriteEffects.None, 0.5f);
        }

        public bool HandleInput(InputState inputState, IMap map)
        {
            if (inputState.IsLeft(PlayerIndex.One))
            {
                int tempX = X - 1;
                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);
                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            else if (inputState.IsRight(PlayerIndex.One))
            {
                int tempX = X + 1;
                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);
                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true; 
                }
            }
            else if (inputState.IsUp(PlayerIndex.One))
            {
                int tempY = Y - 1;
                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            else if (inputState.IsDown(PlayerIndex.One))
            {
                int tempY = Y + 1;
                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
