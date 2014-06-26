using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator.PhysicalModeling
{
    abstract class DynamicObject
    {
        protected Vector3 initialPositionOn3D;
        protected float mapMetersToPixel;

        protected Texture2D textureOnMap;

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        public abstract void Reset();
        public abstract void DrawOnMap(SpriteBatch spriteBatch);
        public abstract void Draw(GraphicsDevice device, BasicEffect effect, BasicEffect lighting = null);
    }
}
