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

#endregion Using Statements

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen
    {
        #region Fields

        private ContentManager content;
        private SpriteFont gameFont;

        private Player player;
        private List<Mob> mobs;
        private Mob mob;

        private SpriteBatch spriteBatch;

        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private SoundEffect bodyHit;

        private IMap map;

        private InputState inputState;

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
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
            bodyHit = content.Load<SoundEffect>("Body_Hit_32");

            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(50, 30, 100, 7, 3);
            map = Map.Create(mapCreationStrategy);

            wallSprite = content.Load<Texture2D>("wall");
            Cell startingCell = GetRandomEmptyCell();
            player = new Player
            {
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
            UpdatePlayerFieldOfView();
            startingCell = GetRandomEmptyCell();
            var pathFromAggressiveEnemy = new PathToPlayer(player, map, content.Load<Texture2D>("white"));
            pathFromAggressiveEnemy.CreateFrom(startingCell.X, startingCell.Y);
            /*mob = new Mob(map, pathFromAggressiveEnemy)
            {
                X = startingCell.X,
                Y = startingCell.Y,
                Scale = 0.25f,
                Sprite = content.Load<Texture2D>("hound")
            };*/
            this.AddAggressiveEnemies(10);
            Global.GameState = GameStates.PlayerTurn;
            Global.CombatManager = new CombatManager(player, mobs, bodyHit);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion Initialization

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (player.Health <= 0)
            {
                Global.GameState = GameStates.GameOver;

                GameOverScreen.Load(ScreenManager, PlayerIndex.One);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (input.IsD(PlayerIndex.One))
            {
                if (Global.GameState == GameStates.PlayerTurn)
                {
                    Global.GameState = GameStates.Debugging;
                }
                else if (Global.GameState == GameStates.Debugging)
                {
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
            else
            {
                if (Global.GameState == GameStates.PlayerTurn && player.HandleInput(input, map))
                {
                    UpdatePlayerFieldOfView();
                    Global.GameState = GameStates.EnemyTurn;
                }
                if (Global.GameState == GameStates.EnemyTurn)
                {
                    //mob.Update();
                    foreach (var enemy in mobs)
                    {
                        enemy.Update();
                    }
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {


            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            int sizeOfSprites = 64;
            float scale = .25f;
            foreach (Cell cell in map.GetAllCells())
            {
                var position = new Vector2(cell.X * sizeOfSprites * scale, cell.Y * sizeOfSprites * scale);
                if (!cell.IsExplored && Global.GameState != GameStates.Debugging)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && Global.GameState != GameStates.Debugging)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(floorSprite, position, null, null, null, 0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                }
                else
                {
                    spriteBatch.Draw(wallSprite, position, null, null, null, 0.0f, new Vector2(scale, scale), tint, SpriteEffects.None, 0.8f);
                }
            }

            player.Draw(spriteBatch);
            foreach (var enemy in mobs)
            {
                if (Global.GameState == GameStates.Debugging || map.IsInFov(enemy.X, enemy.Y))
                {
                    enemy.Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion Update and Draw

        #region Custom Methods

        private Cell GetRandomEmptyCell()
        {
            while (true)
            {
                int x = Global.Random.Next(49);
                int y = Global.Random.Next(29);
                if (map.IsWalkable(x, y))
                {
                    return map.GetCell(x, y);
                }
            }
        }

        private void UpdatePlayerFieldOfView()
        {
            map.ComputeFov(player.X, player.Y, 30, true);
            foreach (Cell cell in map.GetAllCells())
            {
                if (map.IsInFov(cell.X, cell.Y))
                {
                    map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        private void AddAggressiveEnemies(int numberOfEnemies)
        {
            mobs = new List<Mob>();
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Cell enemyCell = GetRandomEmptyCell();
                var pathFromMob = new PathToPlayer(player, map, content.Load<Texture2D>("white"));
                pathFromMob.CreateFrom(enemyCell.X, enemyCell.Y);
                var enemy = new Mob(map, pathFromMob, 50)
                {
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

        #endregion Custom Methods
    }
}