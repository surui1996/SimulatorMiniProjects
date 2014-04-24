using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotSimulator
{
    class Box3
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

        public Box3(Texture2D texture, float x, float y, float z, Vector3 position, float scale)
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

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalFront = Vector3.Forward;// Normal((int)Plane.Front);
            Vector3 normalBack = Vector3.Backward; //Normal((int)Plane.Back);
            Vector3 normalTop = Vector3.Up;//Normal((int)Plane.Top);
            Vector3 normalBottom = Vector3.Down;//Normal((int)Plane.Bottom);
            Vector3 normalLeft = Vector3.Left;//Normal((int)Plane.Left);
            Vector3 normalRight = Vector3.Right;//Normal((int)Plane.Right);

            // UV texture coordinates
            Vector2 textureTopLeft = new Vector2(1.0f, 0.0f);
            Vector2 textureTopRight = new Vector2(0.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(1.0f, 1.0f);
            Vector2 textureBottomRight = new Vector2(0.0f, 1.0f);

            // Add the vertices for the FRONT face.
            vertices[0] = new VertexPositionNormalTexture(topLeftFront, normalFront, textureTopLeft);
            vertices[1] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
            vertices[2] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);
            vertices[3] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
            vertices[4] = new VertexPositionNormalTexture(btmRightFront, normalFront, textureBottomRight);
            vertices[5] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);

            // Add the vertices for the BACK face.
            vertices[6] = new VertexPositionNormalTexture(topLeftBack, normalBack, textureTopRight);
            vertices[7] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
            vertices[8] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
            vertices[9] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
            vertices[10] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
            vertices[11] = new VertexPositionNormalTexture(btmRightBack, normalBack, textureBottomLeft);

            // Add the vertices for the TOP face.
            vertices[12] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
            vertices[13] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);
            vertices[14] = new VertexPositionNormalTexture(topLeftBack, normalTop, textureTopLeft);
            vertices[15] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
            vertices[16] = new VertexPositionNormalTexture(topRightFront, normalTop, textureBottomRight);
            vertices[17] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);

            // Add the vertices for the BOTTOM face. 
            vertices[18] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureTopLeft);
            vertices[19] = new VertexPositionNormalTexture(btmLeftBack, normalBottom, textureBottomLeft);
            vertices[20] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureBottomRight);
            vertices[21] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureTopLeft);
            vertices[22] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureBottomRight);
            vertices[23] = new VertexPositionNormalTexture(btmRightFront, normalBottom, textureTopRight);

            // Add the vertices for the LEFT face.
            vertices[24] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureTopRight);
            vertices[25] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureBottomLeft);
            vertices[26] = new VertexPositionNormalTexture(btmLeftFront, normalLeft, textureBottomRight);
            vertices[27] = new VertexPositionNormalTexture(topLeftBack, normalLeft, textureTopLeft);
            vertices[28] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureBottomLeft);
            vertices[29] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureTopRight);

            // Add the vertices for the RIGHT face. 
            vertices[30] = new VertexPositionNormalTexture(topRightFront, normalRight, textureTopLeft);
            vertices[31] = new VertexPositionNormalTexture(btmRightFront, normalRight, textureBottomLeft);
            vertices[32] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureBottomRight);
            vertices[33] = new VertexPositionNormalTexture(topRightBack, normalRight, textureTopRight);
            vertices[34] = new VertexPositionNormalTexture(topRightFront, normalRight, textureTopLeft);
            vertices[35] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureBottomRight);
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
