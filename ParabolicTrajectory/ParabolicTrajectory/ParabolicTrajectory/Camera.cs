using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ParabolicTrajectory
{
    enum CameraArrow
    {
        Up, Down, Left, Right
    }
    public class Camera
    {
        private const float ZOOM = 0.01f;
        private const float TRANSLATE = 7f;

        public float Zoom { get; set; }
        public Vector2 Translation { get; set; }

        public Camera()
        {
            Zoom = 1f;
            Translation = Vector2.Zero;
        }

        public Matrix Transform()
        {
            return Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(Translation.X, Translation.Y, 0f);
        }

        public void ZoomIn()
        {
            Zoom += ZOOM;
        }

        public void ZoomOut()
        {
            Zoom -= ZOOM;
        }

        public void Translate(Vector2 v)
        {
            Translation += v;
        }

        public void Translate(Keys arrow)
        {
            switch (arrow)
            {
                case Keys.Up: Translation += new Vector2(0f, TRANSLATE);
                    break;
                case Keys.Down: Translation += new Vector2(0f, -TRANSLATE);
                    break;
                case Keys.Left: Translation += new Vector2(TRANSLATE, 0);
                    break;
                case Keys.Right: Translation += new Vector2(-TRANSLATE, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
