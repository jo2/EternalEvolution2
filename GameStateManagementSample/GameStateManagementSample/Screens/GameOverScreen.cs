using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GameStateManagementSample.Screens {
    class GameOverScreen : MenuScreen {
        #region Fields

        private static int[] mobsCount;
        private static GameplayScreen gameplayScreen;

        #endregion Fields

        #region Initialization

        private GameOverScreen(ScreenManager screenManager) : base("GAME OVER") {
            MenuEntry respawnMenuEntry = new MenuEntry("Respawn");
            MenuEntry mainMenuMenuEntry = new MenuEntry("Main Menu");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            respawnMenuEntry.Selected += RespawnMenuEntrySelected;
            mainMenuMenuEntry.Selected += MainMenuMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(respawnMenuEntry);
            MenuEntries.Add(mainMenuMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        public static void Load(ScreenManager screenManager, PlayerIndex? controllingPlayer, int[] lMobsCount, GameplayScreen gs) {
            mobsCount = lMobsCount;
            gameplayScreen = gs;

            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            BackgroundScreen bs = new BackgroundScreen();
            GameOverScreen gameOverScreen = new GameOverScreen(screenManager);

            screenManager.AddScreen(bs, controllingPlayer);
            screenManager.AddScreen(gameOverScreen, controllingPlayer);
        }

        #endregion Initialization

        #region Handle Input

        private void RespawnMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            /*gameplayScreen.player.Health = 50;
            GameplayScreen g = new GameplayScreen(true) {
                player = gameplayScreen.player,
                maps = gameplayScreen.maps,
                mobsPerMap = gameplayScreen.mobsPerMap,
                currentMap = gameplayScreen.currentMap,
                bodyHit = gameplayScreen.bodyHit,
            };*/
            for (int i = 0; i < mobsCount.Length; i++) {
                Console.WriteLine(mobsCount[i]);
            }

            GameplayScreen gs = new GameplayScreen(true) {
                mobsCount = mobsCount,
                maps = gameplayScreen.maps
            };

            Console.WriteLine("RESPAWN");
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gs);
        }

        private void MainMenuMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex) {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e) {
            ScreenManager.Game.Exit();
        }

        #endregion Handle Input

        #region Update and Draw

        /*public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);
                
                ScreenManager.Game.ResetElapsedTime();
            }
        }*/

        #endregion Update and Draw
    }
}
