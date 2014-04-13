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

        public float GyroAngle { get; set; }
        public float EncoderRight { get; set; }
        public float EncoderLeft { get; set; }

        public float RightOutput { get; set; }
        public float LeftOutput { get; set; }

        public Vector2 Position { get; set; }
        public float Orientation { get; set; }

        private float vL, vR, velocity, angularVelocity;
        private const float maximumVelocity = 6f; //m/s
        private const float chassisWidth = 0.5f; //m
        private float metersToPixel;

        public Robot(Vector2 position, float metersToPixel)
        {
            GyroAngle = 0;
            EncoderRight = 0;
            EncoderLeft = 0;
            RightOutput = 0;
            LeftOutput = 0;
            Position = position;
            Orientation = MathHelper.ToRadians(0);
            this.metersToPixel = metersToPixel;
        }

        public void Update(float dt) //dt = timeSinceLastUpdate
        {
            Position += velocity * new Vector2((float)Math.Cos(Orientation),
                    (float)Math.Sin(Orientation)) * dt * metersToPixel;
            Orientation += angularVelocity * dt;
            
            EncoderLeft += vL * dt;
            EncoderRight += vR * dt;

            GyroAngle += MathHelper.ToDegrees(angularVelocity * dt);
        }

        private void SetOutputs(float right, float Left)
        {
            vR = right * maximumVelocity;
            vL = Left * maximumVelocity;
            velocity = (vR + vL) / 2;
            angularVelocity = (vR - vL) / chassisWidth;
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

        public void TankDrive(float right, float left)
        {
            SetOutputs(right, left);
        }
    }
}
