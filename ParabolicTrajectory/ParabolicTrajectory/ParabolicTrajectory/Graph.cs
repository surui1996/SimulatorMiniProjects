using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ParabolicTrajectory
{
    public class Graph
    {
        BasicEffect basicEffect;
        short[] lineListIndices;

        public Graph(GraphicsDevice graphicsDevice)
        {
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            basicEffect.World = Matrix.Identity;

            basicEffect.VertexColorEnabled = true;
        }

        /// <summary>
        /// Draws the values in given order, in specified color
        /// </summary>
        /// <param name="values">Values to draw, in order from left to right</param>
        /// <param name="color">Color of the entire graph</param>
        public void Draw(List<Vector3> values, Color color)
        {
            if (values.Count <= 2)
                return;

            VertexPositionColor[] pointList = new VertexPositionColor[values.Count];
            for (int i = 0; i < values.Count; i++)
                pointList[i] = new VertexPositionColor(values[i], color);

            DrawLineList(pointList);

        }
        /// <summary>
        /// Draws a line, in specified color
        /// </summary>
        /// <param name="p1">the first point of the line</param>
        /// <param name="p2">the second point of the line</param>
        /// <param name="color">Color of the line</param>
        public void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            VertexPositionColor[] pointList = new VertexPositionColor[2];
            pointList[0] = new VertexPositionColor(new Vector3(p1, 0), color);
            pointList[1] = new VertexPositionColor(new Vector3(p2, 0), color);

            DrawLineList(pointList);
        }

        void DrawLineList(VertexPositionColor[] pointList)
        {
            int numOfLines = pointList.Length - 1;

            //indices updated only need to be updated when the number of points has changed
            if (lineListIndices == null || lineListIndices.Length != ((pointList.Length * 2) - 2))
            {
                lineListIndices = new short[numOfLines * 2];
                for (int i = 0; i < numOfLines; i++)
                {
                    lineListIndices[i * 2] = (short)(i);
                    lineListIndices[(i * 2) + 1] = (short)(i + 1);
                }
            }

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                basicEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    pointList,
                    0,
                    pointList.Length,
                    lineListIndices,
                    0,
                    numOfLines
                );
            }
        }
    }
}