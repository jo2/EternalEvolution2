using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using RogueSharp.MapCreation;
using System;

namespace GameStateManagementSample.Maps {
    public class DungeonCentral : EternalEvolutionMap {
        private Cell leftExit;
        private Cell rightExit;
        private Cell topExit;
        private Cell bottomExit;
        private Cell centerExit;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D doorSprite;

        public DungeonCentral(ContentManager lContent) : base(lContent) {
            wallSprite = content.Load<Texture2D>("wall");
            floorSprite = content.Load<Texture2D>("floor");
            doorSprite = content.Load<Texture2D>("door");

            IMapCreationStrategy<Map> mapCreationStrategy = new BorderOnlyMapCreationStrategy<Map>(mapWidth, mapHeight);
            map = Map.Create(mapCreationStrategy);

            leftExit = map.GetCell(1, mapHeight / 2);
            rightExit = map.GetCell(mapWidth - 2, mapHeight / 2);
            topExit = map.GetCell(mapWidth / 2, 1);
            bottomExit = map.GetCell(mapWidth / 2, mapHeight - 2);
            centerExit = map.GetCell(mapWidth / 2, mapHeight / 2);

            spawnPoint = map.GetCell(centerExit.X, centerExit.Y + 1);
        }

        public override void LoadContent() {
            base.LoadContent();
        }

        public override string Update(GameTime gameTime) {
            if (lastMap is DungeonNorth) {
                spawnPoint = map.GetCell(topExit.X, topExit.Y + 1);
            } else if (lastMap is DungeonEast) {
                spawnPoint = map.GetCell(rightExit.X - 1, rightExit.Y);
            } else if (lastMap is DungeonSouth) {
                spawnPoint = map.GetCell(bottomExit.X, bottomExit.Y - 1);
            } else if (lastMap is DungeonWest) {
                spawnPoint = map.GetCell(leftExit.X + 1, leftExit.Y);
            } else if (lastMap is Forest) {
                spawnPoint = map.GetCell(centerExit.X, centerExit.Y + 1);
            }
            if (ComparePositions(player, topExit) && player.hasMoved) {
                Console.WriteLine("north");
                return "dungeonNorth";
            } else if (ComparePositions(player, rightExit) && player.hasMoved) {
                Console.WriteLine("east");
                return "dungeonEast";
            } else if (ComparePositions(player, bottomExit) && player.hasMoved) {
                Console.WriteLine("south");
                return "dungeonSouth";
            } else if (ComparePositions(player, leftExit) && player.hasMoved) {
                Console.WriteLine("west");
                return "dungeonWest";
            } else if (ComparePositions(player, centerExit) && player.hasMoved) {
                Console.WriteLine("forrest");
                return "forrest";
            }
            return null;
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

            var exitPos = new Vector2(leftExit.X * sizeOfSprites * scale, leftExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);
            exitPos = new Vector2(rightExit.X * sizeOfSprites * scale, rightExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);
            exitPos = new Vector2(topExit.X * sizeOfSprites * scale, topExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);
            exitPos = new Vector2(bottomExit.X * sizeOfSprites * scale, bottomExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);
            exitPos = new Vector2(centerExit.X * sizeOfSprites * scale, centerExit.Y * sizeOfSprites * scale);
            spriteBatch.Draw(doorSprite, exitPos, null, null, null, 0.0f, new Vector2(scale, scale), Color.Gray, SpriteEffects.None, 0.8f);

            base.Draw(spriteBatch);
        }
    }
}
