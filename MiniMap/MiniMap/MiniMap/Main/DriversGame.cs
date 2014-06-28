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
using Soopah.Xna.Input;
using System.Diagnostics;
using System.Text;
using Simulator.Animation3D;
using Simulator.PhysicalModeling;
using Simulator.PythonCommunication;
using Simulator.GUI;
using Simulator.Helpers;

namespace Simulator.Main
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class DriversGame : GameScreen
    {
        Texture2D map;

        // Content
        BasicEffect effect3D, effectWithLighting;
        Field field;
        Robot robot;
        GameBall ball;

        Label score;
        Timer timer;

        Matrix proj;

        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (4f * FieldConstants.C);
        float forwardSpeed = 1f / (FieldConstants.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;

        Matrix view1, view2, view3, view4;
        Viewport defaultViewport, vpMap, vp1, vp2, vp3, vp4;
        Dictionary<Viewport, Matrix> viewDictionary;
        bool fourViews = false;

        int width, height;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 10.0f;
        static float farClip = 1000.0f;

        private int MiniMapLong = GameConstants.MINI_MAP_LONG;
        private int MiniMapShort = GameConstants.MINI_MAP_SHORT;

        float metersToPixel, ratioCurrentToFullScreen;

        bool movingCamera = true;

        public DriversGame()
        {
            Vector3 cameraPosition1 = FieldConstants.C * new Vector3(-FieldConstants.WIDTH * 1.2f, FieldConstants.HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 2);
            Vector3 cameraPosition2 = FieldConstants.C * new Vector3(0, FieldConstants.HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT);
            Vector3 cameraPosition3 = FieldConstants.C * new Vector3(FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0);
            Vector3 cameraPosition4 = FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.51f, FieldConstants.HEIGHT * 0.9f, FieldConstants.HEIGHT / 2);

            view1 = Matrix.CreateLookAt(cameraPosition1,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            view2 = Matrix.CreateLookAt(cameraPosition2,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            view3 = Matrix.CreateLookAt(cameraPosition3,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            view4 = Matrix.CreateLookAt(cameraPosition4,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);

            viewDictionary = new Dictionary<Viewport, Matrix>();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();


            width = ScreenManager.Game.Window.ClientBounds.Width;
            height = ScreenManager.Game.Window.ClientBounds.Height;

            ratioCurrentToFullScreen = (((float)width / GameConstants.RESOLUTION_X) +
                ((float)height / GameConstants.RESOLUTION_Y)) / 2;

            MiniMapLong = (int)(MiniMapLong * ratioCurrentToFullScreen);
            MiniMapShort = (int)(MiniMapShort * ratioCurrentToFullScreen);

            metersToPixel = MiniMapLong / FieldConstants.HEIGHT_IN_METERS;

            
            LineBatch.Init(GraphicsDevice);

            map = Content.Load<Texture2D>("carpet");
            robot = new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH / 2,
                0, FieldConstants.HEIGHT / 2),
                 metersToPixel, MiniMapShort,
                 Content.Load<Texture2D>("greenBox"),
                 Content.Load<Texture2D>("innerWheel"),
                 Content.Load<Texture2D>("plaction"),
                 Content.Load<Texture2D>("chassis"));

            ball = new GameBall(FieldConstants.C * new Vector3(FieldConstants.WIDTH / 4,
                0, FieldConstants.HEIGHT / 2),
                metersToPixel,
                Content.Load<Texture2D>("greenBall"), Content.Load<Texture2D>("ballLogo"));

            field = new Field(Content);

            score = new Label("0", new Vector2(width * 0.95f, 0),
                ratioCurrentToFullScreen, Color.Yellow, Content.Load<SpriteFont>("labelFont"));

            timer = new Timer("0", new Vector2(width * 0.07f, 0),
                ratioCurrentToFullScreen, Color.Yellow, Content.Load<SpriteFont>("labelFont"));


            //float aspectRatio = GraphicsDevice.Viewport.AspectRatio;// 640/480 
            defaultViewport = GraphicsDevice.Viewport;
            vpMap = new Viewport(defaultViewport.Width - MiniMapLong, defaultViewport.Height - MiniMapShort,
                MiniMapLong, MiniMapShort);
            vp1 = new Viewport(0, 0, defaultViewport.Width / 2, defaultViewport.Height / 2);
            vp2 = new Viewport(vp1.Width, 0, vp1.Width, vp1.Height);
            vp3 = new Viewport(0, vp1.Height, vp1.Width, vp1.Height);
            vp4 = new Viewport(vp1.Width, vp1.Height, vp1.Width, vp1.Height);

            viewDictionary.Add(vp1, view1);
            viewDictionary.Add(vp2, view2);
            viewDictionary.Add(vp3, view3);
            viewDictionary.Add(vp4, view4);
            //viewport3D = defaultViewport;
            //viewport3D.Height -= MiniMapShort;

            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle / FieldConstants.CAMERA_RATIO, defaultViewport.AspectRatio, nearClip, farClip);

            effect3D = new BasicEffect(GraphicsDevice);
            effect3D.TextureEnabled = true;
            effect3D.Projection = proj;

            effectWithLighting = new BasicEffect(GraphicsDevice);
            effectWithLighting.TextureEnabled = true;
            effectWithLighting.Projection = proj;
            effectWithLighting.EnableDefaultLighting();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, KeyboardState state)
        {
            base.Update(gameTime, state);

            if (IsExiting)
                return;

            state = Keyboard.GetState();

            // Allows the game to exit
            if (state.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ScreenManager.AddScreen(new PauseMenu());
                return;
            }
            else if (state.IsKeyDown(Keys.D1))
            {
                fourViews = false;
                movingCamera = false;
                effect3D.View = view1;
            }
            else if (state.IsKeyDown(Keys.D2))
            {
                fourViews = false;
                movingCamera = false;
                effect3D.View = view2;
            }
            else if (state.IsKeyDown(Keys.D3))
            {
                movingCamera = false;
                effect3D.View = view3;
            }
            else if (state.IsKeyDown(Keys.D4))
            {
                fourViews = false;
                movingCamera = false;
                effect3D.View = view4;
            }
            else if (state.IsKeyDown(Keys.D5))
            {
                fourViews = false;
                movingCamera = true;
            }
            else if (state.IsKeyDown(Keys.D6))
            {
                fourViews = true;
            }
            else if (state.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
            {
                // Rotate Up
                robot.CameraOrientation -= rotationSpeed;
            }
            else if (state.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
            {
                // Rotate Down
                robot.CameraOrientation += rotationSpeed;
            }
            else if (state.IsKeyDown(Keys.P) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                if (Math.Abs((robot.Position - ball.Position).Length()) < 1)
                    ball.PutOnRobot();
            }
            else if (state.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
            {
                ball.ShootBall(robot.Orientation, robot.Velocity);
            }

            if (ball.IsScored)
            {
                score.ChangeText(int.Parse(score.Text) + 10);
                ball.Restore(robot.Position);
            }
            robot.TankDrive(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y);
            robot.KeyboardDrive(state);

            robot.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            for (float t = 0; t < (float)gameTime.ElapsedGameTime.TotalSeconds; t += 0.001f)
                ball.Update(0.001f, robot.GetBoundingSphere(), robot.Velocity, robot.AngularVelocity);
            

            timer.Update(gameTime);

            
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spritebatch enables cull mode, and disable depth calculations
            Restore3DSettings();

            if (fourViews)
            {
                DrawFourViewPorts();
            }
            else
            {
                if (movingCamera)
                    effect3D.View = robot.GetCameraView();
                effectWithLighting.View = effect3D.View;

                ball.Draw(GraphicsDevice, effectWithLighting);
                robot.Draw(GraphicsDevice, effect3D, effectWithLighting);
                field.Draw(GraphicsDevice, effect3D);

                GraphicsDevice.Viewport = vpMap;
                spriteBatch.Begin();
                spriteBatch.Draw(map, new Rectangle(0, 0, vpMap.Width, vpMap.Height),
                    null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                robot.DrawOnMap(spriteBatch);
                ball.DrawOnMap(spriteBatch);
                spriteBatch.End();
            }
            GraphicsDevice.Viewport = defaultViewport;
            spriteBatch.Begin();
            score.Draw(spriteBatch);
            timer.Draw(spriteBatch);
            spriteBatch.End();

            ScreenManager.FadeBackBufferFromColor(TransitionAlpha, Color.Black);
        }

        private void DrawFourViewPorts()
        {
            foreach (KeyValuePair<Viewport, Matrix>  entry in viewDictionary)
            {
                GraphicsDevice.Viewport = entry.Key;
                effect3D.View = entry.Value;

                effectWithLighting.View = effect3D.View;
                ball.Draw(GraphicsDevice, effectWithLighting);
                robot.Draw(GraphicsDevice, effect3D, effectWithLighting);
                field.Draw(GraphicsDevice, effect3D);
            }
             GraphicsDevice.Viewport = defaultViewport;
            spriteBatch.Begin();
            LineBatch.DrawLine(spriteBatch, Color.Black, Vector2.UnitX * vp1.Width,
                new Vector2(vp1.Width, defaultViewport.Height), 5, 1);
            LineBatch.DrawLine(spriteBatch, Color.Black, Vector2.UnitY * vp1.Height,
                new Vector2(defaultViewport.Width, vp1.Height), 5, 1);
            spriteBatch.End();
        }

        private void Restore3DSettings()
        {
            //draw also backed traingles - doesn't help with the 0 z length vision targets
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = state;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
            