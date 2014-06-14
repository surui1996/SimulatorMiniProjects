using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.Animation3D
{
    class Cylinder
    {

        private HollowCylinder hollowCylinder;
        private Circle circleFront;
        private Circle circleBack;

        public Cylinder(Texture2D textureCover, Texture2D textureBase, float radius, float height, Vector3 position)
        {
            hollowCylinder = new HollowCylinder(textureCover, radius, height, position);
            circleFront = new Circle(textureBase, radius, position, Vector3.Zero);
            circleBack = new Circle(textureBase, radius, position, new Vector3(0, 0, -height));
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, float angleY = 0)
        {
            hollowCylinder.Draw(device, effect, angleY);
            circleFront.Draw(device, effect, angleY);
            //maybe should rotate circleBack 180 (if there is cull-mode)
            circleBack.Draw(device, effect, angleY);
        }
    }
}
