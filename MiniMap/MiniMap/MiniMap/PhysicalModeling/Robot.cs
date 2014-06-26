using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Animation3D;

namespace Simulator.PhysicalModeling
{
    class Robot : DynamicObject
    {
        //m.k.s., degrees

        public float GyroAngle { get; set; }
        public float EncoderRight { get; set; }
        public float EncoderLeft { get; set; }

        public float RightOutput { get; set; }
        public float LeftOutput { get; set; }

        public float Orientation { get; set; }
        public float CameraOrientation { get; set; }

        public float AngularVelocity { get; set; }

        //in map-pixels coordinate system
        private Vector2 cornerFrontLeft;
        private Vector2 cornerFrontRight;
        private Vector2 cornerRearLeft;
        private Vector2 cornerRearRight;

        private float mapHeight;

        private const float MAXIMUM_VELOCITY = 3f; //m/s
        private const float CHASSIS_WIDTH = 0.5f; //m
        private const float CHASSIS_LENGTH = CHASSIS_WIDTH * 2; //m
        private const float WHEEL_RADIUS = 0.15f; //m

        private WheeledBox wheeledBox;

        private static Vector3 CameraRelativePosition = Vector3.Zero;
        private static Vector3 CameraStartingOrientation = new Vector3(0, 0, 1);

        public Robot(Vector3 position3D, float mapMetersToPixel, float mapHeight,
            Texture2D body, Texture2D wheelSide, Texture2D wheelCircumference,
            Texture2D robot2D)
        {
            this.initialPositionOn3D = position3D;
            this.Position = position3D / FieldConstants.PIXELS_IN_ONE_METER;
            this.mapMetersToPixel = mapMetersToPixel;
            this.mapHeight = mapHeight;

            this.textureOnMap = robot2D;

            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;

            RightOutput = 0;
            LeftOutput = 0;

            Orientation = MathHelper.ToRadians(0);
            CameraOrientation = MathHelper.ToRadians(0);
            CameraRelativePosition = Vector3.UnitY * FieldConstants.HEIGHT_ABOVE_CARPET / 3 * FieldConstants.C;

            float c = FieldConstants.PIXELS_IN_ONE_METER;
            wheeledBox = new WheeledBox(body, wheelSide, wheelCircumference, CHASSIS_LENGTH * c,
                CHASSIS_WIDTH * c, WHEEL_RADIUS * c);
            
        }

        public void Update(float dt) //dt = timeSinceLastUpdate
        {
            float vL = LeftOutput * MAXIMUM_VELOCITY;
            if (velocityLeftZero)
                vL = 0;

            float vR = RightOutput * MAXIMUM_VELOCITY;
            if (velocityRightZero)
                vR = 0;

            float velocity = (vR + vL) / 2;
            float angularVelocity = (vR - vL) / CHASSIS_WIDTH;
            angularVelocity /= 6f; //because of friction

            if (velocityLeftZero || velocityRightZero)
                angularVelocity *= 2;

            Velocity = Vector3.Transform(Vector3.UnitZ * velocity, Matrix.CreateRotationY(Orientation));
            AngularVelocity = angularVelocity;
            
            Position += Velocity * dt;
            Orientation += angularVelocity * dt;

            //in map-pixels coordinate system
            Vector3 mapVector = Position * mapMetersToPixel;
            Vector2 pos = new Vector2(mapVector.Z, mapHeight - mapVector.X);            
            cornerFrontLeft = pos + Vector2.Transform(new Vector2(CHASSIS_LENGTH / 2, -CHASSIS_WIDTH / 2) * mapMetersToPixel,
                Matrix.CreateRotationZ(-Orientation));
            cornerRearLeft = pos + Vector2.Transform(new Vector2(-CHASSIS_LENGTH / 2, -CHASSIS_WIDTH / 2) * mapMetersToPixel,
                Matrix.CreateRotationZ(-Orientation));
            cornerFrontRight = pos + Vector2.Transform(new Vector2(CHASSIS_LENGTH / 2, CHASSIS_WIDTH / 2) * mapMetersToPixel,
                Matrix.CreateRotationZ(-Orientation));
            cornerRearRight = pos + Vector2.Transform(new Vector2(-CHASSIS_LENGTH / 2, CHASSIS_WIDTH / 2) * mapMetersToPixel,
                Matrix.CreateRotationZ(-Orientation));

            GyroAngle += MathHelper.ToDegrees(angularVelocity * dt);
            EncoderLeft += vL * dt;
            EncoderRight += vR * dt;

            //CheckIntersectionWithField();
        }

