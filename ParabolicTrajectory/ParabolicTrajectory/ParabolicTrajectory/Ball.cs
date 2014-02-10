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
        private Vector2 maximumPoint;
        private Texture2D ball2D;
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
        public Vector2 MaximumPoint { get { return maximumPoint; } set { maximumPoint = value; } }
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

        private static List<Vector2> CalculateTrajectoryWithNoDrag(float vx, float vy, Vector2 initialPosition)
        {
            List<Vector2> myPositions = new List<Vector2>();
            Vector2 pos = Vector2.Zero;
            for (float t = 0; t < 100.0f; t += DT)
            {
                //x = vx* t, y = vy*t - 0.5*g*t^2
                pos = initialPosition + new Vector2(vx * t, -vy * t + 0.5f * kG * t * t) * BIGGER;
                myPositions.Add(pos);
            }
            return myPositions;
        }

        public void CalculateTrajectoryWithNoDrag()
        {
            positions = new List<Vector2>();
            Vector2 pos = Vector2.Zero;
            for (float t = 0; t < 100.0f; t += DT)
            {
                //x = vx* t, y = vy*t - 0.5*g*t^2
                pos = initialPosition + new Vector2(GetVX(initialVelocityMagnitude, InitialAngle) * t,
                    -GetVY(initialVelocityMagnitude, InitialAngle) * t + 0.5f * kG * t * t) * BIGGER;
                this.positions.Add(pos);
            }
            CalculateMaximumPoint();
        }

        public void CalculateTrajectoryWithNoDrag2()
        {
            positions = new List<Vector2>();
            Vector2 currentVelocity = new Vector2(GetVX(initialVelocityMagnitude, InitialAngle), -GetVY(initialVelocityMagnitude, InitialAngle));
            Vector2 currentPosition = initialPosition;
            for (float t = 0; t < 100.0f; t += DT)
            {
                //vx(t) = vx, vy(t) = vy - g * t
                currentVelocity.Y += kG * DT;
                currentPosition += currentVelocity * DT * BIGGER; //we assume speed is constant over t=DT seconds
                this.positions.Add(currentPosition);
            }
            CalculateMaximumPoint();
        }

        public void CalculateTrajectoryWithDrag()
        {
            positions = new List<Vector2>();
            float dragForceMagnitude = 0.0f;
            Vector2 dragAcceleration = Vector2.Zero;
            Vector2 currentVelocity = new Vector2(GetVX(initialVelocityMagnitude, InitialAngle), -GetVY(initialVelocityMagnitude, InitialAngle));

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
            CalculateMaximumPoint();
        }

        private void CalculateMaximumPoint()
        {
            Vector2 maxPoint = positions[0];
            for (int i = 1; i < positions.Count; i++)
            {
                if (positions[i].Y < maxPoint.Y) // y axis is opposite
                    maxPoint = positions[i];
            }
            maximumPoint = maxPoint;
        }

        public static Vector2 CalculateMaximumPoint(List<Vector2> positions)
        {
            Vector2 maxPoint = positions[0];
            for (int i = 1; i < positions.Count; i++)
            {
                if (positions[i].Y < maxPoint.Y) // y axis is opposite
                    maxPoint = positions[i];
            }
            return maxPoint;
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

        private static float GetVX(float velocity, float angle)
        {
            return velocity * (float)Math.Cos(MathHelper.ToRadians(angle));
        }

        private static float GetVY(float velocity, float angle)
        {
            return velocity * (float)Math.Sin(MathHelper.ToRadians(angle));
        }

        public static List<Vector3> GetSimpleTrajectory(Vector2 maximumPoint, float maximumVelocityMagnitude, float initialAngle, Vector2 initialPosition)
        {
            float minimalDistance = Math.Max(maximumPoint.X, maximumPoint.Y);
            float currentDistance;
            List<Vector2> currentPositions, bestPositions = new List<Vector2>();
            Vector2 currentPos;

            for (float v = maximumVelocityMagnitude; v > 0.2f; v -= 0.1f)
            {
                currentPositions = new List<Vector2>(CalculateTrajectoryWithNoDrag(GetVX(v, initialAngle), GetVY(v, initialAngle), initialPosition));
                currentPos = CalculateMaximumPoint(currentPositions);
                currentDistance = Distance(currentPos, maximumPoint);
                if (currentDistance < minimalDistance)
                {
                    minimalDistance = currentDistance;
                    bestPositions = currentPositions;
                }
                
            }
            return To3D(bestPositions);            
        }
        //only with Y
        public static List<Vector3> GetSimpleTrajectory2(Vector2 maximumPoint, float maximumVelocityMagnitude, float initialAngle, Vector2 initialPosition)
        {
            float minimalDistance = maximumPoint.Y;
            float currentDistance;
            List<Vector2> currentPositions, bestPositions = new List<Vector2>();
            float currentMax;

            for (float v = maximumVelocityMagnitude; v > 0.2f; v -= 0.1f)
            {
                currentPositions = new List<Vector2>(CalculateTrajectoryWithNoDrag(GetVX(v, initialAngle),
                    GetVY(v, initialAngle), initialPosition));
                currentMax = CalculateMaximumPoint(currentPositions).Y;
                currentDistance = Math.Abs(currentMax - maximumPoint.Y);//Distance(currentPos, maximumPoint);
                if (currentDistance < minimalDistance)
                {
                    minimalDistance = currentDistance;
                    bestPositions = currentPositions;
                }

            }
            return To3D(bestPositions);
        }

        private static float Distance(Vector2 p1, Vector2 p2)
        {
            return (float)(Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2)));
        }

        public List<Vector3> Get3DPositions()
        {
            return To3D(positions);
        }

        private static List<Vector3> To3D(List<Vector2> vectorList)
        {
            List<Vector3> position3 = new List<Vector3>();
            foreach (Vector2 v in vectorList)
                position3.Add(new Vector3(v, 0f));
            return position3;
        }

        public static List<Vector2> To2D(List<Vector3> vectorList)
        {
            List<Vector2> position2 = new List<Vector2>();
            foreach (Vector3 v in vectorList)
                position2.Add(new Vector2(v.X, v.Y));
            return position2;
        }
    }
}

