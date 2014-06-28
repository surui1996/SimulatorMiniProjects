using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Animation3D;

namespace Simulator.PhysicalModeling
{
    class GameBall : DynamicObject
    {
        private const float BALL_RADIUS = 0.3f; //meters 
        private const float BALL_MASS = 2f; //kg
        private const float GRAVITY_ACCELERATION = 9.8f;
        private const float DRAG_COEFF = 0.47f;
        private const float AIR_DENSITY = 1.2f;
        private const float SHOOTING_VELOCITY = 10f; //m/s
        private const float ROTATE_RESISTANCE = 4f;

        public bool IsScored { get; set; }

        private bool ballPossessed = false, ballInAir = false, disappear = false;

        private Sphere sphere;
        private float yaw = 0, pitch = 0, roll = 0;

        public GameBall(Vector3 position3D, float mapMetersToPixel,
            Texture2D ball2D, Texture2D ballCover)
        {
            this.initialPositionOn3D = position3D;
            this.mapMetersToPixel = mapMetersToPixel;
            this.textureOnMap = ball2D;

            this.Position = position3D / FieldConstants.PIXELS_IN_ONE_METER + Vector3.UnitY * BALL_RADIUS;
            this.Velocity = Vector3.Zero;
            this.IsScored = false;

            this.sphere = new Sphere(ballCover, Vector3.Zero,
                BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER, 40);
        }

        public override void Reset()
        {
            this.Position = initialPositionOn3D / FieldConstants.PIXELS_IN_ONE_METER + Vector3.UnitY * BALL_RADIUS;
            this.Velocity = Vector3.Zero;
            this.IsScored = false;
            ballPossessed = false;
            ballInAir = false;
            disappear = false;
        }

        public void Restore()
        {
            Random random = new Random();
            float z = random.Next(9, 25) / 100f * FieldConstants.HEIGHT_IN_METERS;
            float x = random.Next(5, 95) / 100f * FieldConstants.WIDTH_IN_METERS;
            float y = (float)random.NextDouble() * 1.5f;
            
            this.Position = new Vector3(x, y, z) + Vector3.UnitY * BALL_RADIUS;

            this.Velocity = Vector3.UnitY;
            this.IsScored = false;
            ballPossessed = false; ballInAir = true; disappear = false;
        }

        public void Restore(Vector3 robotPosition)
        {
            Random random = new Random();
            float z;
            if (robotPosition.Z < 0.5 * FieldConstants.HEIGHT_IN_METERS)
                z = random.Next(75, 91) / 100f * FieldConstants.HEIGHT_IN_METERS;
            else
                z = random.Next(9, 25) / 100f * FieldConstants.HEIGHT_IN_METERS;
            float x = random.Next(5, 95) / 100f * FieldConstants.WIDTH_IN_METERS;
            float y = (float)random.NextDouble() * 1.5f;
            this.Position = new Vector3(x, y, z) + Vector3.UnitY * BALL_RADIUS;

            this.Velocity = Vector3.UnitY;
            this.IsScored = false;
            ballPossessed = false; ballInAir = true; disappear = false;
        }


