using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiniMap
{
    class Robot
    {
        //m.k.s., degrees
        
        public double GyroAngle { get; set; }
        public double EncoderRight { get; set; }
        public double EncoderLeft { get; set; }

        public double RightOutput { get; set; }
        public double LeftOutput { get; set; }

        public Vector2 Position { get; set; }

        private double absoluteAngle;
        private Vector2 velocity;

        //the acceleration will change only when the motors output change, and after some time there will be a const velocity
        //public Vector2 Acceleration { get; set; }
        //private static double frictionAcceleration; //maybe add static and dynamic
        private static double outputToVelocity;

        public Robot()
        {
            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;
            RightOutput = 0;
            LeftOutput = 0;
            Position = Vector2.Zero;
            velocity = Vector2.Zero;
        }

        public void Update(double dt) //dt = timeSinceLastUpdate
        {
            Position += (float)dt * velocity;


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

        public void ArcadeDrive(double forward, double curve)
        {
            //TODO: copy from wpilib
        }

        public void TankDrive(double right, double left)
        {
            RightOutput = right;
            LeftOutput = left;
        }
    }
}
