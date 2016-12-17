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

#endregion Using Statements

namespace GameStateManagement {
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen {
        #region Fields

        private readonly int mapWidth = 50;
        private readonly int mapHeight = 30;

        private ContentManager content;
        private SpriteFont gameFont;

        private Player player;

        private IDictionary<string, EternalEvolutionMap> maps;
        private IDictionary<string, List<Mob>> mobsPerMap;
        private EternalEvolutionMap currentMap;

        private SpriteBatch spriteBatch;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D doorSprite;
        private SoundEffect bodyHit;

        private InputState inputState;

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen() {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
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

            wallSprite = content.Load<Texture2D>("wall");
            floorSprite = content.Load<Texture2D>("floor");
            doorSprite = content.Load<Texture2D>("door");
            bodyHit = content.Load<SoundEffect>("Body_Hit_32");

            maps = new Dictionary<string, EternalEvolutionMap>();
            maps.Add("dungeonCentral", new DungeonCentral(content));
            maps.Add("dungeonNorth", new DungeonNorth(content));
            maps.Add("dungeonEast", new DungeonEast(content));
            maps.Add("dungeonSouth", new DungeonSouth(content));
            maps.Add("dungeonWest", new DungeonWest(content));
            maps.Add("forest", new Forest(content));
            maps.Add("city", new City(content));
            
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
            maps.TryGetValue("dungeonCentral", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for central: " + m.mobs);
            mobsPerMap.Add("dungeonCentral", m.mobs);

            maps.TryGetValue("dungeonNorth", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for north: " + m.mobs);
            mobsPerMap.Add("dungeonNorth", m.mobs);

            maps.TryGetValue("dungeonEast", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for east: " + m.mobs);
            mobsPerMap.Add("dungeonEast", m.mobs);

            maps.TryGetValue("dungeonSouth", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for south: " + m.mobs);
            mobsPerMap.Add("dungeonSouth", m.mobs);

            maps.TryGetValue("dungeonWest", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for west: " + m.mobs);
            mobsPerMap.Add("dungeonWest", m.mobs);

            maps.TryGetValue("forest", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for forest: " + m.mobs);
            mobsPerMap.Add("forest", m.mobs);

            maps.TryGetValue("city", out m);
            m.player = player;
            m.LoadContent();
            Console.WriteLine("mobs for city: " + m.mobs);
            mobsPerMap.Add("city", m.mobs);

            maps.TryGetValue("city", out currentMap);
            currentMap.player = player;

            currentMap.LoadContent();

            UpdatePlayerFieldOfView();

            startingCell = currentMap.GetRandomEmptyCell();
            var pathFromAggressiveEnemy = new PathToPlayer(player, currentMap.map, content.Load<Texture2D>("white"));
            pathFromAggressiveEnemy.CreateFrom(startingCell.X, startingCell.Y);

            Global.GameState = GameStates.PlayerTurn;
            Global.CombatManager = new CombatManager(player, currentMap.mobs, bodyHit);
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
            if (player.Health <= 0) {
                Global.GameState = GameStates.GameOver;
                GameOverScreen.Load(ScreenManager, PlayerIndex.One);
            }
            string ret = currentMap.Update(gameTime);
            if (ret != null) {
                //Map wechseln
                Debug.WriteLine("change map to: " + ret);
                EternalEvolutionMap temp = currentMap;
                List<Mob> tempList = currentMap.mobs;
                maps.TryGetValue(ret, out currentMap);
                currentMap.lastMap = temp;
                player.X = currentMap.spawnPoint.X;
                player.Y = currentMap.spawnPoint.Y;
                currentMap.player = player;
                mobsPerMap.TryGetValue(ret, out currentMap.mobs);

                Console.WriteLine("mobs: " + currentMap.mobs);

                Global.GameState = GameStates.PlayerTurn;
                Global.CombatManager = new CombatManager(player, currentMap.mobs, bodyHit);

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

            //Console.WriteLine("map to draw: " + currentMap.GetType());

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