using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simulator.PhysicalModeling;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simulator.GUI
{
    class BallMenuEntry
    {
        private GameBall ball;
        private float yaw = 0;

        public Texture2D DefaultTexture { get; set; }
        public Texture2D SelectedTexture { get; set; }

        public bool Selected { get; set; }

        public BallMenuEntry(Vector3 CenterPosition, Texture2D defaultTexture, Texture2D selectedTexture)
        {
            DefaultTexture = defaultTexture;
            SelectedTexture = selectedTexture;

            ball = new GameBall(CenterPosition, 0, null, defaultTexture);

        }

        public void Select()
        {
            Selected = true;
            ball.ChangeSphereTexture(SelectedTexture);
        }

        public void UnSelect()
        {
            Selected = false;
            ball.ChangeSphereTexture(DefaultTexture);
        }

        public void Update(float dt)
        {

            if (Selected || yaw % MathHelper.Pi > 0.05)
            {
                yaw += dt * MathHelper.PiOver2;
            }
            ball.Rotate(yaw, 0, 0);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            ball.Draw(device, effect);
        }
    }
}
