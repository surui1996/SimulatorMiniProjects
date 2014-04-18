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
        TexturedWall wallRed, wallBlue, carpet3;
        //TexturedWall dynamicRightVision, staticRightVision;

        public Field(ContentManager content)
        {
            wallRed = new TexturedWall(FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
               new Vector3(FieldConstants.WIDTH / 2, FieldConstants.HEIGHT_ABOVE_CARPET / 2, 0),
               content.Load<Texture2D>("visionRed"));

            wallBlue = new TexturedWall(FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0,
                new Vector3(FieldConstants.WIDTH / 2, FieldConstants.HEIGHT_ABOVE_CARPET / 2, -FieldConstants.HEIGHT),
                content.Load<Texture2D>("visionBlue"));

            //dynamicRightVision = new TexturedWall(FieldConstants.DYNAMIC_WIDTH, FieldConstants.DYNAMIC_HEIGHT, 0.1f,
            //    new Vector3(-FieldConstants.WIDTH / 2, -FieldConstants.HEIGHT_ABOVE_CARPET / 2, 0) +
            //    new Vector3((FieldConstants.LOWGOAL_WIDTH + FieldConstants.LOWGOAL_WIDTH) / 2, FieldConstants.DYNAMIC_HEIGHT_ABOVE_CARPET + FieldConstants.DYNAMIC_HEIGHT, 0),
            //    content.Load<Texture2D>("visionTarget"));

            //staticRightVision = new TexturedWall(FieldConstants.STATIC_WIDTH, FieldConstants.STATIC_HEIGHT, 0.1f,
            //    new Vector3(-FieldConstants.WIDTH / 2, -FieldConstants.HEIGHT_ABOVE_CARPET / 2, 0) +
            //    new Vector3(FieldConstants.LOWGOAL_WIDTH + FieldConstants.STATIC_WIDTH + FieldConstants.STATIC_BLACK_STRIPES_WIDTH,
            //        FieldConstants.STATIC_HEIGHT_ABOVE_CARPET + FieldConstants.STATIC_HEIGHT, 0),
            //    content.Load<Texture2D>("visionTarget"));

            carpet3 = new TexturedWall(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT,
                new Vector3(FieldConstants.WIDTH / 2, -FieldConstants.HEIGHT_ABOVE_CARPET / 2, 0),
                content.Load<Texture2D>("carpet3D"));
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            effect.Texture = wallRed.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                wallRed.Draw(graphicsDevice);
            }

            effect.Texture = carpet3.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                carpet3.Draw(graphicsDevice);
            }

            effect.Texture = wallBlue.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                wallBlue.Draw(graphicsDevice);
            }

            //effect.Texture = dynamicRightVision.WallTexture;
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    dynamicRightVision.Draw(graphicsDevice);
            //    staticRightVision.Draw(graphicsDevice);
            //}
        }
    }
}
