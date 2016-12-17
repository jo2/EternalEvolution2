using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RogueSharp.MapCreation;
using RogueSharp;

namespace GameStateManagementSample.Maps {
    public class City : EternalEvolutionMap {
        private Cell exit;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D doorSprite;

        public City(ContentManager lContent) : base(lContent) {
            wallSprite = content.Load<Texture2D>("city_wall");
            floorSprite = content.Load<Texture2D>("city_floor");
            doorSprite = content.Load<Texture2D>("door");

            IMapCreationStrategy<Map> mapCreationStrategy = new ForestMapCreationStrategy<Map>(mapWidth, mapHeight);
            map = Map.Create(mapCreationStrategy);

            exit = map.GetCell(1, 1);
            
            spawnPoint = map.GetCell(1, 2);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            int sizeOfSprites = 64;
            float scale = .25f;
            foreach (Cell cell in map.GetAllCells()) {
                var position = new Vector2(cell.X * sizeOfSprites * scale, cell.Y * sizeOfSprites * scale);
                if (!cell.IsExplored && Global.GameState != GameStates.Debugging) {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && Global.GameState != GameStates.Debugging) {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable) {
                    spriteBatch.Draw(floorSprite, position, null, null, null, 0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                } else {
                    spriteBatch.Draw(wallSprite, position, null, null, null, 0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                }
            }

            var exitPos = new Vector2(exit.X * sizeOfSprites * scale, exit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);
            
            base.Draw(spriteBatch);
        }

        public override void LoadContent() {
            mobs = new List<Mob>();
        }

        public override string Update(GameTime gameTime) {
            if (lastMap is DungeonNorth) {
                spawnPoint = map.GetCell(exit.X, exit.Y + 1);
            }
            if (ComparePositions(player, exit) && player.hasMoved) {
                return "forest";
            }
            return null;
        }
    }
}
