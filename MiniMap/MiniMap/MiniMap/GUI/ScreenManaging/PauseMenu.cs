using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Simulator.Main;

namespace Simulator.GUI
{
    class PauseMenu : MenuScreen
    {
        public PauseMenu()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Main Menu");
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += Resume;
            quitGameMenuEntry.Selected += MainMenu;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
            BlocksUpdate = true;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToColor(TransitionAlpha * 2f / 3f, Color.Black);
            base.Draw(gameTime);
        }

        void Resume(object sender, EventArgs e)
        {
            ExitScreen();
        }

        void MainMenu(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            foreach (GameScreen screen in ScreenManager.Screens)
                screen.ExitScreen();
            ScreenManager.AddScreen(new WaitingScreen(new MainMenu(false)));
        }
    }
}
