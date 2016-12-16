﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;

namespace GameStateManagementSample
{
    public class Mob : Entity
    {
        private readonly PathToPlayer path;
        public float Scale { get; set; }
        private readonly IMap map;
        private bool isAwareOfPlayer;
        private int followingCount = 0;
        public int ExpReward { get; set; }

        // Update constructor to also take in an IMap
        public Mob(IMap lMap, PathToPlayer lPath)
        {
            map = lMap;
            path = lPath;
            ExpReward = 10;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float multiplier = Scale * Sprite.Width;
            spriteBatch.Draw(Sprite, new Vector2(X * multiplier, Y * multiplier), null, null, null, 0.0f, new Vector2(Scale, Scale), Color.White, SpriteEffects.None, 0.5f);
            path.Draw(spriteBatch);
        }

        public void Update()
        {
            if (!isAwareOfPlayer)
            {
                if (followingCount > 0)
                {
                    followingCount--;
                }
                else if (followingCount == 0 && !map.IsInFov(X, Y))
                {
                    isAwareOfPlayer = false;
                }
                if (map.IsInFov(X, Y))
                {
                    isAwareOfPlayer = true;
                }
            }
            if (isAwareOfPlayer)
            {
                path.CreateFrom(X, Y);
                if (Global.CombatManager.IsPlayerAt(path.FirstCell.X, path.FirstCell.Y))
                {
                    Global.CombatManager.Attack(this, Global.CombatManager.FigureAt(path.FirstCell.X, path.FirstCell.Y));
                }
                else
                {
                    X = path.FirstCell.X;
                    Y = path.FirstCell.Y;
                }
            }
        }
    }
}
