using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Simulator.Main;

namespace Simulator.GUI
{
    class EndOfMatchMenu : MenuScreen
    {
        public EndOfMatchMenu()
            : base("Match Ended!")
        {
            // Create our menu entries.
            MenuEntry rematchMenuEntry = new MenuEntry("Rematch");
            MenuEntry quitGameMenuEntry = new MenuEntry("Main Menu");
            
            // Hook up menu event handlers.
            rematchMenuEntry.Selected += Rematch;
            quitGameMenuEntry.Selected += MainMenu;

            // Add entries to the menu.
            MenuEntries.Add(rematchMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
            BlocksUpdate = true;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToColor(TransitionAlpha * 2f / 3f, Color.Black);
            base.Draw(gameTime);
        }

        void Rematch(object sender, EventArgs e)
        {
            ExitScreen();
            foreach (GameScreen screen in ScreenManager.Screens)
                if (screen.GetType() == typeof(SimulatorGame))
                    (screen as SimulatorGame).Rematch();
        }

        void MainMenu(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.Screens)
                screen.ExitScreen();

            ScreenManager.AddScreen(new MainMenu(false));
        }
    }
}
