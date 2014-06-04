using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMap.Animation3D
{
    class Box
    {

        protected static int NUM_TRIANGLES = 12;
        protected static int NUM_VERTICES = 36;

        // Array of vertex information - contains position, normal and texture data
        protected VertexPositionNormalTexture[] vertices;

        private Vector3 topLeftFront, topLeftBack, topRightFront, topRightBack;
        private Vector3 btmLeftFront, btmLeftBack, btmRightFront, btmRightBack;

        public Vector3 Position { get; set; }
        public Texture2D Texture { get; set; }

        private float x, y, z;

        public Box(Texture2D texture, float x, float y, float z, Vector3 position, float scale)
        {
            Texture = texture;
            
            Position = position * scale;
            x = x * scale;
            y = y * scale;
            z = z * scale;

            this.x = x; this.y = y; this.z = z;

            //somewhat strange axis system

            btmRightFront = new Vector3(0, 0, z);
            btmRightBack = new Vector3(0, 0, 0);
            btmLeftFront = new Vector3(x, 0, z);
            btmLeftBack = new Vector3(x, 0, 0);

            topRightFront = new Vector3(0, y, z);
            topRightBack = new Vector3(0, y, 0);
            topLeftFront = new Vector3(x, y, z);
            topLeftBack = new Vector3(x, y, 0);
            
            Construct();
        }

        protected virtual void Construct()
        {
            NUM_TRIANGLES = 12;
            NUM_VERTICES = 36;
            vertices = new VertexPositionNormalTexture[NUM_VERTICES];

            // I'm using left-handed coordinate system so the directions are opposite
            Vector3 front = Vector3.Backward;
            Vector3 back = Vector3.Forward; 
            Vector3 top = Vector3.Up;
            Vector3 bottom = Vector3.Down;
            Vector3 left = Vector3.Right;
            Vector3 right = Vector3.Left;

            // UV texture coordinates
            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(1.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 bottomRight = new Vector2(1.0f, 1.0f);


            // Add the vertices for the FRONT face.
            vertices[0] = new VertexPositionNormalTexture(topRightFront, front, topLeft);
            vertices[1] = new VertexPositionNormalTexture(btmRightFront, front, bottomLeft);
            vertices[2] = new VertexPositionNormalTexture(topLeftFront, front, topRight);
            vertices[3] = new VertexPositionNormalTexture(topLeftFront, front, topRight);
            vertices[4] = new VertexPositionNormalTexture(btmRightFront, front, bottomLeft);
            vertices[5] = new VertexPositionNormalTexture(btmLeftFront, front, bottomRight);


            // Add the vertices for the BACK face.
            vertices[6] = new VertexPositionNormalTexture(topLeftBack, back, topLeft);
            vertices[7] = new VertexPositionNormalTexture(btmLeftBack, back, bottomLeft);
            vertices[8] = new VertexPositionNormalTexture(topRightBack, back, topRight);
            vertices[9] = new VertexPositionNormalTexture(topRightBack, back, topRight);
            vertices[10] = new VertexPositionNormalTexture(btmLeftBack, back, bottomLeft);
            vertices[11] = new VertexPositionNormalTexture(btmRightBack, back, bottomRight);

            // Add the vertices for the TOP face.
            vertices[12] = new VertexPositionNormalTexture(topLeftFront, top, topLeft);
            vertices[13] = new VertexPositionNormalTexture(topLeftBack, top, bottomLeft);
            vertices[14] = new VertexPositionNormalTexture(topRightFront, top, topRight);
            vertices[15] = new VertexPositionNormalTexture(topRightFront, top, topRight);
            vertices[16] = new VertexPositionNormalTexture(topLeftBack, top, bottomLeft);
            vertices[17] = new VertexPositionNormalTexture(topRightBack, top, bottomRight);

            // Add the vertices for the BOTTOM face. 
            vertices[18] = new VertexPositionNormalTexture(btmLeftBack, bottom, topLeft);
            vertices[19] = new VertexPositionNormalTexture(btmLeftFront, bottom, bottomLeft);
            vertices[20] = new VertexPositionNormalTexture(btmRightBack, bottom, topRight);
            vertices[21] = new VertexPositionNormalTexture(btmRightBack, bottom, topRight);
            vertices[22] = new VertexPositionNormalTexture(btmLeftFront, bottom, bottomLeft);
            vertices[23] = new VertexPositionNormalTexture(btmRightFront, bottom, bottomRight);

            // Add the vertices for the LEFT face.
            vertices[24] = new VertexPositionNormalTexture(topLeftFront, left, topLeft);
            vertices[25] = new VertexPositionNormalTexture(btmLeftFront, left, bottomLeft);
            vertices[26] = new VertexPositionNormalTexture(topLeftBack, left, topRight);
            vertices[27] = new VertexPositionNormalTexture(topLeftBack, left, topRight);
            vertices[28] = new VertexPositionNormalTexture(btmLeftFront, left, bottomLeft);
            vertices[29] = new VertexPositionNormalTexture(btmLeftBack, left, bottomRight);

            // Add the vertices for the RIGHT face. 
            vertices[30] = new VertexPositionNormalTexture(topRightBack, right, topLeft);
            vertices[31] = new VertexPositionNormalTexture(btmRightBack, right, bottomLeft);
            vertices[32] = new VertexPositionNormalTexture(topRightFront, right, topRight);
            vertices[33] = new VertexPositionNormalTexture(topRightFront, right, topRight);
            vertices[34] = new VertexPositionNormalTexture(btmRightBack, right, bottomLeft);
            vertices[35] = new VertexPositionNormalTexture(btmRightFront, right, bottomRight);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0)
        {
            Matrix oldWorld = effect.World;
            Texture2D oldTexture = effect.Texture;

            effect.World = Matrix.CreateTranslation(-0.5f * topLeftFront) * Matrix.CreateRotationY(angleY) *
                Matrix.CreateTranslation(Position) * Matrix.CreateTranslation(0.5f * topLeftFront)
                * effect.World;

            if (Texture != null)
            {
                effect.Texture = Texture;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertices, 0, NUM_TRIANGLES);
            }

            effect.World = oldWorld;
            effect.Texture = oldTexture;
        }   


        
    }
}
