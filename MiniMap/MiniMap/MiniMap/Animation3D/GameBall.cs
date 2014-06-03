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

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        private Vector2 initialPositionOnMap;
        private Vector3 initialPositionOn3D;
        private float mapMetersToPixel;

        private Texture2D ballTexture2D;
        private Sphere sphere;

        public GameBall(Vector3 position3D, Vector2 mapPosition, float mapMetersToPixel,
            Texture2D ball2D, Texture2D ballCover)
        {
            this.initialPositionOn3D = position3D;
            this.initialPositionOnMap = mapPosition;
            this.mapMetersToPixel = mapMetersToPixel;
            this.ballTexture2D = ball2D;

            this.sphere = new Sphere(ballCover, new Vector3(0,BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER,0),
                BALL_RADIUS * FieldConstants.PIXELS_IN_ONE_METER, 40);
        }

        public void DrawOnMap(SpriteBatch spriteBatch)
        {
            Vector3 mapVector = Position * mapMetersToPixel;
            spriteBatch.Draw(ballTexture2D, new Vector2(mapVector.Z, -mapVector.X) + initialPositionOnMap,
                null, Color.White, 0, new Vector2(150, 150), (BALL_RADIUS * mapMetersToPixel) / 150,
                SpriteEffects.None, 0);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            Matrix oldWorld = effect.World;
            effect.World = Matrix.CreateTranslation(initialPositionOn3D + Position * FieldConstants.PIXELS_IN_ONE_METER) * effect.World;
            sphere.Draw(device, effect);
            effect.World = oldWorld;
        }
    }
}
