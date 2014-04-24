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
using MiniMap.Animation3D;
namespace MiniMap
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D map, chassis;
        //RobotOld robotOld;
        RobotClient client;

        // Content
        BasicEffect effect3D;
        Field field;
        Robot robot;
        Matrix proj;

        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (4f * FieldConstants.C);
        float forwardSpeed = 1f / (FieldConstants.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.ToRadians(88f);

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 10000.0f;


        private const int MiniMapLong = 741 / 4;
        private const int MiniMapShort = 335 / 4;

        public static KeyboardState keyboardState;

        float metersToPixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1365 / 2;
            graphics.PreferredBackBufferHeight = 765 / 2;
            metersToPixel = MiniMapLong / FieldConstants.HEIGHT_IN_METERS;
            //robotOld = new RobotOld(new Vector2(graphics.PreferredBackBufferWidth - (MiniMapWidth / 2f),
            //    graphics.PreferredBackBufferHeight - (MiniMapHeight / 2f)), metersToPixel);

            robot = new Robot(new Vector3(0, -(FieldConstants.HEIGHT_ABOVE_CARPET / 3) * FieldConstants.C,
                -(FieldConstants.HEIGHT / 2) * FieldConstants.C),
                new Vector2(graphics.PreferredBackBufferWidth - (MiniMapLong / 2f),
                graphics.PreferredBackBufferHeight - (MiniMapShort / 2f)), metersToPixel);
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

            //draw also backed traingles - doesn't help with the 0 z length vision targets
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = state;
        }

        void RunPythonServer()
        {
            string directory = @"C:\try\my_robot.py";
            //byte[] directoryBytes = Encoding.Default.GetBytes(directory);
            //directory = Encoding.UTF8.GetString(directoryBytes);

            RunPyScript(directory);

            //client = new RobotClient(robotOld);
            client = new RobotClient(robot);
            client.Start();
        }

        Process p;
        private void RunPyScript(string scriptName)
        {
            p = new Process();
            p.StartInfo.FileName = "python.exe";
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.UseShellExecute = false;
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
            chassis = Content.Load<Texture2D>("chassis");

            field = new Field(Content);

            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;// 640/480

            //TODO: learn about it more, what each argument actually means
            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle / FieldConstants.CAMERA_RATIO, aspectRatio, nearClip, farClip);

            effect3D = new BasicEffect(GraphicsDevice);
            effect3D.TextureEnabled = true;
            effect3D.Projection = proj;
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
                robot.Position += new Vector3(v.X, 0, v.Z);
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Matrix rotation = Matrix.CreateRotationY(robot.Orientation);
                Vector3 v = new Vector3(0, 0, -forwardSpeed);
                v = Vector3.Transform(v, rotation);
                robot.Position += new Vector3(v.X, 0, v.Z);
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
                client.Stop();
                p.Kill();
                this.Exit();
                return;
            }
            else if (keyboardState.IsKeyDown(Keys.F1))
            {
                client.SetState(RobotState.Teleop);
            } 
            else if (keyboardState.IsKeyDown(Keys.F2))
            {
                client.SetState(RobotState.Auto);
            }
            else if (keyboardState.IsKeyDown(Keys.F3))
            {
                client.SetState(RobotState.Disabled);
            }

            UpdateRobotPosition();

            //robotOld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            robot.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect3D.View = robot.GetCameraView();
            field.Draw(GraphicsDevice, effect3D);

            spriteBatch.Begin();
            spriteBatch.Draw(map, new Rectangle(graphics.PreferredBackBufferWidth - MiniMapLong,
                graphics.PreferredBackBufferHeight - MiniMapShort, MiniMapLong, MiniMapShort),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            //robotOld.DrawRobotOnMap(spriteBatch, chassis);
            robot.DrawRobotOnMap(spriteBatch, chassis);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
