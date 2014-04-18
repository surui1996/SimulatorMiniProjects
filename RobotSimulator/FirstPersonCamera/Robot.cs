using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RobotSimulator
{
    class Robot
    {
        //m.k.s., degrees

        public float GyroAngle { get; set; }
        public float EncoderRight { get; set; }
        public float EncoderLeft { get; set; }

        public float RightOutput { get; set; }
        public float LeftOutput { get; set; }

        public Vector2 MapPosition { get; set; }
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }
        public float CameraOrientation { get; set; }

        private float mapMetersToPixel;
        private float vL, vR, velocity, angularVelocity;

        private const float maximumVelocity = 6f; //m/s
        private const float chassisWidth = 0.5f; //m

        //CAMERA STUFF
        //TODO: make it half the size of the robot
        private static Vector3 CameraRelativePosition = Vector3.Zero;
        private static Vector3 CameraStartingOrientation = new Vector3(0, 0, 1);

        public Robot(Vector3 position, Vector2 mapPosition, float mapMetersToPixel)
        {
            this.Position = position;
            this.MapPosition = mapPosition;
            this.mapMetersToPixel = mapMetersToPixel;

            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;

            RightOutput = 0;
            LeftOutput = 0;            
            
            Orientation = MathHelper.ToRadians(0);
            CameraOrientation = MathHelper.ToRadians(0);
        }

        public Matrix GetCameraView()
        {
            Vector3 cameraPosition = Position + CameraRelativePosition;

            Matrix rotationMatrix = Matrix.CreateRotationX(CameraOrientation)
                * Matrix.CreateRotationY(Orientation);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(CameraStartingOrientation, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = cameraPosition + transformedReference;

            // Set up the view matrix and projection matrix.
            return Matrix.CreateLookAt(cameraPosition, cameraLookat, Vector3.Up);
        }

        //TODO: take from the MiniMap project
        public void DrawRobotOnMap()
        {
            
        }

        //TODO: when we have robot animation...
        public void DrawRobot()
        {

        }

    }
}


