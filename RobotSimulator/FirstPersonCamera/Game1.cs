using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Net.Sockets;
using RobotSimulator._3D;

namespace RobotSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Content
        BasicEffect effect;

        //ImageStreamingServer streamingServer;

        Field field;
        
        Cylinder c;
        Sphere sphere;
        //Circle circle;

        Box3 box;

        WheeledBox wheeledBox;
        Robot robot;

        //Matrix view;
        Matrix proj;

        // Set the camera position and rotation variables.
        //Vector3 cameraPos = new Vector3(-50, -(FieldConstants.HEIGHT_ABOVE_CARPET / 3) * FieldConstants.C, -500);
        //float angleX, angleY;


        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (4f * FieldConstants.C);
        float forwardSpeed = 50f / (FieldConstants.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.ToRadians(90f);

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 10000.0f;

        GraphicsDeviceManager graphics;

        public static object locker;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 320 *2;
            graphics.PreferredBackBufferHeight = 240 * 2;

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
            base.Initialize();
            //draw also backed traingles - doesn't help with the 0 z length vision targets
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = state;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            locker = new object();

            //carpet = Content.Load<Texture2D>("carpet");

            field = new Field(Content);

            c = new Cylinder(Content.Load<Texture2D>("grey"), Content.Load<Texture2D>("grey"), 20, 10,
                new Vector3(0, 20, 200));

            box = new Box3(Content.Load<Texture2D>("grey"), 50, 50, 50, new Vector3(30, 50, 350), 1f);
            
            sphere = new Sphere(Content.Load<Texture2D>("earth"),
                new Vector3(0, 40, 100), 40.0f, 80);

            wheeledBox = new WheeledBox(Content.Load<Texture2D>("earth"), Content.Load<Texture2D>("grey"), Content.Load<Texture2D>("grey"),
                2 * FieldConstants.C, 0.8f * FieldConstants.C, 0.25f * FieldConstants.C, Vector3.Zero);

            robot = new Robot(new Vector3(0, 10, 0),
                Vector2.Zero, 0f);

            //Set up projection matrice
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = 640f / 480f;//(float)viewport.Width / (float)viewport.Height;

            //TODO: learn about it more, what each argument actually means
            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle / FieldConstants.CAMERA_RATIO, aspectRatio, nearClip, farClip);

            effect = new BasicEffect(GraphicsDevice);
            effect.TextureEnabled = true;
            effect.Projection = proj;

            //streamingServer = new ImageStreamingServer();
            //streamingServer.Start(80);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateCameraPosition();
            base.Update(gameTime);
        }


        
        /// <summary>
        /// Updates the position and direction of the avatar.
        /// </summary>
        void UpdateCameraPosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
                //Environment.Exit(1);
                return;
            }

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

        float x = 0;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            x += 0.01f;
            //graphics.GraphicsDevice.Clear(Color.Black);
            //DrawWithEffect();
            //base.Draw(gameTime);
            //NewFrame();
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.View = robot.GetCameraView();
            field.Draw(GraphicsDevice, effect);
            c.Draw(GraphicsDevice, effect);
            sphere.Draw(GraphicsDevice, effect);

            box.Draw(GraphicsDevice, effect, x);

            wheeledBox.Draw(GraphicsDevice, effect, x);
            //circle.Draw(GraphicsDevice, effect);
            base.Draw(gameTime);
        }

        public static MemoryStream ImageStream;
        public static byte[] ImageBuffer;
        public static bool FrameRecieved;
        private Texture2D texture;
        private MemoryStream ms;

        private void NewFrame()
        {
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);
            byte[] result = new byte[backBuffer.Length * sizeof(int)];
            Buffer.BlockCopy(backBuffer, 0, result, 0, result.Length);

            using (texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat))
            {
                texture.SetData(backBuffer);

                using (ms = new MemoryStream())
                {
                    texture.SaveAsJpeg(ms, w, h);
                    lock (locker)
                    {
                        ImageBuffer = ms.GetBuffer();
                    }
                }
            }

            FrameRecieved = true;
        }

    }
}
