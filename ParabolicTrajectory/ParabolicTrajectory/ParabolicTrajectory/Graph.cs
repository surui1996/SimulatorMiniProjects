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

        /// <summary>
        /// The bottom left position of the graph
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The size of the graph.
        /// The graph values will be scaled horizontally to fill width (Size.X)
        /// Vertically, the values will be scaled based on MaxValue property, where the position of the value that is equal to MaxValue will be Size.Y
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        /// Determines the vertical scaling of the graph.
        /// The value that is equal to MaxValue will be displayed at the top of the graph (at point Size.Y)
        /// </summary>
        public float MaxValue { get; set; }

        private Vector2 _scale = new Vector2(1.0f, 1.0f);

        BasicEffect _effect;
        short[] lineListIndices;

        public Graph(GraphicsDevice graphicsDevice)
        {
            _effect = new BasicEffect(graphicsDevice);
            _effect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            _effect.World = Matrix.Identity;

            _effect.VertexColorEnabled = true;
        }

        //public Graph(GraphicsDevice graphicsDevice, Point size)
        //{
        //    _effect = new BasicEffect(graphicsDevice);
        //    _effect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
        //    _effect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
        //    _effect.World = Matrix.Identity;

        //    _effect.VertexColorEnabled = true;

        //    this.MaxValue = 1;
        //    this.Size = size;
        //    if (size.Y <= 0)
        //        Size = new Point(size.X, 1);
        //    if (size.X <= 0)
        //        Size = new Point(1, size.Y);
        //}

        void UpdateWorld()
        {
            _effect.World = Matrix.CreateScale(_scale.X, _scale.Y, 1.0f)
                          * Matrix.CreateRotationX(MathHelper.Pi) //flips the graph so that the higher values are above. Makes bottom left the graph origin.
                          * Matrix.CreateTranslation(new Vector3(this.Position, 0));
        }

        /// <summary>
        /// Draws the values in given order, in specified color
        /// </summary>
        /// <param name="values">Values to draw, in order from left to right</param>
        /// <param name="color">Color of the entire graph</param>
        public void Draw(List<Vector3> values, Color color)
        {
            if (values.Count < 2)
                return;

            //float xScale = this.Size.X / (float)values.Count;
            //float yScale = this.Size.Y / MaxValue;

            //_scale = new Vector2(xScale, yScale);
            //UpdateWorld();

            VertexPositionColor[] pointList = new VertexPositionColor[values.Count];
            for (int i = 0; i < values.Count; i++)
                pointList[i] = new VertexPositionColor(values[i], color);

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

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _effect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
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
