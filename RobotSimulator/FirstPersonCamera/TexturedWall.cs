using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotSimulator
{
    class TexturedWall : Box3
    {
        

        public Texture2D WallTexture;

        public TexturedWall(float x, float y, float z, Vector3 position, Texture2D texture)
            : base(x, y, z, position)
        {
            WallTexture = texture;
        }
    }
}
