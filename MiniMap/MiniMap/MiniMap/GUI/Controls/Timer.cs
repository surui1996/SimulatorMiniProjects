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
        public TimeSpan TimePassed { get; set; }
        public TimeSpan InitialValue { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public bool CountDown { get; set; }

        public Timer(string text, Vector2 centerPosition, float scale, Color color, SpriteFont spriteFont)
            : base(text, centerPosition, scale, color, spriteFont)
        {
            Reset();
            CountDown = false;
        }

        public void Reset()
        {
            Reset(TimeSpan.FromSeconds(0));
        }

        public void Reset(TimeSpan resetValue)
        {
            InitialValue = resetValue;
            TimePassed = TimeSpan.FromSeconds(1);
            CurrentTime = InitialValue;
        }

        public void Update(GameTime gameTime)
        {
            TimePassed = TimePassed.Add(gameTime.ElapsedGameTime);

            TimeSpan show;

            if (CountDown)
                show = InitialValue.Subtract(TimePassed);
            else
                show = InitialValue.Add(TimePassed);
            
            if (show.Ticks < 0)
                show = TimeSpan.FromSeconds(0);

            CurrentTime = show;

            Text = show.ToString(@"m\:ss");
        }
    }
}
