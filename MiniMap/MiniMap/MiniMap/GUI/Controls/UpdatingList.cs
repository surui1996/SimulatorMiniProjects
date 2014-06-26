using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.GUI
{
    class UpdatingList : Label
    {
        private int rows;

        private string[] list;

        public UpdatingList(int numRows, Vector2 centerPosition, float scale, Color color, SpriteFont spriteFont)
            : base("", centerPosition, scale, color, spriteFont)
        {
            rows = numRows;
            list = new string[rows];
        }

        public void Add(string s)
        {
            for (int i = 0; i < rows; i++)
                if (s == list[i])
                    return;

            Text = s + "\n" + list[0] + "\n" + list[1] + "\n" + list[2];
            
            for (int i = rows - 1; i > 0; i--)
                    list[i] = list[i - 1];
                
            list[0] = s;            
        }

        public void Reset()
        {
            for (int i = 0; i < rows; i++)
                list[i] = null;
            Text = "";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < rows; i++)
                if (list[i] != null && list[i] != "")
                    spriteBatch.DrawString(font, list[i], CenterPosition + new Vector2(0, MeasureString(list[i]).Y * i),
                    Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
               
        }
    }

}
