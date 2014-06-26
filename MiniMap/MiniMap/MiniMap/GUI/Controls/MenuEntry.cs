using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.GUI
{
    class MenuEntry
    {
        public string Text { get; set; }
        public Vector2 Position { get; set; }

        public event EventHandler Selected;

        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, null);
        }



        public MenuEntry(string text)
        {
            this.Text = text;
        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Yellow : Color.White;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, Text, Position, color, 0,
                                  origin, 1f, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

    }
}
