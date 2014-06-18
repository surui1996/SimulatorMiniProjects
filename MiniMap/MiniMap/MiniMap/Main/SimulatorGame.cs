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

namespace Simulator.Main
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SimulatorGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D map;
        RobotServer server;

        // Content
        BasicEffect effect3D, effectWithLighting;
        Field field;
        Robot robot;
        GameBall ball;
        Matrix proj;

        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (4f * FieldConstants.C);
        float forwardSpeed = 1f / (FieldConstants.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 10.0f;
        static float farClip = 1000.0f;

        private const int MiniMapLong = 741 / 3;
        private const int MiniMapShort = 335 / 3;

        public static KeyboardState keyboardState;

        float metersToPixel;

        Vector3 cameraPosition1, cameraPosition2, cameraPosition3, cameraPosition4;
        bool movingCamera = true;

        public SimulatorGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
            graphics.PreferredBackBufferWidth = (int)(1366 / 1.5);
            graphics.PreferredBackBufferHeight = (int)(768 / 1.5);
            //graphics.IsFullScreen = true;
            metersToPixel = MiniMapLong / FieldConstants.HEIGHT_IN_METERS;

            cameraPosition1 = FieldConstants.C * new Vector3(-FieldConstants.WIDTH * 1.2f, FieldConstants.HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 2);
            cameraPosition4 = FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.500001f, FieldConstants.HEIGHT * 0.93f, FieldConstants.HEIGHT / 2);
            cameraPosition2 = FieldConstants.C * new Vector3(0, FieldConstants.HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT);
            cameraPosition3 = FieldConstants.C * new Vector3(FieldConstants.WIDTH, FieldConstants.HEIGHT_ABOVE_CARPET, 0);
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            RunPythonServer();
        }

        void RunPythonServer()
        {
            server = new RobotServer(robot, ball);
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<Texture2D>("carpet");
            robot = new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH / 2,
                0, FieldConstants.HEIGHT / 2),
                new Vector2(graphics.PreferredBackBufferWidth - (MiniMapLong / 2f),
                graphics.PreferredBackBufferHeight - (MiniMapShort / 2f)), metersToPixel,
                 Content.Load<Texture2D>("greenBox"),
                 Content.Load<Texture2D>("innerWheel"),
                 Content.Load<Texture2D>("plaction"),
                 Content.Load<Texture2D>("chassis"));

            ball = new GameBall(FieldConstants.C * new Vector3(FieldConstants.WIDTH / 4,
                0, FieldConstants.HEIGHT / 2),
                new Vector2(graphics.PreferredBackBufferWidth - (MiniMapLong / 2),
                graphics.PreferredBackBufferHeight - (MiniMapShort / 4)), metersToPixel,
                Content.Load<Texture2D>("greenBall"), Content.Load<Texture2D>("ballLogo"));

            field = new Field(Content);

            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;// 640/480

            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle / FieldConstants.CAMERA_RATIO, aspectRatio, nearClip, farClip);

            effect3D = new BasicEffect(GraphicsDevice);
            effect3D.TextureEnabled = true;
            effect3D.Projection = proj;

            effectWithLighting = new BasicEffect(GraphicsDevice);
            effectWithLighting.TextureEnabled = true;
            effectWithLighting.Projection = proj;
            effectWithLighting.EnableDefaultLighting();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        void UpdateRobotPosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // Rotate left .
                //angleX += rotationSpeed;
                robot.Orientation += rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // Rotate right.
                //angleX -= rotationSpeed;
                robot.Orientation -= rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Matrix rotation = Matrix.CreateRotationY(robot.Orientation);
                Vector3 v = new Vector3(0, 0, forwardSpeed);
                v = Vector3.Transform(v, rotation);
                robot.RelativePosition += new Vector3(v.X, 0, v.Z);
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Matrix rotation = Matrix.CreateRotationY(robot.Orientation);
                Vector3 v = new Vector3(0, 0, -forwardSpeed);
                v = Vector3.Transform(v, rotation);
                robot.RelativePosition += new Vector3(v.X, 0, v.Z);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                // Rotate Up
                //angleY -= rotationSpeed;
                robot.CameraOrientation -= rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                // Rotate Down
                //angleY += rotationSpeed;
                robot.CameraOrientation += rotationSpeed;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
                return;
            }
            else if (keyboardState.IsKeyDown(Keys.F1))
            {
                server.SetState(RobotState.Teleop);
            }
            else if (keyboardState.IsKeyDown(Keys.F2))
            {
                server.SetState(RobotState.Auto);
            }
            else if (keyboardState.IsKeyDown(Keys.F3))
            {
                server.SetState(RobotState.Disabled);
            }
            else if (keyboardState.IsKeyDown(Keys.D1))
            {
                movingCamera = false;
                effect3D.View = Matrix.CreateLookAt(cameraPosition1,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                movingCamera = false;
                effect3D.View = Matrix.CreateLookAt(cameraPosition2,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            }
            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                movingCamera = false;
                effect3D.View = Matrix.CreateLookAt(cameraPosition3,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            }
            else if (keyboardState.IsKeyDown(Keys.D4))
            {
                movingCamera = false;
                effect3D.View = Matrix.CreateLookAt(cameraPosition4,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 0, FieldConstants.HEIGHT), Vector3.Up);
            }
            else if (keyboardState.IsKeyDown(Keys.D5))
            {
                movingCamera = true;
            }
            else if (keyboardState.IsKeyDown(Keys.P))
            {
                if (Math.Abs((robot.Position - ball.Position).Length()) < 1)
                    ball.PutOnRobot();
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                ball.ShootBall(robot.Orientation, robot.Velocity);
            }

            if (ball.IsScored)
                ball.Restore();

            UpdateRobotPosition();

            //robotOld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            robot.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            ball.Update((float)gameTime.ElapsedGameTime.TotalSeconds, robot.GetBoundingSphere(), robot.Velocity);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Restore3DSettings();
            
            if (movingCamera)
                effect3D.View = robot.GetCameraView();
            effectWithLighting.View = effect3D.View;

            ball.Draw(GraphicsDevice, effectWithLighting);
            robot.Draw(GraphicsDevice, effect3D, effectWithLighting);
            field.Draw(GraphicsDevice, effect3D);
            

            
            //spritebatch enables cull mode, and disable depth calculations
            spriteBatch.Begin();
            spriteBatch.Draw(map, new Rectangle(graphics.PreferredBackBufferWidth - MiniMapLong,
                graphics.PreferredBackBufferHeight - MiniMapShort, MiniMapLong, MiniMapShort),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            robot.DrawOnMap(spriteBatch);
            ball.DrawOnMap(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            server.Stop();
            System.Threading.Thread.Sleep(10);
            p.Kill();
            Environment.Exit(1);
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
            