using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagementSample {
    public class PathToPlayer {
        private readonly Player player;
        private readonly IMap map;
        private readonly Texture2D sprite;
        private readonly PathFinder pathFinder;
        private IEnumerable<Cell> cells;

        public PathToPlayer(Player lPlayer, IMap lMap, Texture2D lSprite) {
            player = lPlayer;
            map = lMap;
            sprite = lSprite;
            pathFinder = new PathFinder(lMap);
        }
        public Cell FirstCell {
            get {
                return cells.First();
            }
        }
        public void CreateFrom(int x, int y) {
            try {
                Console.WriteLine("source: (" + x + "|" + y + "), dest: (" + player.X + "|" + player.Y + ")");
                Path p = pathFinder.ShortestPath(map.GetCell(x, y), map.GetCell(player.X, player.Y));
                //Console.WriteLine(p.Length);
                cells = (IEnumerable<Cell>) pathFinder.ShortestPath(map.GetCell(x, y), map.GetCell(player.X, player.Y)).Steps;
            }catch (PathNotFoundException e) {
                Console.WriteLine("Path Not Found: " + e.Message + ", " + e.Data);
            } catch (ArgumentOutOfRangeException e) {
                Console.WriteLine("Fehler aufgetreten: " + e.Message);
            }
        }
        public void Draw(SpriteBatch spriteBatch) {
            if (cells != null && Global.GameState == GameStates.Debugging) {
                foreach (Cell cell in cells) {
                    if (cell != null) {
                        float scale = .25f;
                        float multiplier = .25f * sprite.Width;
                        spriteBatch.Draw(sprite, new Vector2(cell.X * multiplier, cell.Y * multiplier),
                          null, null, null, 0.0f, new Vector2(scale, scale), Color.Blue * .2f,
                          SpriteEffects.None, 0.6f);
                    }
                }
            }
        }
    }
}
