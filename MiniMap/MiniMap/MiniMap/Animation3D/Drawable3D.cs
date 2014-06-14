using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.Animation3D
{
    abstract class Drawable3D
    {
        //array of the primitive vertices
        protected VertexPositionNormalTexture[] vertices;

        //position of the 3D primitive on the field
        public Vector3 Position { get; set; }

        //texture that will cover the primitive
        public Texture2D Texture { get; set; }

        //method to draw the primitive, with an option to rotate it over the x-z plane
        public abstract void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0);
    }
}
