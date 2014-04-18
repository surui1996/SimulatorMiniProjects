using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MiniMap.Animation3D
{
    class TexturedWall : Box3D
    {
        public Texture2D WallTexture;

        public TexturedWall(float x, float y, float z, Vector3 position, Texture2D texture)
            : base(x, y, z, position)
        {
            WallTexture = texture;
        }
    }
}
