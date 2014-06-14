using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.Animation3D
{
    class HollowCylinder : Drawable3D
    {
        private short[] sideStrip;

        private const int RESOLUTION = 10; // degrees between points
        private const int NUM_POINTS = 360 / RESOLUTION;

        public float Height { get; set; }

        public float Radius { get; set; }


        public HollowCylinder(Texture2D texture, float radius, float height, Vector3 initialPosition)
        {
            Texture = texture;
            Radius = radius;
            Height = height;
            Position = initialPosition;

            vertices = new VertexPositionNormalTexture[(NUM_POINTS) * 2];
            for (int a = 0, i = 0; a < 360.0f; a += RESOLUTION, i++)
            {
                float rad = MathHelper.ToRadians(a);
                float x = (float)Math.Cos(rad);
                float y = (float)Math.Sin(rad);

                vertices[i].Position = new Vector3(x, y, 0.0f);
                vertices[i].Normal = new Vector3(x, y, 0.0f);
                vertices[i].TextureCoordinate = new Vector2((float)i / (NUM_POINTS - 1), 1.0f);

                vertices[i + NUM_POINTS].Position = new Vector3(x, y, 1.0f);
                vertices[i + NUM_POINTS].Normal = new Vector3(x, y, 0.0f);
                vertices[i + NUM_POINTS].TextureCoordinate = new Vector2((float)i / (NUM_POINTS - 1), 0.0f);
            }

            sideStrip = new short[(NUM_POINTS + 1) * 2];
            for (short t = 0; t < NUM_POINTS; t++)
            {
                sideStrip[t * 2] = (short)(t + NUM_POINTS);
                sideStrip[t * 2 + 1] = t;
            }
            sideStrip[NUM_POINTS * 2] = (short)NUM_POINTS;
            sideStrip[NUM_POINTS * 2 + 1] = 0;


        }


        public override void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0)
        {
            Matrix oldWorld = effect.World;
            Texture2D oldTexture = effect.Texture;

            effect.World =Matrix.CreateScale(Radius, Radius, Height) * Matrix.CreateRotationY(MathHelper.PiOver2)
                * Matrix.CreateTranslation(Position) * Matrix.CreateRotationY(angleY) * effect.World;
            
            if (Texture != null)
            {
                effect.Texture = Texture;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip,
                    vertices, 0, NUM_POINTS * 2, sideStrip, 0, NUM_POINTS * 2);
            }

            effect.World = oldWorld;
            effect.Texture = oldTexture;
        }
    }
}
