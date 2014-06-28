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
    class SimulatorGame : GameScreen
    {
        Texture2D map;
        Texture2D dashboardBack;
        RobotServer server;

        // Content
        BasicEffect effect3D, effectWithLighting;
        Field field;
        Robot robot;
        GameBall ball;

        Label score, gameState;
        Timer timer;
        HeadedLabel gyro, encoders;
        UpdatingList communicationList;

        Matrix proj;

        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (4f * FieldConstants.C);
        float forwardSpeed = 1f / (FieldConstants.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;
        Matrix view1, view2, view3, view4;
        Viewport defaultViewport, vp3D, vp2D, vpMap, vp1, vp2, vp3, vp4;
        Dictionary<Viewport, Matrix> viewDictionary;
        bool fourViews = false;

        int width, height;

        public static KeyboardState keyboardState;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 10.0f;
        static float farClip = 1000.0f;

        private int MiniMapLong = GameConstants.MINI_MAP_LONG;
        private int MiniMapShort = GameConstants.MINI_MAP_SHORT;

        float metersToPixel, ratioCurrentToFullScreen;

        bool movingCamera = true;
        bool match;

        public SimulatorGame(bool match)
        {
            metersToPixel = MiniMapLong / FieldConstants.HEIGHT_IN_METERS;

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

            this.match = match;
        }

        void RunPythonServer()
        {
            server = new RobotServer(robot, ball, communicationList);
            server.Start();

            string directory = @"C:\try\my_robot.py";
            RunPyScript(directory);
        }

        Process p;
        private void RunPyScript(string scriptName)
        {
            p = new Process();
            p.StartInfo.FileName = "python.exe";
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.UseShellExecute = false; //opens shell window and show errors if any
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = scriptName;
            p.Start(); // start the process (the python program)
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
            dashboardBack = Content.Load<Texture2D>("DashboardBackground");

            Vector3 robotPos, ballPos;

            if (match)
                robotPos = FieldConstants.C * new Vector3(FieldConstants.WIDTH / 4,
                0, FieldConstants.HEIGHT / 2);
            else
                robotPos = FieldConstants.C * new Vector3(FieldConstants.WIDTH / 2,
                0, FieldConstants.HEIGHT / 2);

            robot = new Robot(robotPos,
                 metersToPixel, MiniMapShort,
                 Content.Load<Texture2D>("greenBox"),
                 Content.Load<Texture2D>("innerWheel"),
                 Content.Load<Texture2D>("plaction"),
                 Content.Load<Texture2D>("chassis"));

            if (match)
                ballPos = robotPos;
            else
                ballPos = FieldConstants.C * new Vector3(FieldConstants.WIDTH / 4,
                0, FieldConstants.HEIGHT / 2);

            ball = new GameBall(ballPos,
                metersToPixel,
                Content.Load<Texture2D>("greenBall"), Content.Load<Texture2D>("ballLogo"));

            if (match)
                ball.PutOnRobot();


            field = new Field(Content);

            SpriteFont font = Content.Load<SpriteFont>("labelFont");
            SpriteFont bold = Content.Load<SpriteFont>("boldFont");

            score = new Label("0", new Vector2(width * 0.95f, 0),
                ratioCurrentToFullScreen, Color.Yellow, font);

            gameState = new Label("Teleop", new Vector2(width * 0.65f, height * 0.86f),
                0.5f * ratioCurrentToFullScreen, Color.Green, bold);

            timer = new Timer("0", new Vector2(width * 0.07f, 0),
                ratioCurrentToFullScreen, Color.Yellow, font);

            gyro = new HeadedLabel("Gyro", "0", new Vector2(width * 0.05f, height * 0.83f),
                0.45f * ratioCurrentToFullScreen, Color.Yellow, font);

            encoders = new HeadedLabel("Encoders", "0", new Vector2(width * 0.2f, height * 0.83f),
               0.45f * ratioCurrentToFullScreen, Color.Yellow, font);

            communicationList = new UpdatingList(4, new Vector2(width * 0.33f, height * 0.8f),
                0.4f * ratioCurrentToFullScreen, Color.Yellow, font);

            //float aspectRatio = GraphicsDevice.Viewport.AspectRatio;// 640/480 
            defaultViewport = GraphicsDevice.Viewport;
            vp2D = new Viewport(0, defaultViewport.Height - MiniMapShort, defaultViewport.Width - MiniMapLong, MiniMapShort);
            vp3D = new Viewport(0, 0, defaultViewport.Width, defaultViewport.Height - MiniMapShort);
            vpMap = new Viewport(defaultViewport.Width - MiniMapLong, defaultViewport.Height - MiniMapShort,
                MiniMapLong, MiniMapShort);
            vp1 = new Viewport(0, 0, vp3D.Width / 2, vp3D.Height / 2);
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

            RunPythonServer();

            if (match)
            {
                timer.Reset(GameConstants.MATCH_TIME);
                timer.CountDown = true;
            }
        }


        public static bool isXboxKeyPressed(string key)
        {
            switch (key)
            {
                case "A":
                    return GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;
                case "B":
                    return GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed;
                case "X":
                    return GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed;
                case "Y":
                    return GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed;
                case "LB":
                    return GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed;
                case "RB":
                    return GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed;
                case "Left":
                    return GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed;
                case "Right":
                    return GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed;
            }

            return false;
        }

        public override void BlockedUpdate(GameTime gameTime, KeyboardState state)
        {
            if (match)
                for (float t = 0; t < (float)gameTime.ElapsedGameTime.TotalSeconds; t += 0.001f)
                    ball.Update(0.001f, robot.GetBoundingSphere(), robot.Velocity, robot.AngularVelocity);
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

            keyboardState = state;

            if (match)
            {
                if (timer.TimePassed < GameConstants.AUTO_TIME)
                {
                    server.SetState(RobotState.Auto);
                    gameState.Color = Color.Green;
                    gameState.Text = "Auto";
                }
                else if (timer.TimePassed < GameConstants.MATCH_TIME)
                {
                    server.SetState(RobotState.Teleop);
                    gameState.Color = Color.Green;
                    gameState.Text = "Teleop";
                }
                else if (timer.TimePassed > GameConstants.MATCH_TIME)
                {
                    server.SetState(RobotState.Disabled);
                    gameState.Color = Color.Red;
                    gameState.Text = "Disabled";
                    ScreenManager.AddScreen(new EndOfMatchMenu());
                }

                if (timer.TimePassed > GameConstants.MATCH_TIME.Subtract(TimeSpan.FromSeconds(10)))
                    timer.Color = Color.Red;
                else
                    timer.Color = Color.Yellow;
            }


            if (state.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                server.Pause();
                ScreenManager.AddScreen(new PauseMenu());
                return;
            }
            else if (!match && state.IsKeyDown(Keys.F1))
            {
                server.SetState(RobotState.Teleop);
                gameState.Color = Color.Green;
                gameState.Text = "Teleop";
            }
            else if (!match && state.IsKeyDown(Keys.F2))
            {
                server.SetState(RobotState.Auto);
                gameState.Color = Color.Green;
                gameState.Text = "Auto";
            }
            else if (!match && state.IsKeyDown(Keys.F3))
            {
                server.SetState(RobotState.Disabled);
                gameState.Color = Color.Red;
                gameState.Text = "Disabled";
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

            server.Resume();

            if (ball.IsScored)
            {
                score.ChangeText(int.Parse(score.Text) + 10);
                ball.Restore(robot.Position);
            }

            if (server.RobotState == RobotState.Teleop && server.KeyboardDrive)
                robot.KeyboardDrive(state);

            robot.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            for (float t = 0; t < (float)gameTime.ElapsedGameTime.TotalSeconds; t += 0.001f)
                ball.Update(0.001f, robot.GetBoundingSphere(), robot.Velocity, robot.AngularVelocity);

            timer.Update(gameTime);
            gyro.ChangeText(robot.GyroAngle);
            encoders.ChangeText(((robot.EncoderLeft + robot.EncoderRight) / 2f));
        }

        public override void OnExiting()
        {
            server.Stop();
            try
            {
                p.Kill();
            }
            catch { }

        }

        public void Rematch()
        {
            if (!match || !(timer.TimePassed >= GameConstants.MATCH_TIME))
                return;

            timer.Reset(GameConstants.MATCH_TIME);
            timer.CountDown = true;
            score.Text = "0";
            robot.Reset();
            ball.Reset();
            if (match)
                ball.PutOnRobot();
            communicationList.Reset();
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
                GraphicsDevice.Viewport = vp3D;
                DrawFourViewPorts();
            }
            else
            {
                GraphicsDevice.Viewport = defaultViewport;
                if (movingCamera)
                    effect3D.View = robot.GetCameraView();
                effectWithLighting.View = effect3D.View;

                ball.Draw(GraphicsDevice, effectWithLighting);
                robot.Draw(GraphicsDevice, effect3D, effectWithLighting);
                field.Draw(GraphicsDevice, effect3D);
            }

            DrawDashboard();

            ScreenManager.FadeBackBufferFromColor(TransitionAlpha, Color.Black);
        }

        private void DrawDashboard()
        {
            GraphicsDevice.Viewport = vpMap;

            float alpha = fourViews ? 1f : 0.7f;

            spriteBatch.Begin();
            spriteBatch.Draw(map, new Rectangle(0, 0, vpMap.Width, vpMap.Height),
                null, Color.White * alpha, 0, Vector2.Zero, SpriteEffects.None, 1);
            robot.DrawOnMap(spriteBatch);
            ball.DrawOnMap(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.Viewport = defaultViewport;
            spriteBatch.Begin();
            spriteBatch.Draw(dashboardBack, new Rectangle(vp2D.X, vp2D.Y, vp2D.Width, vp2D.Height),
                null, Color.White * alpha, 0, Vector2.Zero, SpriteEffects.None, 0);
            gyro.Draw(spriteBatch);
            encoders.Draw(spriteBatch);
            communicationList.Draw(spriteBatch);
            gameState.Draw(spriteBatch);
            score.Draw(spriteBatch);
            timer.Draw(spriteBatch);

            if (fourViews)
                LineBatch.DrawLine(spriteBatch, Color.Black, Vector2.UnitY * (vp3D.Height - 3),
                    new Vector2(vp3D.Width, vp3D.Height - 3), 3, 1);

            LineBatch.DrawLine(spriteBatch, Color.Black, new Vector2(vp2D.Width * 0.15f, vp3D.Height),
                new Vector2(vp2D.Width * 0.15f, defaultViewport.Height), 1, 1);
            LineBatch.DrawLine(spriteBatch, Color.Black, new Vector2(vp2D.Width * 0.41f, vp3D.Height),
               new Vector2(vp2D.Width * 0.41f, defaultViewport.Height), 1, 1);
            LineBatch.DrawLine(spriteBatch, Color.Black, new Vector2(vp2D.Width * 0.8f, vp3D.Height),
               new Vector2(vp2D.Width * 0.8f, defaultViewport.Height), 1, 1);
            spriteBatch.End();
        }

        private void DrawFourViewPorts()
        {
            foreach (KeyValuePair<Viewport, Matrix> entry in viewDictionary)
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
                new Vector2(vp1.Width, vp3D.Height), 5, 1);
            LineBatch.DrawLine(spriteBatch, Color.Black, Vector2.UnitY * vp1.Height,
                new Vector2(vp3D.Width, vp1.Height), 5, 1);
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
