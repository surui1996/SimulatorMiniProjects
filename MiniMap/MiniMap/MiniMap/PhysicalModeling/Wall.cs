using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simulator.PhysicalModeling
{
    enum Direction { PositiveDirection, NegativeDirection }
    enum Axis { X, Z }
    class Wall
    {
        public float LineCoordinate { get; set; }
        public Direction Direction { get; set; }
        public Axis Axis { get; set; }

        private List<Vector3> lastCornersPosition;

        const float ELASTIC_COEFF = 1000;
        const float DAMP_COEFF = 10000;
        const float FRICTION_COEFF = 0.3f;
        const float ROTATION_INERTIA = 11;

        public Wall(Axis axis, Direction direction, float lineCoordinate)
        {
            Axis = axis;
            Direction = direction;
            LineCoordinate = lineCoordinate;
        }

        public void Interact(float dt, Robot robot)
        {
            switch (Axis)
            {
                case Axis.X: InteractX(dt, robot);
                    break;
                case Axis.Z: InteractZ(dt, robot);
                    break;
            }
        }

        private void InteractZ(float dt, Robot robot)
        {
            List<Vector3> corners = robot.GetCorners();
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 corner = corners[i];
                int d = Direction == Direction.PositiveDirection ? 1 : -1;

                if (corner.Z * d < LineCoordinate * d)
                    continue;

                float dz = Math.Abs(corner.Z - LineCoordinate);
                float vx = 0, vz = 0;
                if (lastCornersPosition[i] != null)
                {
                    vx = corner.X - lastCornersPosition[i].X;
                    vz = corner.Z - lastCornersPosition[i].Z;
                }
                float accelerationMagnitude = -d * ELASTIC_COEFF * dz - DAMP_COEFF * vz;
                Vector3 velocity = robot.Velocity;
                float ax = -Math.Sign(vx) * FRICTION_COEFF * Math.Abs(accelerationMagnitude);

                velocity.X = 0;//assuming friction is very high
                velocity.Z += accelerationMagnitude * dt; 
                robot.Velocity = velocity;

                //making the robot rotate by calculating moments around center of mass
                Vector3 centerToCorner = corner - robot.GetBoundingSphere().Center / FieldConstants.PIXELS_IN_ONE_METER;
                robot.AngularVelocity += (-centerToCorner.X * accelerationMagnitude +
                    centerToCorner.Z * ax) * dt;
            }

            lastCornersPosition = corners;
        }

        private void InteractX(float dt, Robot robot)
        {
            List<Vector3> corners = robot.GetCorners();
            for (int i = 0; i < corners.Count; i++)
            {
                Vector3 corner = corners[i];
                int d = Direction == Direction.PositiveDirection ? 1 : -1;

                if (corner.X * d < LineCoordinate * d)
                    continue;

                float dx = Math.Abs(corner.X - LineCoordinate);
                float vx = 0, vz = 0;
                if (lastCornersPosition[i] != null)
                {
                    vx = corner.X - lastCornersPosition[i].X;
                    vz = corner.Z - lastCornersPosition[i].Z;
                }
                float accelerationMagnitude = -d * ELASTIC_COEFF * dx - DAMP_COEFF * vx;
                Vector3 velocity = robot.Velocity;
                float az = -Math.Sign(vz) * FRICTION_COEFF * Math.Abs(accelerationMagnitude);

                velocity.X += accelerationMagnitude * dt;
                velocity.Z = 0; //assuming friction is very high
                robot.Velocity = velocity;
                
                //making the robot rotate by calculating moments around center of mass
                Vector3 centerToCorner = corner - robot.GetBoundingSphere().Center / FieldConstants.PIXELS_IN_ONE_METER;
                robot.AngularVelocity += (centerToCorner.Z * accelerationMagnitude +
                    -centerToCorner.X * az) * dt;
            }

            lastCornersPosition = corners;
        }
    }
}
