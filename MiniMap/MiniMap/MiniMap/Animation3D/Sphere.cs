using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.Animation3D
{
    class Sphere : Drawable3D
    {
        // indices of the traingles, can't use traingle strip - becase there are fans involved
        List<short> sphereIndices = new List<short>();

        public float Radius { get; set; }

        public Quaternion Rotation = Quaternion.Identity;

        public Sphere(Texture2D texture, BoundingSphere sphere)
            : this(texture, sphere.Center, sphere.Radius, 40)
        { }

        /// <summary>
        /// Constructs a new sphere primitive,
        /// with the specified size and tessellation level.
        /// </summary>
        public Sphere(Texture2D texture, Vector3 position, float radius, int tessellation)
        {
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            Texture = texture;
            Position = position;
            Radius = radius;

            vertices = new VertexPositionNormalTexture[(verticalSegments - 1) * horizontalSegments + 2];

            // Start with a single vertex at the bottom of the sphere.
            vertices[0].Position = Vector3.Down * radius; vertices[0].Normal = Vector3.Down;
            vertices[0].TextureCoordinate = new Vector2(0, 0);

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                //-90 -> 90
                float latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

                float dy = (float)Math.Sin(latitude);
                float currentRadius = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    //0 -> 360
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * currentRadius;
                    float dz = (float)Math.Sin(longitude) * currentRadius;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    vertices[horizontalSegments * i + j + 1].Position = normal * radius;
                    vertices[horizontalSegments * i + j + 1].Normal = normal;

                    float textureX = (MathHelper.TwoPi - longitude) / MathHelper.TwoPi;
                    float textureY = 0.5f - (0.5f * latitude / MathHelper.PiOver2);
                    vertices[horizontalSegments * i + j + 1].TextureCoordinate =
                        new Vector2(textureX, textureY);
                }
            }

            vertices[(verticalSegments - 1) * horizontalSegments + 1].Position = Vector3.Up * radius;
            vertices[(verticalSegments - 1) * horizontalSegments + 1].Normal = Vector3.Up;
            vertices[(verticalSegments - 1) * horizontalSegments + 1].TextureCoordinate = new Vector2(0, 0);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                sphereIndices.Add(0);
                sphereIndices.Add((short)(1 + (i + 1) % horizontalSegments));
                sphereIndices.Add((short)(1 + i));
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    sphereIndices.Add((short)(1 + i * horizontalSegments + j));
                    sphereIndices.Add((short)(1 + i * horizontalSegments + nextJ));
                    sphereIndices.Add((short)(1 + nextI * horizontalSegments + j));

                    sphereIndices.Add((short)(1 + i * horizontalSegments + nextJ));
                    sphereIndices.Add((short)(1 + nextI * horizontalSegments + nextJ));
                    sphereIndices.Add((short)(1 + nextI * horizontalSegments + j));
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                sphereIndices.Add((short)((verticalSegments - 1) * horizontalSegments + 1));
                sphereIndices.Add((short)((verticalSegments - 1) * horizontalSegments
                    - (i + 1) % horizontalSegments));
                sphereIndices.Add((short)((verticalSegments - 1) * horizontalSegments - i));
            }
        }

        /// <summary>
        /// Draws this globe on the given device using the given effect.
        /// </summary>
        /// <param name="device">The device to draw on</param>
        /// <param name="effect">The effect to draw with</param>
        public override void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0)
        {
            Matrix oldWorld = effect.World;
            Texture2D oldTexture = effect.Texture;
            
            effect.World = Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position) * oldWorld;

            // draw the sphere
            if (Texture != null)
            {
                effect.Texture = Texture;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>
                    (PrimitiveType.TriangleList, vertices, 0, vertices.Length, sphereIndices.ToArray(),
                    0, sphereIndices.Count / 3);
            }

            // restore old effect state
            effect.Texture = oldTexture;
            effect.World = oldWorld;
        }
    }
}