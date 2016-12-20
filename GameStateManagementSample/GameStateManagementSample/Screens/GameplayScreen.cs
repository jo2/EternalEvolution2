#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using GameStateManagementSample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharp.MapCreation;
using System;
using System.Threading;
using System.Collections.Generic;
using RogueSharp.DiceNotation;
using GameStateManagementSample.Screens;
using Microsoft.Xna.Framework.Audio;
using GameStateManagementSample.Maps;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

#endregion Using Statements

namespace GameStateManagement {
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen {
        #region Fields

        public readonly int mapWidth = 50;
        public readonly int mapHeight = 30;

        public ContentManager content;
        public SpriteFont gameFont;

        public Player player;

        public int[] mobsCount;

        public IDictionary<string, EternalEvolutionMap> maps;
        public IDictionary<string, List<Mob>> mobsPerMap;
        public EternalEvolutionMap currentMap;

        public SpriteBatch spriteBatch;

        public SoundEffect bodyHit;
        private bool loaded;

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool lLoaded) {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            loaded = lLoaded;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            spriteBatch = ScreenManager.SpriteBatch;

            bodyHit = content.Load<SoundEffect>("Body_Hit_32");

            if (!loaded) {
                maps = new Dictionary<string, EternalEvolutionMap>();
                maps.Add("dungeonCentral", new DungeonCentral(content));
                maps.Add("dungeonNorth", new DungeonNorth(content));
                maps.Add("dungeonEast", new DungeonEast(content));
                maps.Add("dungeonSouth", new DungeonSouth(content));
                maps.Add("dungeonWest", new DungeonWest(content));
                maps.Add("forest", new Forest(content));
                maps.Add("city", new City(content));
            } else {
                EternalEvolutionMap map;
                foreach (string s in maps.Keys) {
                    maps.TryGetValue(s, out map);
                    map.LoadSprites();
                }
            }

            Cell startingCell = new Cell(2, 3, true, true, true);
            player = new Player {
                X = startingCell.X,
                Y = startingCell.Y,
                Scale = 0.25f,
                Sprite = content.Load<Texture2D>("player"),
                ArmorClass = 15,
                AttackBonus = 1,
                Damage = Global.Random.Next(10),
                Health = 50,
                Name = "Mr. Rouge"
            };

            EternalEvolutionMap m;
            mobsPerMap = new Dictionary<string, List<Mob>>();
            if (loaded) {
                int i = 0;
                foreach (string s in maps.Keys) {
                    Console.WriteLine("mobs in " + s + " after Spawn: " + mobsCount[i]);
                    maps.TryGetValue(s, out m);
                    m.player = player;
                    m.LoadContent(mobsCount[i]);
                    mobsPerMap.Add(s, m.mobs);
                    i++;
                }
            } else {
                foreach (string s in maps.Keys) {
                    maps.TryGetValue(s, out m);
                    m.player = player;
                    m.LoadContent(10);
                    mobsPerMap.Add(s, m.mobs);
                }
            }

            maps.TryGetValue("city", out currentMap);
            currentMap.player = player;

            if (currentMap.GetType() == typeof(City)) {
                foreach (Cell cell in currentMap.map.GetAllCells()) {
                    currentMap.map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            } else {
                foreach (Cell cell in currentMap.map.GetAllCells()) {
                    currentMap.map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, false);
                }
            }

            UpdatePlayerFieldOfView();
            loaded = false;

            Global.GameState = GameStates.PlayerTurn;
            Global.CombatManager = new CombatManager(player, currentMap.mobs, bodyHit, content);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent() {
            content.Unload();
        }

        #endregion Initialization

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);

  

            if(player.Level > 0)
            {
                if((player.Experience >= player.Level * 100))
                {
                    player.Level++;
                    player.Experience -= (player.Level-1) * 100;
                    player.Health += 5;
                    player.Log = "You have advanced to level  " + player.Level;
                }
            }
            if(player.Level == 0)
            {
                if ((player.Experience >= 100) && (player.Level == 0))
                {
                    player.Level++;
                    player.Experience = player.Experience - 100;
                    player.Health += 5;
                } 
            }

            if (player.Health <= 0) {
                mobsCount = new int[7];
                int i = 0;
                List<Mob> list;
                foreach (string s in mobsPerMap.Keys) {
                    mobsPerMap.TryGetValue(s, out list);
                    mobsCount[i] = list.Count;
                    i++;
                }
                Global.GameState = GameStates.GameOver;
                GameOverScreen.Load(ScreenManager, PlayerIndex.One, mobsCount, this);
            }
            string ret = currentMap.Update(gameTime);
            if (ret != null) {
                //Map wechseln
                Debug.WriteLine("change map to: " + ret);
                EternalEvolutionMap temp = currentMap;
                List<Mob> tempList = currentMap.mobs;
                maps.TryGetValue(ret, out currentMap);
                currentMap.lastMap = temp;
                //setze SPawnpoint
                currentMap.setSpawnpoint();
                player.X = currentMap.spawnPoint.X;
                player.Y = currentMap.spawnPoint.Y;
                currentMap.player = player;
                mobsPerMap.TryGetValue(ret, out currentMap.mobs);

                Global.GameState = GameStates.PlayerTurn;
                Global.CombatManager = new CombatManager(player, currentMap.mobs, bodyHit, content);

                Draw(gameTime);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input) {
            if (input == null)
                throw new ArgumentNullException("input");

            int playerIndex = (int) ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected) {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            } else if (input.IsD(PlayerIndex.One)) {
                if (Global.GameState == GameStates.PlayerTurn) {
                    Global.GameState = GameStates.Debugging;
                } else if (Global.GameState == GameStates.Debugging) {
                    Global.GameState = GameStates.PlayerTurn;
                }
            } else {
                if (Global.GameState == GameStates.PlayerTurn && player.HandleInput(input, currentMap.map)) {
                    UpdatePlayerFieldOfView();
                    Global.GameState = GameStates.EnemyTurn;
                }
                if (Global.GameState == GameStates.EnemyTurn) {
                    foreach (var enemy in currentMap.mobs) {
                        enemy.Update();
                    }
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            player.Draw(spriteBatch);

            spriteBatch.DrawString(gameFont, "HP:  " + player.Health, new Vector2(0, 50), Color.Red, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);
            spriteBatch.DrawString(gameFont, "Armor:  " + player.ArmorClass, new Vector2(0, 100), Color.Green, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);
            spriteBatch.DrawString(gameFont, "Attack:   " + player.AttackBonus, new Vector2(0, 150), Color.Purple, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);
            spriteBatch.DrawString(gameFont, "Experience: " + player.Experience, new Vector2(0, 200), Color.Purple, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);

            spriteBatch.DrawString(gameFont, "Level:  " + player.Level, new Vector2(0, 250), Color.Purple, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);

            spriteBatch.DrawString(gameFont, "Log:  " + player.Log, new Vector2(0, 450), Color.Purple, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);

            currentMap.Draw(spriteBatch);

            spriteBatch.End();
        }

        #endregion Update and Draw

        #region Custom Methods

        private void UpdatePlayerFieldOfView() {
            currentMap.map.ComputeFov(player.X, player.Y, 30, true);
            foreach (Cell cell in currentMap.map.GetAllCells()) {
                if (currentMap.map.IsInFov(cell.X, cell.Y)) {
                    currentMap.map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        #endregion Custom Methods
    }
}