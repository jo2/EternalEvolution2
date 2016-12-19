using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System;

namespace GameStateManagementSample.Maps {
    public class DungeonNorth : EternalEvolutionMap {
        private Cell exit;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D doorSprite;

        public DungeonNorth(ContentManager lContent) : base(lContent) {
            LoadSprites();

            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(50, 30, 100, 7, 3);
            map = Map.Create(mapCreationStrategy);

            bool found = false;

            for (int y = mapHeight - 1; y > 1; y--) {
                if (found) {
                    break;
                }
                for (int x = 1; x < mapWidth; x++) {
                    try {
                        if (map.GetCell(x, y).IsWalkable) {
                            exit = map.GetCell(x, y);
                            found = true;
                            break;
                        }
                    } catch (IndexOutOfRangeException e) {
                        break;
                    }
                }
            }

            found = false;

            for (int y = mapHeight - 1; y > 1; y--) {
                if (found) {
                    break;
                }
                for (int x = 1; x < mapWidth; x++) {
                    try {
                        if (map.GetCell(x, y).IsWalkable) {
                            spawnPoint = map.GetCell(x, y);
                            if (CompareCells(exit, spawnPoint)) {
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
        }

        public override void LoadContent(int numberOfMobs) {
            base.LoadContent(numberOfMobs);
        }

        public override void LoadSprites() {
            wallSprite = content.Load<Texture2D>("wall");
            floorSprite = content.Load<Texture2D>("floor");
            doorSprite = content.Load<Texture2D>("door");
        }

        public override string Update(GameTime gameTime) {
            if (ComparePositions(player, exit)) {
                return "dungeonCentral";
            }
            return null;
        }

        public override void setSpawnpoint() {
            base.setSpawnpoint();
        }

        public override void Draw(SpriteBatch spriteBatch) {


            // TODO: Add your drawing code here
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
    }
}
