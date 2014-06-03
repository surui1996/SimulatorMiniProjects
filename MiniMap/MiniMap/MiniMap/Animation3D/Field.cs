using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MiniMap.Animation3D
{

    class Field
    {
        Box wallRed, wallBlue, carpet3, boundryLeft, boundryRight;
        //TexturedWall dynamicRightVision, staticRightVision;

        public Field(ContentManager content)
        {
            wallRed = new Box(content.Load<Texture2D>("visionRed"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
               new Vector3(0, 0, FieldConstants.HEIGHT), FieldConstants.C);

            wallBlue = new Box(content.Load<Texture2D>("visionBlue"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
                Vector3.Zero, FieldConstants.C);

            boundryRight = new Box(content.Load<Texture2D>("boundryGrey"), FieldConstants.WIDTH / 50,
                FieldConstants.HEIGHT_ABOVE_CARPET / 10, FieldConstants.HEIGHT,
                -Vector3.UnitX * FieldConstants.WIDTH / 50, FieldConstants.C);

            boundryLeft = new Box(content.Load<Texture2D>("boundryGrey"), FieldConstants.WIDTH / 50,
               FieldConstants.HEIGHT_ABOVE_CARPET / 10, FieldConstants.HEIGHT,
               Vector3.UnitX * FieldConstants.WIDTH, FieldConstants.C);

            carpet3 = new Box(content.Load<Texture2D>("carpet3D"), FieldConstants.WIDTH, 2 / FieldConstants.C, FieldConstants.HEIGHT,
                Vector3.Zero, FieldConstants.C);

            
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            wallRed.Draw(graphicsDevice, effect);
            wallBlue.Draw(graphicsDevice, effect);
            boundryRight.Draw(graphicsDevice, effect);
            boundryLeft.Draw(graphicsDevice, effect);
            carpet3.Draw(graphicsDevice, effect);
        }

    }
}
