using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.GUI
{
    class HeadedLabel : Label
    {
        public string Heading { get; set; }

        public HeadedLabel(string heading, string text, Vector2 centerPosition, float scale, Color color, SpriteFont spriteFont)
            : base(text, centerPosition, scale, color, spriteFont)
        {
            Heading = heading;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, Heading + ":", CenterPosition - Vector2.UnitX * MeasureString(Heading + ":").X / 2f,
                Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Text, CenterPosition + new Vector2(-MeasureString(Text).X / 2f, MeasureString(Text).Y),
                Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