        public Matrix GetCameraView()
        {
            Vector3 cameraPosition = Position * FieldConstants.PIXELS_IN_ONE_METER + CameraRelativePosition;

            Matrix rotationMatrix = Matrix.CreateRotationX(CameraOrientation)
                * Matrix.CreateRotationY(Orientation);

            // Create a vector pointing the direction the camera is facing.
            Vector3 cameraDirection = Vector3.Transform(CameraStartingOrientation, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = cameraPosition + cameraDirection;

            // Set up the view matrix and projection matrix.
            return Matrix.CreateLookAt(cameraPosition, cameraLookat, Vector3.Up);
        }

        //not working well
        //public BoundingBox GetBoundingBox()
        //{
        //    Vector3 centerPosition = Position * FieldConstants.PIXELS_IN_ONE_METER;      
            
        //    Vector3 cornerLowRearLeft = centerPosition + Vector3.Transform(new Vector3(-CHASSIS_WIDTH / 2, 0, -CHASSIS_LENGTH / 2) * FieldConstants.PIXELS_IN_ONE_METER,
        //        Matrix.CreateRotationY(Orientation));
        //    Vector3 cornerUpFrontRight = centerPosition + Vector3.Transform(new Vector3(CHASSIS_WIDTH / 2, WHEEL_RADIUS * 2, CHASSIS_LENGTH / 2) * FieldConstants.PIXELS_IN_ONE_METER,
        //        Matrix.CreateRotationY(Orientation));

        //    Vector3 min = new Vector3(0, 0, 0);
        //    Vector3 max = new Vector3(0, WHEEL_RADIUS * 2, 0);

        //    return new BoundingBox(cornerLowRearLeft, cornerUpFrontRight);
        //}

        public BoundingSphere GetBoundingSphere()
        {
            Vector3 centerPosition = Position * FieldConstants.PIXELS_IN_ONE_METER;

            return new BoundingSphere(centerPosition + Vector3.UnitY * WHEEL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER,
                CHASSIS_LENGTH * FieldConstants.PIXELS_IN_ONE_METER / 2f);
        }

        public override void Reset()
        {
            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;

            RightOutput = 0;
            LeftOutput = 0;

            Position = initialPositionOn3D / FieldConstants.PIXELS_IN_ONE_METER;

            Orientation = MathHelper.ToRadians(0);
            CameraOrientation = MathHelper.ToRadians(0);
            CameraRelativePosition = Vector3.UnitY * FieldConstants.HEIGHT_ABOVE_CARPET / 3 * FieldConstants.C;
        }

        public void ResetGyro()
        {
            GyroAngle = 0;
        }

        public void ResetEncoders()
        {
            EncoderLeft = 0;
            EncoderRight = 0;
        }

        private float Limit(float value)
        {
            if (value > 1f) value = 1f;
            else if (value < -1f) value = -1f;
            return value;
        }

        public void ArcadeDrive(float forward, float curve)
        {
            float leftMotorOutput;
            float rightMotorOutput;

            forward = Limit(forward);
            curve = -Limit(curve);


            if (forward > 0.0)
            {
                if (curve > 0.0)
                {
                    leftMotorOutput = forward - curve;
                    rightMotorOutput = Math.Max(forward, curve);
                }
                else
                {
                    leftMotorOutput = Math.Max(forward, -curve);
                    rightMotorOutput = forward + curve;
                }
            }
            else
            {
                if (curve > 0.0)
                {
                    leftMotorOutput = -Math.Max(-forward, curve);
                    rightMotorOutput = forward + curve;
                }
                else
                {
                    leftMotorOutput = forward - curve;
                    rightMotorOutput = -Math.Max(-forward, -curve);
                }
            }
            SetOutputs(leftMotorOutput, rightMotorOutput);

        }

        public void TankDrive(float left, float right)
        {
            SetOutputs(Limit(left), Limit(right));
        }

        private void SetOutputs(float left, float right)
        {
            LeftOutput = left;
            RightOutput = right;
        }

        //draw must be with map viewport
        public override void DrawOnMap(SpriteBatch spriteBatch)
        {
            Vector3 mapVector = Position * mapMetersToPixel;
            spriteBatch.Draw(textureOnMap, new Vector2(mapVector.Z, spriteBatch.GraphicsDevice.Viewport.Height - mapVector.X),
                null, Color.White, -this.Orientation, new Vector2(50, 25), (mapMetersToPixel) / 100,
                SpriteEffects.None, 0);
            spriteBatch.Draw(textureOnMap, cornerFrontLeft,
               null, Color.White, 0, new Vector2(50, 25), (mapMetersToPixel) / 200,
               SpriteEffects.None, 0);
            spriteBatch.Draw(textureOnMap, cornerRearRight,
               null, Color.White, 0, new Vector2(50, 25), (mapMetersToPixel) / 200,
               SpriteEffects.None, 0);
        }

        public override void Draw(GraphicsDevice device, BasicEffect effect, BasicEffect lighting)
        {
            Matrix oldWorld = effect.World;
            Matrix oldLighting = lighting.World;

            effect.World = Matrix.CreateTranslation(Position * FieldConstants.PIXELS_IN_ONE_METER) * effect.World;
            lighting.World = Matrix.CreateTranslation(Position * FieldConstants.PIXELS_IN_ONE_METER) * lighting.World;
            
            wheeledBox.Draw(device, effect, lighting, Orientation);

            effect.World = oldWorld;
            lighting.World = oldLighting;
        }

        private bool velocityRightZero, velocityLeftZero;

        private void CheckIntersectionWithField()
        {
            velocityLeftZero = false;
            velocityRightZero = false;
            CheckIntersectionWithXLine(-FieldConstants.HEIGHT_IN_METERS / 2 * mapMetersToPixel, false);
            CheckIntersectionWithXLine(FieldConstants.HEIGHT_IN_METERS / 2 * mapMetersToPixel, true);
            CheckIntersectionWithYLine(-FieldConstants.WIDTH_IN_METERS / 2 * mapMetersToPixel, false);
            CheckIntersectionWithYLine(FieldConstants.WIDTH_IN_METERS / 2 * mapMetersToPixel, true);
        }

        private void CheckIntersectionWithXLine(float x, bool robotPosGreaterThanX)
        {
            int d = robotPosGreaterThanX ? 1 : -1;

            if ((cornerFrontLeft.X * d < x * d && LeftOutput > 0) || (cornerRearLeft.X * d < x * d && LeftOutput < 0))
                velocityLeftZero = true;

            if ((cornerFrontRight.X * d < x * d && RightOutput > 0) || (cornerRearRight.X * d < x * d && RightOutput < 0))
                velocityRightZero = true;
        }

        private void CheckIntersectionWithYLine(float y, bool robotPosGreaterThanY)
        {
            int d = robotPosGreaterThanY ? 1 : -1;

            if ((cornerFrontLeft.Y * d < y * d && LeftOutput > 0) || (cornerRearLeft.Y * d < y * d && LeftOutput < 0))
                velocityLeftZero = true;

            if ((cornerFrontRight.Y * d < y * d && RightOutput > 0) || (cornerRearRight.Y * d < y * d && RightOutput < 0))
                velocityRightZero = true;
        }

    }
}
