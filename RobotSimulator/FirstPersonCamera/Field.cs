using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotSimulator
{
    class Field
    {
        Box3 wallRed, wallBlue, carpet3;
        //TexturedWall dynamicRightVision, staticRightVision;

        public Field(ContentManager content)
        {
            wallRed = new Box3(content.Load<Texture2D>("visionRed"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
               Vector3.Zero, FieldConstants.C);

            wallBlue = new Box3(content.Load<Texture2D>("visionBlue"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
                new Vector3(0, 0, FieldConstants.HEIGHT), FieldConstants.C);

            carpet3 = new Box3(content.Load<Texture2D>("carpet"), FieldConstants.WIDTH, 0, FieldConstants.HEIGHT,
                Vector3.Zero, FieldConstants.C);
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            wallRed.Draw(graphicsDevice, effect);
            wallBlue.Draw(graphicsDevice, effect);
            carpet3.Draw(graphicsDevice, effect);
        }
    }
}
