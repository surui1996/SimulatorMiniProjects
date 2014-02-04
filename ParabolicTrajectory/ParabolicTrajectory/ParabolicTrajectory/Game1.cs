using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ParabolicTrajectory
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D ball2D;
        const int BALL_DIAMETER = 51;
        Texture2D arrow2D;
        Texture2D ground2D;
        const int GROUND_LEN = 225;

        Graph graph;
        float velocity, angle;
        const float kG = 9.8f, X_POS = 50f, Y_POS = 200f, DEFAULT_VELOCITY = 50f, DT = 0.005f, SCALE = 0.1f;
        Color DEFAULT_COLOR = Color.CornflowerBlue;
        bool spaceClicked = false, finishedShot = false;
        float timeFromShot;
        int positionIndex;
        Vector2 origin;
        List<Vector3> positions;
        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graph = new Graph(GraphicsDevice);
            positions = new List<Vector3>();
            velocity = DEFAULT_VELOCITY;
            angle = 45.0f;
            spaceClicked = false;
            origin = new Vector2(X_POS, Y_POS);


            CalcTrajectory(velocity, angle);
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            ball2D = Content.Load<Texture2D>("transparent ball");
            arrow2D = Content.Load<Texture2D>("transparent arrow");
            ground2D = Content.Load<Texture2D>("ground");
        }

        private void CalcTrajectory(float velocity, float angle)
        {
            float angleRad = MathHelper.ToRadians(angle);
            
            float vx = velocity * (float)Math.Cos(angleRad);
            float vy =  velocity * (float)Math.Sin(angleRad);
            Vector3 pos = Vector3.Zero;
            
            for (float t = 0; t < 100.0f; t += DT)
            {
                pos = new Vector3(origin, 0) + new Vector3(vx * t, -vy * t + kG * t * t, 0);
                positions.Add(pos);
                //if (HasBallReachedGround((int)(t / DT)))
                //    break;
            }
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            // Allows the game to exit
            if (state.IsKeyDown(Keys.Escape))
                this.Exit();
            else if (state.IsKeyDown(Keys.W))
                angle += 0.05f;
            else if (state.IsKeyDown(Keys.S))
                angle -= 0.05f;
            else if (state.IsKeyDown(Keys.D))
                velocity += 0.5f;
            else if (state.IsKeyDown(Keys.A))
                velocity -= 0.5f;

            if (state.IsKeyDown(Keys.Space) && !spaceClicked)
            {
                positions = new List<Vector3>();
                CalcTrajectory(velocity, angle);
                spaceClicked = true;
                finishedShot = false;
                timeFromShot = 0;
            }
            else if (!state.IsKeyDown(Keys.Space) && spaceClicked)
                spaceClicked = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(DEFAULT_COLOR);

            spriteBatch.Begin();

            DrawShootingParameters();
            DrawMovingBall(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            DrawGround();

            spriteBatch.End();

            graph.Draw(positions, Color.Black);
            base.Draw(gameTime);
        }

        private void DrawShootingParameters()
        {
            spriteBatch.DrawString(font, "velocity: " + Math.Round(velocity).ToString() + "m/s", new Vector2(600, 30), Color.Red);
            spriteBatch.DrawString(font, "angle: " + angle.ToString(), new Vector2(600, 60), Color.Red);
            spriteBatch.Draw(arrow2D, origin, null, Color.CornflowerBlue, -MathHelper.ToRadians(angle),
                new Vector2(0f, 55f), new Vector2(SCALE * 0.8f * velocity / DEFAULT_VELOCITY, SCALE), SpriteEffects.None, 0);
        }

        private void DrawMovingBall(float ellapsedSeconds)
        {
            if (!finishedShot)
            {
                positionIndex = (int)(timeFromShot / DT);
                spriteBatch.Draw(ball2D, new Vector2(positions[positionIndex].X, positions[positionIndex].Y), null, DEFAULT_COLOR, 0f,
                    new Vector2(51f, 51f), SCALE, SpriteEffects.None, 0);
                if (HasBallReachedGround(positionIndex))
                    finishedShot = true;

                timeFromShot += ellapsedSeconds;
            }
            else
                spriteBatch.Draw(ball2D, new Vector2(positions[positionIndex].X, positions[positionIndex].Y), null, DEFAULT_COLOR, 0f,
                    new Vector2(BALL_DIAMETER, BALL_DIAMETER), SCALE, SpriteEffects.None, 0);
        }

        private void DrawGround()
        {
            float len = ((float)GROUND_LEN * SCALE);
            for (int i = 0; i <= (int)((float)(graphics.PreferredBackBufferWidth) / len); i++)
                spriteBatch.Draw(ground2D, new Vector2(i * len, Y_POS), null, Color.White, 0f, new Vector2(0, 0),
                    SCALE, SpriteEffects.None, 0);
        }

        private bool HasBallReachedGround(int positionIndex)
        {
            return positionIndex > 10 && Math.Abs(positions[positionIndex].Y - Y_POS) < 1f;
        }

    }
}
