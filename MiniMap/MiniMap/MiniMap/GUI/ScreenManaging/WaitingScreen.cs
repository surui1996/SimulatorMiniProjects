using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Simulator.GUI
{
    class WaitingScreen : GameScreen
    {
        GameScreen screenToLoad;
        float timePassed;
        public WaitingScreen(GameScreen screen)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);
            screenToLoad = screen;
            timePassed = 0;
        }

        public override void Update(GameTime gameTime, KeyboardState state)
        {
            base.Update(gameTime, state);
            timePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timePassed > 0.5f)//(ScreenManager.Screens.Count == 2)
            {
                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(screenToLoad);
            }
        }
    }
}
