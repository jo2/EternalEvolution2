using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;

namespace GameStateManagementSample.Maps {
    public abstract class EternalEvolutionMap {
        public readonly int mapWidth = 50;
        public readonly int mapHeight = 30;
        public IMap map;
        public EternalEvolutionMap lastMap;
        public Cell spawnPoint;
        public Player player;
        public List<Mob> mobs;
        public ContentManager content;

        public EternalEvolutionMap(ContentManager lContent) {
            content = lContent;
        }

        public virtual void LoadContent() {
            AddAggressiveEnemies(10);
        }

        public virtual string Update(GameTime gameTime) {
            return "";
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            foreach (var enemy in mobs) {
                if (Global.GameState == GameStates.Debugging || map.IsInFov(enemy.X, enemy.Y)) {
                    enemy.Draw(spriteBatch);
                }
            }
        }

        public bool ComparePositions(Player p, Cell c) {
            if (c.X == p.X && c.Y == p.Y) {
                return true;
            }
            return false;
        }

        public bool CompareCells(Cell a, Cell b) {
            if (a.X == b.X && a.Y == b.Y) {
                return true;
            }
            return false;
        }

        public Cell GetRandomEmptyCell() {
            while (true) {
                int x = Global.Random.Next(49);
                int y = Global.Random.Next(29);
                if (map.IsWalkable(x, y)) {
                    return map.GetCell(x, y);
                }
            }
        }

        private void AddAggressiveEnemies(int numberOfEnemies) {
            mobs = new List<Mob>();
            for (int i = 0; i < numberOfEnemies; i++) {
                Cell enemyCell = GetRandomEmptyCell();
                var pathFromMob = new PathToPlayer(player, map, content.Load<Texture2D>("white"));
                pathFromMob.CreateFrom(enemyCell.X, enemyCell.Y);
                var enemy = new Mob(map, pathFromMob) {
                    X = enemyCell.X,
                    Y = enemyCell.Y,
                    Sprite = content.Load<Texture2D>("hound"),
                    Scale = 0.25f,
                    ArmorClass = 10,
                    AttackBonus = 0,
                    Damage = Global.Random.Next(5),
                    Health = 10,
                    Name = "Hunting Hound " + i
                };
                mobs.Add(enemy);

            }
        }
    }
}
