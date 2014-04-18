using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMap
{
    class RobotOld
    {
        //m.k.s., degrees

        public float GyroAngle { get; set; }
        public float EncoderRight { get; set; }
        public float EncoderLeft { get; set; }

        public float RightOutput { get; set; }
        public float LeftOutput { get; set; }

        public Vector2 Position { get; set; }
        public float Orientation { get; set; }

        private float metersToPixel;
        private float vL, vR, velocity, angularVelocity;

        private const float maximumVelocity = 6f; //m/s
        private const float chassisWidth = 0.5f; //m        

        public RobotOld(Vector2 position, float metersToPixel)
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
            Orientation += -angularVelocity * dt;

            GyroAngle += MathHelper.ToDegrees(angularVelocity * dt);
            EncoderLeft += vL * dt;
            EncoderRight += vR * dt;            
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
            spriteBatch.Draw(chassisTexture, this.Position, null, Color.White, this.Orientation,
                new Vector2(50, 25), (metersToPixel) / 100, SpriteEffects.None, 0);
        }
    }
}
