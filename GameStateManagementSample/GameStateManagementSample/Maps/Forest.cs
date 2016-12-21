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
    public class Forest : EternalEvolutionMap {
        private Cell dungeonExit;
        private Cell cityExit;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D doorSprite;

        public Forest(ContentManager lContent) : base(lContent) {
            LoadSprites();

            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(50, 30, 100, 7, 3);
            map = Map.Create(mapCreationStrategy);

            bool found = false;

            for (int x = mapWidth - 1; x > 1; x--) {
                if (found) {
                    break;
                }
                for (int y = 1; y < mapHeight; y++) {
                    try {
                        if (map.GetCell(x, y).IsWalkable) {
                            cityExit = map.GetCell(x, y);
                            found = true;
                            break;
                        }
                    } catch (IndexOutOfRangeException e) {
                        break;
                    }
                }
            }

            found = false;

            for (int x = mapWidth - 1; x > 1; x--) {
                if (found) {
                    break;
                }
                for (int y = 1; y < mapHeight; y++) {
                    try {
                        if (map.GetCell(x, y).IsWalkable) {
                            spawnPoint = map.GetCell(x, y);
                            if (CompareCells(cityExit, spawnPoint)) {
                                continue;
                            } else {
                                found = true;
                                break;
                            }
                        }
                    } catch (IndexOutOfRangeException e) {
                        break;
                    }
                }
            }

            found = false;

            for (int x = 1; x < mapWidth; x++) {
                if (found) {
                    break;
                }
                for (int y = 1; y < mapHeight; y++) {
                    try {
                        if (map.GetCell(x, y).IsWalkable) {
                            dungeonExit = map.GetCell(x, y);
                            found = true;
                            break;
                        }
                    } catch (IndexOutOfRangeException e) {
                        break;
                    }
                }
            }
        }

        public override void LoadSprites() {
            wallSprite = content.Load<Texture2D>("forest_wall");
            floorSprite = content.Load<Texture2D>("forest_floor");
            doorSprite = content.Load<Texture2D>("door");
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

            var exitPos = new Vector2(dungeonExit.X * sizeOfSprites * scale, dungeonExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.1f);
            exitPos = new Vector2(cityExit.X * sizeOfSprites * scale, cityExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.1f);

            base.Draw(spriteBatch);
        }

        public override void LoadContent(int numberOfMobs) {
            base.LoadContent(numberOfMobs);
        }

        public override string Update(GameTime gameTime) {
            if (ComparePositions(player, cityExit) && player.hasMoved) {
                return "city";
            } else if (ComparePositions(player, dungeonExit) && player.hasMoved) {
                return "dungeonCentral";
            }
            return null;
        }

        public override void setSpawnpoint() {
            if (lastMap.GetType() == typeof(City)) {
                spawnPoint = map.GetCell(cityExit.X, cityExit.Y + 1);
            } else if (lastMap.GetType() == typeof(DungeonCentral)) {
                spawnPoint = map.GetCell(dungeonExit.X, dungeonExit.Y - 1);
            }
        }
    }
}