        public void Update(float dt, BoundingSphere robotBox, Vector3 robotVelocity, float angularVelocity)
        {
            if (ballPossessed)
            {
                yaw += angularVelocity * dt;
                sphere.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

                Position = robotBox.Center / FieldConstants.PIXELS_IN_ONE_METER + Vector3.UnitY * BALL_RADIUS;
                Velocity = Vector3.Zero;
                return;
            }

            bool robotIntersection = false, wallIntersection = false;

            BoundingSphere boundingSphere = new BoundingSphere(Position * FieldConstants.PIXELS_IN_ONE_METER,
                    BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER);

            if (boundingSphere.Intersects(robotBox))
                robotIntersection = true;

            //velocity update
            if (ballInAir)
            {
                //Fdrag = 0.5*Cdrag*Density*cross-sectionalArea*v^2, Adrag = fDrag / ballMass
                float dragForceMagnitude = (float)(0.5 * DRAG_COEFF * AIR_DENSITY * Math.PI * Math.Pow(BALL_RADIUS, 2)
                        * Velocity.LengthSquared());
                Vector3 dragAcceleration = -(Vector3.Normalize(Velocity) * dragForceMagnitude) / BALL_MASS; // F = ma
                
                Vector3 lastVelocity = Velocity;
                Velocity = new Vector3(
                    Velocity.X + dragAcceleration.X * dt,
                    Velocity.Y + (-GRAVITY_ACCELERATION + dragAcceleration.Y) * dt,
                    Velocity.Z + dragAcceleration.Z * dt); 
            }
            else
            {
                if (robotIntersection)
                {
                    //velocity direction towards the robot
                    if (robotBox.Intersects(new Ray(Position * FieldConstants.PIXELS_IN_ONE_METER, Velocity)) != null)
                        Velocity = -0.5f * Velocity;

                    //ball velocity in the direction outside of the robot
                    if (Velocity.Length() < robotVelocity.Length())
                        Velocity = robotVelocity;
                }
                else
                {
                    if (Velocity.Length() < 0.03f)
                        Velocity = Vector3.Zero;
                    else
                        Velocity -= Vector3.Normalize(Velocity) * dt * 1.2f; //-1.2 m/s^2 acceleration
                }
            }

            

            //ball rotation
            pitch += (Velocity.Z / BALL_RADIUS) * dt / ROTATE_RESISTANCE;
            roll += (Velocity.X / BALL_RADIUS) * dt / ROTATE_RESISTANCE;
            sphere.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            Vector3 ballFront;
            if (Velocity.Length() != 0)
                ballFront = boundingSphere.Center + Vector3.Normalize(Velocity) * boundingSphere.Radius;
            else
                ballFront = boundingSphere.Center;
            //intersections
            if (ballFront.X < 0 || ballFront.X > FieldConstants.C * FieldConstants.WIDTH)
            {
                Velocity -= new Vector3(1.8f * Velocity.X, Velocity.Y / 10, Velocity.Z / 10);
                wallIntersection = true;
            }
            else if (ballFront.Z < 0 || ballFront.Z > FieldConstants.C * FieldConstants.HEIGHT)
            {
                
                wallIntersection = true;

                if ((Position.Y + BALL_RADIUS * 0.8f) * FieldConstants.PIXELS_IN_ONE_METER > FieldConstants.C * FieldConstants.HEIGHT_ABOVE_CARPET ||
                    (Position.Y - BALL_RADIUS * 0.8f) * FieldConstants.PIXELS_IN_ONE_METER < FieldConstants.C * FieldConstants.HIGH_GOAL_BOTTOM_ABOVE_CARPET ||
                    (ballFront.X > 11.5f * FieldConstants.C && ballFront.X < 12.9f * FieldConstants.C))
                {
                    Velocity -= new Vector3(Velocity.X / 10, Velocity.Y / 10, 1.8f * Velocity.Z);
                }

                if (boundingSphere.Center.Z < 0 || boundingSphere.Center.Z > FieldConstants.C * FieldConstants.HEIGHT)
                    IsScored = true;

                if (IsScored)
                {
                    Vector3 ballBack = boundingSphere.Center - Vector3.Normalize(Velocity) * boundingSphere.Radius;
                    if (ballBack.Z < 0 || ballBack.Z > FieldConstants.C * FieldConstants.HEIGHT)
                        disappear = true;
                }

            }
            else if (ballInAir && Velocity.Y < 0 && Position.Y < BALL_RADIUS)
            {
                Velocity -= new Vector3(Velocity.X / 10, 1.7f * Velocity.Y, Velocity.Z / 10);

                if (Math.Abs(Velocity.Y) < 0.1f)
                {
                    Velocity -= Vector3.UnitY * Velocity.Y;
                    ballInAir = false;
                }
            }

            if (wallIntersection && robotIntersection)
                ballPossessed = true;

            Position += Velocity * dt;
        }

        public void PutOnRobot()
        {
            ballPossessed = true;
        }

        public void ChangeSphereTexture(Texture2D newTexture)
        {
            sphere.Texture = newTexture;
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            sphere.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        public void ShootBall(float robotOrientation, Vector3 robotVelocity)
        {
            if (ballPossessed)
            {
                Vector3 direction = new Vector3(0, 1, 1); //45 degrees shooting
                direction = Vector3.Transform(direction, Matrix.CreateRotationY(robotOrientation));
                float velocity = (new Random()).Next(-10, 11) / 100f * SHOOTING_VELOCITY + SHOOTING_VELOCITY;
                Velocity = (Vector3.Normalize(direction) * velocity) + robotVelocity;
                ballInAir = true;
                ballPossessed = false;
            }
        }

        //draw must be with map viewport
        public override void DrawOnMap(SpriteBatch spriteBatch)
        {
            if (disappear)
                return;
            Vector3 mapVector = Position * mapMetersToPixel;
            spriteBatch.Draw(textureOnMap, new Vector2(mapVector.Z, spriteBatch.GraphicsDevice.Viewport.Height - mapVector.X),
                null, Color.White, 0, new Vector2(150, 150), (BALL_RADIUS * mapMetersToPixel) / 150,
                SpriteEffects.None, 0);
        }

        public override void Draw(GraphicsDevice device, BasicEffect effect, BasicEffect lighting = null)
        {
            if (disappear)
                return;
            Matrix oldWorld = effect.World;
            effect.World = Matrix.CreateTranslation(Position * FieldConstants.PIXELS_IN_ONE_METER) * effect.World;
            sphere.Draw(device, effect);
            effect.World = oldWorld;

            //Sphere s = new Sphere(textureOnMap, new BoundingSphere(Position * FieldConstants.PIXELS_IN_ONE_METER,
            //        BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER));
            //s.Draw(device, effect);
        }
    }
}
