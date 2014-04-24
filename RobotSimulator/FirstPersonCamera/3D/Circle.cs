using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotSimulator._3D
{
    class Circle
    {
        public Texture2D Texture { get; set; }

        private VertexPositionNormalTexture[] vertices;
        private short[] indices;

        private const int RESOLUTION = 10; // degrees between points
        private const int NUM_POINTS = 360 / RESOLUTION;

        public float Radius { get; set; }
        public Vector3 AbsolutePosition { get; set; }
        public Vector3 RelativePosition { get; set; }

        public Circle(Texture2D texture, float radius, Vector3 absolutePosition,
            Vector3 relativePosition)
        {
            Texture = texture;
            Radius = radius;
            AbsolutePosition = absolutePosition;
            RelativePosition = relativePosition;

            vertices = new VertexPositionNormalTexture[NUM_POINTS + 1];
            vertices[0].Position = new Vector3(0.0f, 0.0f, 0.0f);
            vertices[0].Normal = Vector3.Forward; ;//new Vector3(x, y, 0.0f);
            vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);
            for (int a = 0, i = 1; a < 360.0f; a += RESOLUTION, i++)
            {
                float rad = MathHelper.ToRadians(a);
                float x = (float)Math.Cos(rad);
                float y = (float)Math.Sin(rad);

                vertices[i].Position = new Vector3(x, y, 0.0f);
                vertices[i].Normal = Vector3.Forward;//new Vector3(x, y, 0.0f);
                vertices[i].TextureCoordinate = new Vector2(0.5f + 0.5f * x, 0.5f - 0.5f * y);
            }

            indices = new short[NUM_POINTS * 3];
            for (short t = 0; t < NUM_POINTS; t++)
            {
                indices[t * 3] = 0;
                indices[t * 3 + 1] = (short)(t + 1);
                indices[t * 3 + 2] = (short)(((t + 1) % NUM_POINTS) + 1);
            }

        }


        public void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0)
        {
            Matrix oldWorld = effect.World;
            Texture2D oldTexture = effect.Texture;

            //maybe add the rotation as param
            effect.World = Matrix.CreateScale(Radius, Radius, 0) * Matrix.CreateTranslation(RelativePosition)
                * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(AbsolutePosition)
                * Matrix.CreateRotationY(angleY) * effect.World;
            
            if (Texture != null)
            {
                effect.Texture = Texture;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertices, 0, NUM_POINTS + 1, indices, 0, NUM_POINTS);
            }

            effect.World = oldWorld;
            effect.Texture = oldTexture;
        }


        
    }
}
