using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.GUI
{
    class Label
    {
        public string Text { get; set; }
        public Vector2 CenterPosition { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }

        protected SpriteFont font;

        public Label(string text, Vector2 centerPosition, float scale, Color color, SpriteFont spriteFont)
        {
            Text = text;
            CenterPosition = centerPosition;
            Color = color;
            Scale = scale;
            font = spriteFont;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, Text, CenterPosition - (Vector2.UnitX * MeasureString(Text).X / 2f),
                Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public void ChangeText(object text)
        {
            if (text.GetType() == typeof(float))
                Text = ((float)(text)).ToString("0.00");
            else
                Text = text.ToString();
        }

        protected Vector2 MeasureString(string text)
        {
            return font.MeasureString(text) * Scale;
        }
    }
}
