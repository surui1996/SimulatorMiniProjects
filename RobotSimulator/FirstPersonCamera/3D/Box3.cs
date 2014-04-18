using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotSimulator
{
    enum Plane
    {
        Top = 0, Bottom = 1, Right = 2, Left = 3, Front = 4, Back = 5
    }
    class Box3
    {

        protected static int NUM_TRIANGLES = 12;
        protected static int NUM_VERTICES = 36;

        // Array of vertex information - contains position, normal and texture data
        protected VertexPositionNormalTexture[] vertices;
        protected bool isConstructed;

        protected Vector3 topLeftFront, topLeftBack, topRightFront, topRightBack;
        protected Vector3 btmLeftFront, btmLeftBack, btmRightFront, btmRightBack;

        public Vector3 Position {get; set;}


        public Box3(float x, float y, float z, Vector3 position)
        {
            x = -x * FieldConstants.C;
            y = -y * FieldConstants.C;
            z = -z * FieldConstants.C;
            position = position * FieldConstants.C;
            //position -= new Vector3((int)((float)x / 2), (int)((float)y / 2), 0); 

            topLeftFront = position;
            topLeftBack = new Vector3(0, 0, z) + position;
            topRightFront = new Vector3(x, 0, 0) + position;
            topRightBack = new Vector3(x, 0, z) + position;
            btmLeftFront = new Vector3(0, y, 0) + position;
            btmLeftBack = new Vector3(0, y, z) + position;
            btmRightFront = new Vector3(x, y, 0) + position;
            btmRightBack = new Vector3(x, y, z) + position;
            Position = position;
        }

        public Box3(Vector3[] vertices, Vector3 position)
        {
            topLeftFront = vertices[0] + position;
            topLeftBack = vertices[1] + position;
            topRightFront = vertices[2] + position;
            topRightBack = vertices[3] + position;
            btmLeftFront = vertices[4] + position;
            btmLeftBack = vertices[5] + position;
            btmRightFront = vertices[6] + position;
            btmRightBack = vertices[7] + position;
            Position = position;
        }

        public static Vector3 Normal(int plane)
        {
            // Normal vectors for each face (needed for lighting / display)
            switch (plane)
	        {
		        case 0://Planes.Top:
                    return new Vector3(0.0f, 1.0f, 0.0f);
                case 1://Planes.Bottom:
                    return new Vector3(0.0f, -1.0f, 0.0f);
                case 2://Planes.Right:
                    return new Vector3(1.0f, 0.0f, 0.0f);
                case 3://Planes.Left:
                    return new Vector3(-1.0f, 0.0f, 0.0f);
                case 4://Planes.Front:
                    return new Vector3(0.0f, 0.0f, 1.0f);
                case 5://Planes.Back:
                    return new Vector3(0.0f, 0.0f, -1.0f);
	        }
            return Vector3.Zero;
        }

        public void Draw(GraphicsDevice device)
        {
            // Build the cube, setting up the _vertices array
            if (isConstructed == false)
                Construct();

            // Create the shape buffer and dispose of it to prevent out of memory
            using (VertexBuffer buffer = new VertexBuffer(
                device,
                VertexPositionNormalTexture.VertexDeclaration,
                vertices.Length,
                BufferUsage.WriteOnly))
            {
                // Load the buffer
                buffer.SetData(vertices);
                
                // Send the vertex buffer to the device
                device.SetVertexBuffer(buffer);
            }

            device.DrawPrimitives(PrimitiveType.TriangleList, 0, NUM_TRIANGLES);
        }   

        protected virtual void Construct()
        {
            NUM_TRIANGLES = 12;
            NUM_VERTICES = 36;
            vertices = new VertexPositionNormalTexture[NUM_VERTICES];

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalFront = Normal((int)Plane.Front);
            Vector3 normalBack = Normal((int)Plane.Back);
            Vector3 normalTop = Normal((int)Plane.Top);
            Vector3 normalBottom = Normal((int)Plane.Bottom);
            Vector3 normalLeft = Normal((int)Plane.Left);
            Vector3 normalRight = Normal((int)Plane.Right);

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

            isConstructed = true;
        }
        
    }
}
