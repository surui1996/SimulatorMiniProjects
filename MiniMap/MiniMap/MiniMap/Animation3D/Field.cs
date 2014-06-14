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
        Box lowGoal, truss, trussVertical;
        //TexturedWall dynamicRightVision, staticRightVision;

        public Field(ContentManager content)
        {
            wallRed = new Box(content.Load<Texture2D>("visionRed"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0.1f,
               new Vector3(0, 0, FieldConstants.HEIGHT), FieldConstants.C);

            wallBlue = new Box(content.Load<Texture2D>("wallBlue"), FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0.1f,
                Vector3.Zero, FieldConstants.C);

            boundryRight = new Box(content.Load<Texture2D>("boundryGrey"), FieldConstants.WIDTH / 50,
                FieldConstants.HEIGHT_ABOVE_CARPET / 10, FieldConstants.HEIGHT,
                -Vector3.UnitX * FieldConstants.WIDTH / 50, FieldConstants.C);

            boundryLeft = new Box(content.Load<Texture2D>("boundryGrey"), FieldConstants.WIDTH / 50,
               FieldConstants.HEIGHT_ABOVE_CARPET / 10, FieldConstants.HEIGHT,
               Vector3.UnitX * FieldConstants.WIDTH, FieldConstants.C);

            carpet3 = new Box(content.Load<Texture2D>("carpet3D"), FieldConstants.WIDTH, 2 / FieldConstants.C, FieldConstants.HEIGHT,
                Vector3.Zero, FieldConstants.C);

            float a = FieldConstants.LOWGOAL_WIDTH;


            lowGoal = new Box(content.Load<Texture2D>("goalFrame"), a, a, a,
               Vector3.Zero, FieldConstants.C);

            truss = new Box(content.Load<Texture2D>("truss"), FieldConstants.WIDTH * 1.5f, FieldConstants.TRUSS_SQUARE_EDGE, FieldConstants.TRUSS_SQUARE_EDGE,
               new Vector3(-0.25f * FieldConstants.WIDTH, FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 2 - 0.5f), FieldConstants.C);

            trussVertical = new Box(content.Load<Texture2D>("trussVertical"), FieldConstants.TRUSS_SQUARE_EDGE, FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.TRUSS_SQUARE_EDGE,
              new Vector3(-0.25f * FieldConstants.WIDTH, 0, FieldConstants.HEIGHT / 2 - 0.5f), FieldConstants.C);

        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            wallRed.Draw(graphicsDevice, effect);
            wallBlue.Draw(graphicsDevice, effect);
            boundryRight.Draw(graphicsDevice, effect);
            boundryLeft.Draw(graphicsDevice, effect);
            carpet3.Draw(graphicsDevice, effect);

            Matrix oldWorld = effect.World;
            lowGoal.Draw(graphicsDevice, effect);
            effect.World = Matrix.CreateTranslation(Vector3.UnitZ * ((FieldConstants.HEIGHT - FieldConstants.LOWGOAL_WIDTH ) * FieldConstants.C - 1f)) * effect.World;
            lowGoal.Draw(graphicsDevice, effect);
            effect.World = oldWorld;

            effect.World = Matrix.CreateTranslation(Vector3.UnitX * (FieldConstants.WIDTH - FieldConstants.LOWGOAL_WIDTH) * FieldConstants.C) * effect.World;
            lowGoal.Draw(graphicsDevice, effect);
            effect.World = Matrix.CreateTranslation(Vector3.UnitZ * ((FieldConstants.HEIGHT - FieldConstants.LOWGOAL_WIDTH)* FieldConstants.C - 1f)) * effect.World;
            lowGoal.Draw(graphicsDevice, effect);
            effect.World = oldWorld;

            truss.Draw(graphicsDevice, effect);
            trussVertical.Draw(graphicsDevice, effect);

            oldWorld = effect.World;
            effect.World = Matrix.CreateTranslation(Vector3.UnitX * (FieldConstants.WIDTH * 1.5f - FieldConstants.TRUSS_SQUARE_EDGE) * FieldConstants.C) * effect.World;
            trussVertical.Draw(graphicsDevice, effect);
            effect.World = oldWorld;
        }

    }
}
