using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Animation3D;
using Simulator.PhysicalModeling;
using Microsoft.Xna.Framework.Input;
using Simulator.GUI;
using Simulator.Helpers;

namespace Simulator.Main
{
    public class Simulator : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public Simulator()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)(GameConstants.RESOLUTION_X);
            graphics.PreferredBackBufferHeight = (int)(GameConstants.RESOLUTION_Y);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new MainMenu(true));
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            if (screenManager.Screens.Count != 0)
                graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            else
                graphics.GraphicsDevice.Clear(Color.Black);
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

    }
}
