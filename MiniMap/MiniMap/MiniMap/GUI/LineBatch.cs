using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.GUI
{
    static class LineBatch
    {
        static private Texture2D pixelTexture;
        static private bool _set_data = false;

        static public void Init(GraphicsDevice device)
        {
            pixelTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            pixelTexture.SetData(new[] { Color.White });
        }

        static public void DrawLine(SpriteBatch spriteBatch, Color color,
                                    Vector2 point1, Vector2 point2)
        {
            DrawLine(spriteBatch, color, point1, point2, 1.0f, 0.0f);
        }

        static public void DrawLine(SpriteBatch spriteBatch, Color color, Vector2 point1,
                                    Vector2 point2, float thinkness, float Layer)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = (point2 - point1).Length();

            spriteBatch.Draw(pixelTexture, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, thinkness),
                       SpriteEffects.None, Layer);
        }
    }
}
