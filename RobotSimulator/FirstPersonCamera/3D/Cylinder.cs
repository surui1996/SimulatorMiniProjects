using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RobotSimulator._3D
{
    class Cylinder
    {
        public Texture2D TextureCover { get; set; }
        public Texture2D TextureBase { get; set; }

        private VertexPositionNormalTexture[] sides;
        private short[] sideStrip;
        private VertexPositionNormalTexture[] bases;
        private short[] baseStrip;

        private const int RESOLUTION = 10; // degrees between points
        private const int NUM_POINTS = 360 / RESOLUTION;

        private float height;
        public float Height
        {
            get { return height; }
            set { if (value <= 0.0) throw new ArgumentOutOfRangeException(); height = value; }
        }

        private float radius;
        public float Radius
        {
            get { return radius; }
            set { if (value <= 0.0) throw new ArgumentOutOfRangeException(); radius = value; }
        }

        public Vector3 Position { get; set; }

        public Cylinder(Texture2D textureCover, float radius, float height, Vector3 position)
        {
            TextureCover = textureCover;
            Radius = radius;
            Height = height;
            Position = position;

            sides = new VertexPositionNormalTexture[(NUM_POINTS) * 2];
            for (int a = 0, i = 0; a < 360.0f; a += RESOLUTION, i++)
            {
                float rad = MathHelper.ToRadians(a);
                float x = (float)Math.Cos(rad);
                float y = (float)Math.Sin(rad);

                sides[i].Position = new Vector3(x, y, 0.0f);
                sides[i].Normal = new Vector3(x, y, 0.0f);
                sides[i].TextureCoordinate = new Vector2((float)i / (NUM_POINTS - 1), 1.0f);

                sides[i + NUM_POINTS].Position = new Vector3(x, y, 1.0f);
                sides[i + NUM_POINTS].Normal = new Vector3(x, y, 0.0f);
                sides[i + NUM_POINTS].TextureCoordinate = new Vector2((float)i / (NUM_POINTS - 1), 0.0f);
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


        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            Matrix oldWorld = effect.World;
            Texture2D oldTexture = effect.Texture;

            effect.World = Matrix.CreateScale(radius, radius, height) * Matrix.CreateTranslation(Position)
                * effect.World;
            
            if (TextureCover != null)
            {
                effect.Texture = TextureCover;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip,
                    sides, 0, NUM_POINTS * 2, sideStrip, 0, NUM_POINTS * 2);
            }

            //if (TextureBase != null)
            //{
            //    effect.Texture = TextureBase;
            //}
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip,
            //        bases, 0, NUM_POINTS * 2, baseStrip, 0, NUM_POINTS * 2);
            //}

            effect.World = oldWorld;
            effect.Texture = oldTexture;
        }
    }
}
