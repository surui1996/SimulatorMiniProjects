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

namespace RobotSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Content
        BasicEffect effect;

        //MJPEGStreamer streamer;
        ImageStreamingServer streamingServer;

        TexturedWall wallRed, wallBlue, carpet3;
        TexturedWall dynamicRightVision, staticRightVision;
        //TexturedWall dynamicLeftVision, staticLeftVision;
        Matrix view;
        Matrix proj;

        // Set the camera position and rotation variables.
        Vector3 cameraPos = new Vector3(-50, -(Field.HEIGHT_ABOVE_CARPET / 3) * Field.C, -500);
        float angleX, angleY;

        // Set the direction the camera points without rotation.
        Vector3 cameraReference = new Vector3(0, 0, 1);

        // Set rates in world units per 1/60th second (the default fixed-step interval).
        float rotationSpeed = 1f / (10f * Field.C);
        float forwardSpeed = 50f / (3f * Field.C);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.ToRadians(45f);

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 10000.0f;

        GraphicsDeviceManager graphics;

        public static object locker;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 320;
            graphics.PreferredBackBufferHeight = 240;

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
            wallRed = new TexturedWall(Field.WIDTH, Field.HEIGHT_ABOVE_CARPET, 0,
                new Vector3(Field.WIDTH / 2, Field.HEIGHT_ABOVE_CARPET / 2, 0),
                Content.Load<Texture2D>("visionRed"));

            wallBlue = new TexturedWall(Field.WIDTH, Field.HEIGHT_ABOVE_CARPET, 0,
                new Vector3(Field.WIDTH / 2, Field.HEIGHT_ABOVE_CARPET / 2, -Field.HEIGHT),
                Content.Load<Texture2D>("visionBlue"));

            dynamicRightVision = new TexturedWall(Field.DYNAMIC_WIDTH, Field.DYNAMIC_HEIGHT, 0.1f,
                new Vector3(-Field.WIDTH / 2, -Field.HEIGHT_ABOVE_CARPET / 2, 0) +
                new Vector3((Field.LOWGOAL_WIDTH + Field.LOWGOAL_WIDTH) / 2, Field.DYNAMIC_HEIGHT_ABOVE_CARPET + Field.DYNAMIC_HEIGHT, 0),
                Content.Load<Texture2D>("visionTarget"));

            staticRightVision = new TexturedWall(Field.STATIC_WIDTH, Field.STATIC_HEIGHT, 0.1f,
                new Vector3(-Field.WIDTH / 2, -Field.HEIGHT_ABOVE_CARPET / 2, 0) +
                new Vector3(Field.LOWGOAL_WIDTH + Field.STATIC_WIDTH + Field.STATIC_BLACK_STRIPES_WIDTH,
                    Field.STATIC_HEIGHT_ABOVE_CARPET + Field.STATIC_HEIGHT, 0),
                Content.Load<Texture2D>("visionTarget"));



            carpet3 = new TexturedWall(Field.WIDTH, 0, Field.HEIGHT,
                new Vector3(Field.WIDTH / 2, -Field.HEIGHT_ABOVE_CARPET / 2, 0),
                Content.Load<Texture2D>("carpet"));

            //Set up projection matrice
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = 640f / 480f;//(float)viewport.Width / (float)viewport.Height;

            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle / Field.CAMERA_RATIO, aspectRatio, nearClip, farClip);

            effect = new BasicEffect(GraphicsDevice);

            //streamer = new MJPEGStreamer();
            //streamer.Start();
            streamingServer = new ImageStreamingServer();
            streamingServer.Start(80);
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
            UpdateMatrices();

            base.Update(gameTime);
        }


        int cou = 0;
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
                return;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // Rotate left .
                angleX += rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // Rotate right.
                angleX -= rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Matrix rotation = Matrix.CreateRotationY(angleX);
                Vector3 v = new Vector3(0, 0, forwardSpeed);
                v = Vector3.Transform(v, rotation);
                cameraPos.Z += v.Z;
                cameraPos.X += v.X;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Matrix rotation = Matrix.CreateRotationY(angleX);
                Vector3 v = new Vector3(0, 0, -forwardSpeed);
                v = Vector3.Transform(v, rotation);
                cameraPos.Z += v.Z;
                cameraPos.X += v.X;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                // Rotate Up
                angleY += rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                // Rotate Down
                angleY -= rotationSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                // Rotate Down
                //CopyScreen(cou);
                cou++;
            }
        }

        /// <summary>
        /// Updates the position and direction of the camera relative to the avatar.
        /// </summary>
        void UpdateMatrices()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(angleX) * Matrix.CreateRotationX(angleY);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = cameraPos + transformedReference;

            // Set up the view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPos, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            DrawWithEffect();
            base.Draw(gameTime);


            NewFrame();


            graphics.GraphicsDevice.Clear(Color.Black);
            DrawWithEffect();
            base.Draw(gameTime);
        }

        private void DrawWithEffect()
        {
            effect.TextureEnabled = true;
            effect.Texture = wallRed.WallTexture;
            effect.Projection = proj;
            effect.View = view;

            // apply the effect and render the cube
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                wallRed.Draw(GraphicsDevice);
            }

            effect.Texture = carpet3.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                carpet3.Draw(GraphicsDevice);
            }

            effect.Texture = wallBlue.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                wallBlue.Draw(GraphicsDevice);
            }

            effect.Texture = dynamicRightVision.WallTexture;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                dynamicRightVision.Draw(GraphicsDevice);
                staticRightVision.Draw(GraphicsDevice);
            }
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
