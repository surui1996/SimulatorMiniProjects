using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.GUI
{
    class Timer : Label
    {
        private TimeSpan startTime;

        public Timer(string text, Vector2 centerPosition, float scale, Color color, SpriteFont spriteFont)
            : base(text, centerPosition, scale, color, spriteFont)
        {}

        public void Reset(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime;
        }

        public void Update(GameTime gameTime)
        {
            Text = gameTime.TotalGameTime.Subtract(startTime).ToString(@"m\:ss");
        }
    }
}
