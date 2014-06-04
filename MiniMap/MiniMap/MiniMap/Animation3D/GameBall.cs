using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMap.Animation3D
{
    class GameBall
    {

        private const float BALL_RADIUS = 0.3f; //meters 
        private const float BALL_MASS = 2f; //kg
        private const float GRAVITY_ACCELERATION = 9.8f;
        private const float DRAG_COEFF = 0.47f;
        private const float AIR_DENSITY = 1.2f;
        private const float SHOOTING_VELOCITY = 10f; //m/s

        public Vector3 Position { get { return RelativePosition + initialPositionOn3D / FieldConstants.PIXELS_IN_ONE_METER; } }
        public Vector3 RelativePosition { get; set; }
        public Vector3 Velocity { get; set; }
        public bool IsScored { get; set; }

        private Vector2 initialPositionOnMap;
        private Vector3 initialPositionOn3D;
        private float mapMetersToPixel;

        private bool ballPossessed = false, ballInAir = false, disappear = false;

        private Texture2D ballTexture2D;
        private Sphere sphere;

        public GameBall(Vector3 position3D, Vector2 mapPosition, float mapMetersToPixel,
            Texture2D ball2D, Texture2D ballCover)
        {
            this.initialPositionOn3D = position3D;
            this.initialPositionOnMap = mapPosition;
            this.mapMetersToPixel = mapMetersToPixel;
            this.ballTexture2D = ball2D;

            this.RelativePosition = Vector3.Zero;
            this.Velocity = Vector3.Zero;
            this.IsScored = false;

            this.sphere = new Sphere(ballCover, new Vector3(0, BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER, 0),
                BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER, 40);
        }

        public void Restore()
        {
            this.RelativePosition = Vector3.Zero;
            this.Velocity = Vector3.Zero;
            this.IsScored = false;
            ballPossessed = false; ballInAir = false; disappear = false;
        }

        public void Update(float dt, BoundingSphere robotBox, Vector3 robotVelocity)
        {
            if (ballPossessed)
            {
                RelativePosition = (robotBox.Center - initialPositionOn3D) / FieldConstants.PIXELS_IN_ONE_METER;
                Velocity = Vector3.Zero;
                return;
            }

            bool robotIntersection = false, wallIntersection = false;
            BoundingSphere boundingSphere = new BoundingSphere(initialPositionOn3D + RelativePosition * FieldConstants.PIXELS_IN_ONE_METER,
                    BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER);

            if (ballInAir)
            {
                //Fdrag = 0.5*Cdrag*Density*cross-sectionalArea*v^2, Adrag = fDrag / ballMass
                float dragForceMagnitude = (float)(0.5 * DRAG_COEFF * AIR_DENSITY * Math.PI * Math.Pow(BALL_RADIUS, 2)
                        * Velocity.LengthSquared());
                Vector3 dragAcceleration = -(Vector3.Normalize(Velocity) * dragForceMagnitude) / BALL_MASS; // F = ma

                Velocity = new Vector3(
                    Velocity.X + dragAcceleration.X * dt,
                    Velocity.Y + (-GRAVITY_ACCELERATION + dragAcceleration.Y) * dt,
                    Velocity.Z + dragAcceleration.Z * dt);
            }
            else
            {
                if (boundingSphere.Intersects(robotBox) && Velocity.Length() <= robotVelocity.Length())
                {
                    Velocity += robotVelocity / 4;
                    robotIntersection = true;
                }
                else
                {
                    if (Velocity.Length() < 0.03f)
                        Velocity = Vector3.Zero;
                    else
                        Velocity -= Vector3.Normalize(Velocity) * dt * 1f; //-0.2 m/s^2 acceleration
                }
            }

            Vector3 ballFront = boundingSphere.Center + Vector3.Normalize(Velocity) * boundingSphere.Radius;

            if (ballFront.X < 0 || ballFront.X > FieldConstants.C * FieldConstants.WIDTH)
            {
                Velocity -= new Vector3(1.8f * Velocity.X, Velocity.Y / 10, Velocity.Z / 10);
                wallIntersection = true;
            }
            else if (ballFront.Z < 0 || ballFront.Z > FieldConstants.C * FieldConstants.HEIGHT)
            {
                wallIntersection = true;

                if (ballFront.Y > FieldConstants.C * FieldConstants.HEIGHT_ABOVE_CARPET ||
                    ballFront.Y < FieldConstants.C * FieldConstants.HIGH_GOAL_BOTTOM_ABOVE_CARPET ||
                    (ballFront.X > 11.5f * FieldConstants.C && ballFront.X < 12.9f * FieldConstants.C))
                        Velocity -= new Vector3(Velocity.X / 10, Velocity.Y / 10, 1.8f * Velocity.Z);
                

                if (boundingSphere.Center.Z < 0 || boundingSphere.Center.Z > FieldConstants.C * FieldConstants.HEIGHT)
                    IsScored = true;

                if (IsScored)
                {
                    Vector3 ballBack = boundingSphere.Center - Vector3.Normalize(Velocity) * boundingSphere.Radius;
                    if (ballBack.Z < 0 || ballBack.Z > FieldConstants.C * FieldConstants.HEIGHT)
                        disappear = true;
                }
                    
            }
            else if (ballFront.Y < 0)
            {
                if (Math.Abs(Velocity.Y) < 0.1f)
                {
                    Velocity -= Vector3.UnitY * Velocity.Y;
                    ballInAir = false;
                }
                else
                {
                    Velocity -= new Vector3(Velocity.X / 7, 1.5f * Velocity.Y, Velocity.Z / 7);
                    wallIntersection = true;
                }
            }

            if (wallIntersection && robotIntersection)
                ballPossessed = true;

            RelativePosition += Velocity * dt;
        }

        public void PutOnRobot()
        {
            ballPossessed = true;
        }

        //TODO: add robot velocity also!
        public void ShootBall(float robotOrientation)
        {
            if (ballPossessed)
            {
                Vector3 direction = new Vector3(0, 1, 1); //45 degrees shooting
                direction = Vector3.Transform(direction, Matrix.CreateRotationY(robotOrientation));
                Velocity = Vector3.Normalize(direction) * SHOOTING_VELOCITY;
                ballInAir = true;
                ballPossessed = false;
            }
        }

        public void DrawOnMap(SpriteBatch spriteBatch)
        {
            if (disappear)
                return;
            Vector3 mapVector = RelativePosition * mapMetersToPixel;
            spriteBatch.Draw(ballTexture2D, new Vector2(mapVector.Z, -mapVector.X) + initialPositionOnMap,
                null, Color.White, 0, new Vector2(150, 150), (BALL_RADIUS * mapMetersToPixel) / 150,
                SpriteEffects.None, 0);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            if (disappear)
                return;
            Matrix oldWorld = effect.World;
            effect.World = Matrix.CreateTranslation(initialPositionOn3D + RelativePosition * FieldConstants.PIXELS_IN_ONE_METER) * effect.World;
            sphere.Draw(device, effect);
            effect.World = oldWorld;
        }
    }
}
