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
        protected Vector2 initialPositionOnMap;
        protected Vector3 initialPositionOn3D;
        protected float mapMetersToPixel;

        protected Texture2D textureOnMap;

        public Vector3 Position { get { return RelativePosition + initialPositionOn3D / FieldConstants.PIXELS_IN_ONE_METER; } }
        public Vector3 RelativePosition { get; set; }
        public Vector3 Velocity { get; set; }

        public abstract void DrawOnMap(SpriteBatch spriteBatch);
        public abstract void Draw(GraphicsDevice device, BasicEffect effect, BasicEffect lighting = null);
    }
}
