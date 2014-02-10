using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParabolicTrajectory
{
    // meters, kilograms, seconds
    class Ball
    {
        #region Class members
        private float initialVelocityMagnitude, initialAngle;
        private Vector2 initialPosition;
        private List<Vector2> positions;
        Texture2D ball2D;
        #endregion
        
        #region Constants and Formula Constants
        Color DEFAULT_COLOR = Color.CornflowerBlue;
        const float SCALE = 0.1f;
        const float kG = 9.8f, DT = 0.005f;        
        private float dragCoefficient;
        private float AIR_DENSITY = 1.21f, BALL_RADIUS = 0.305f, BALL_MASS = 1.0f;
        #endregion

        #region Properties
        public float InitialVelocityMagnitude { get { return initialVelocityMagnitude; } set { initialVelocityMagnitude = value; } }
        public float InitialAngle { get { return initialAngle; } set { initialAngle = value; } }
        public List<Vector2> Positions { get { return positions; } set { positions = value; } }
        #endregion
        
        public Ball(Vector2 position, float velocity, float angle, Texture2D texture)
        {
            this.initialPosition = position;
            this.initialVelocityMagnitude = velocity;
            this.initialAngle = angle;

            this.positions = new List<Vector2>();
            this.ball2D = texture;
        }

        public Ball(Vector2 position, Vector2 velocity, Texture2D texture)
        {
            this.initialPosition = position;
            this.initialVelocityMagnitude = velocity.Length();
            this.initialAngle = (float)Math.Tan(velocity.Y / velocity.X);

            this.positions = new List<Vector2>();

            this.ball2D = texture;
        }

        public void CalculateTrajectoryWithNoDrag()
        {
            positions = new List<Vector2>();
            Vector2 pos = Vector2.Zero;
            for (float t = 0; t < 100.0f; t += DT)
            {
                pos = initialPosition + new Vector2(GetVX() * t, -GetVY() * t + kG * t * t);
                this.positions.Add(pos);
            }
        }

        private bool HasBallReachedGround(int positionIndex)
        {
            return positionIndex > 10 && Math.Abs(positions[positionIndex].Y - initialPosition.Y) < 1f;
        }

        public bool DrawBall(int positionIndex, SpriteBatch sb)
        {
            sb.Draw(ball2D, new Vector2(positions[positionIndex].X, positions[positionIndex].Y), null, DEFAULT_COLOR, 0f,
                    new Vector2(51f, 51f), SCALE, SpriteEffects.None, 0);
            if (HasBallReachedGround(positionIndex))
                return true;
            return false;
        }

        private float GetVX()
        {
            return initialVelocityMagnitude * (float)Math.Cos(MathHelper.ToRadians(initialAngle));
        }

        private float GetVY()
        {
            return initialVelocityMagnitude * (float)Math.Sin(MathHelper.ToRadians(initialAngle));
        }

        public List<Vector3> Get3DPositions()
        {
            List<Vector3> position3 = new List<Vector3>();
            foreach (Vector2 v in positions)
                position3.Add(new Vector3(v, 0f));
            return position3;
        }
    }
}
