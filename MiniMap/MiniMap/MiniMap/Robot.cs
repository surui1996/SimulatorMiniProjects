using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniMap.Animation3D;

namespace MiniMap
{
    class Robot
    {
        //m.k.s., degrees

        public float GyroAngle { get; set; }
        public float EncoderRight { get; set; }
        public float EncoderLeft { get; set; }

        public float RightOutput { get; set; }
        public float LeftOutput { get; set; }
        
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }
        public float CameraOrientation { get; set; }

        private Vector2 initialPositionOnMap;
        private Vector3 initialPositionOn3D;

        private float mapMetersToPixel;
        private float vL, vR, velocity, angularVelocity;

        private const float maximumVelocity = 3f; //m/s
        private const float chassisWidth = 0.5f; //m

        //CAMERA STUFF
        //TODO: make it half the size of the robot
        private static Vector3 CameraRelativePosition = Vector3.Zero;
        private static Vector3 CameraStartingOrientation = new Vector3(0, 0, 1);

        public Robot(Vector3 position3D, Vector2 mapPosition, float mapMetersToPixel)
        {
            this.initialPositionOn3D = position3D;
            this.initialPositionOnMap = mapPosition;
            this.mapMetersToPixel = mapMetersToPixel;

            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;

            RightOutput = 0;
            LeftOutput = 0;

            Orientation = MathHelper.ToRadians(0);
            CameraOrientation = MathHelper.ToRadians(0);
        }

        public void Update(float dt) //dt = timeSinceLastUpdate
        {
            //MapPosition += velocity * new Vector2((float)Math.Cos(Orientation),
            //        (float)Math.Sin(Orientation)) * dt * mapMetersToPixel;
            Position += velocity * new Vector3((float)Math.Sin(Orientation), 0,
                (float)Math.Cos(Orientation)) * dt;//;//mapMetersToPixel;
            Orientation += -angularVelocity * dt;

            GyroAngle += MathHelper.ToDegrees(angularVelocity * dt);
            EncoderLeft += vL * dt;
            EncoderRight += vR * dt;
        }

        public Matrix GetCameraView()
        {
            Vector3 cameraPosition = Position * (FieldConstants.C / FieldConstants.FOOT_IN_METERS) + initialPositionOn3D + CameraRelativePosition;

            Matrix rotationMatrix = Matrix.CreateRotationX(CameraOrientation)
                * Matrix.CreateRotationY(Orientation);

            // Create a vector pointing the direction the camera is facing.
            Vector3 cameraDirection = Vector3.Transform(CameraStartingOrientation, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = cameraPosition + cameraDirection;

            // Set up the view matrix and projection matrix.
            return Matrix.CreateLookAt(cameraPosition, cameraLookat, Vector3.Up);
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
            curve = Limit(curve);


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

            vR = right * maximumVelocity;
            vL = left * maximumVelocity;
            velocity = (vR + vL) / 2;
            angularVelocity = (vR - vL) / chassisWidth;
        }

        public void DrawRobotOnMap(SpriteBatch spriteBatch, Texture2D chassisTexture)
        {
            Vector3 mapVector = Position * mapMetersToPixel;
            spriteBatch.Draw(chassisTexture, new Vector2(mapVector.Z, mapVector.X) + initialPositionOnMap,
                null, Color.White, this.Orientation, new Vector2(50, 25), (mapMetersToPixel) / 100,
                SpriteEffects.None, 0);
        }

        //TODO: when we have robot animation...
        public void DrawRobot()
        {

        }

    }
}
