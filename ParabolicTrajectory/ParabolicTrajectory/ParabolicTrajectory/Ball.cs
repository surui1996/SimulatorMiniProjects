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
        const float BIGGER = 85f;
        const float kG = 9.8f, DT = 0.005f;
        private float dragCoefficient = 0.47f;
            
            
        private float AIR_DENSITY = 1.2f, BALL_RADIUS = 0.305f, BALL_MASS = 1.0f;
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
                //x = vx* t, y = vy*t - 0.5*g*t^2
                pos = initialPosition + new Vector2(GetVX() * t, -GetVY() * t + 0.5f * kG * t * t) * BIGGER;
                this.positions.Add(pos);
            }
        }

        public void CalculateTrajectoryWithNoDrag2()
        {
            positions = new List<Vector2>();
            Vector2 currentVelocity = new Vector2(GetVX(), -GetVY());
            Vector2 currentPosition = initialPosition;
            for (float t = 0; t < 100.0f; t += DT)
            {
                //vx(t) = vx, vy(t) = vy - g * t
                currentVelocity.Y += kG * DT;
                currentPosition += currentVelocity * DT * BIGGER; //we assume speed is constant over t=DT seconds
                this.positions.Add(currentPosition);
            }
        }

        public void CalculateTrajectoryWithDrag()
        {
            positions = new List<Vector2>();
            float dragForceMagnitude = 0.0f;
            Vector2 dragAcceleration = Vector2.Zero;
            Vector2 currentVelocity = new Vector2(GetVX(), -GetVY());

            Vector2 currentPosition = initialPosition;
            for (float t = 0; t < 100.0f; t += DT)
            {
                //Fdrag = 0.5*Cdrag*Density*cross-sectionalArea*v^2, Adrag = fDrag / ballMass
                dragForceMagnitude = (float)(0.5 * dragCoefficient * AIR_DENSITY * Math.PI * Math.Pow(BALL_RADIUS, 2)
                    * currentVelocity.LengthSquared());
                dragAcceleration = -(NormalizeVector(currentVelocity) * dragForceMagnitude) / BALL_MASS; // F = ma

                currentVelocity.X += dragAcceleration.X * DT;
                currentVelocity.Y += (kG + dragAcceleration.Y) * DT;
                currentPosition += currentVelocity * DT * BIGGER;
                this.positions.Add(currentPosition);
            }
        }


        private Vector2 NormalizeVector(Vector2 v)
        {
            Vector2 ret = new Vector2(v.X, v.Y);
            ret.Normalize();
            return ret;
        }

        public bool DrawBall(int positionIndex, SpriteBatch sb)
        {
            if (positionIndex < 0 || positions.Count == 0)
                return false;
            sb.Draw(ball2D, new Vector2(positions[positionIndex].X, positions[positionIndex].Y), null, DEFAULT_COLOR, 0f,
                    new Vector2(51f, 51f), SCALE, SpriteEffects.None, 0);
            if (HasBallReachedGround(positionIndex))
                return true;
            return false;
        }

        private bool HasBallReachedGround(int positionIndex)
        {
            return positionIndex > 10 && Math.Abs(positions[positionIndex].Y - initialPosition.Y) < 1f;
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
