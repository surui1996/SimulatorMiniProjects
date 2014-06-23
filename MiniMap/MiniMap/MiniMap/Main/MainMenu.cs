using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulator.Animation3D;
using Simulator.PhysicalModeling;
using Microsoft.Xna.Framework.Input;
using Simulator.GUI;

namespace Simulator.Main
{
    enum GameMode { Drivers = 0, Programmer = 1, Match = 2, Exit = 3 }

    class MainMenu : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Content
        BasicEffect effect3D, effectWithLighting;
        Field field;
        
        List<Robot> robots;
        List<GameBall> balls;

        List<BallMenuEntry> entries;
        int selectionIndex = 0;


        Matrix proj;

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;

        Matrix view;
        Vector3 cameraPosition;

        Viewport defaultViewport;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 10.0f;
        static float farClip = 1000.0f;

        public static KeyboardState newState, oldState;

        public MainMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = (int)(1366 / 1.5);
            graphics.PreferredBackBufferHeight = (int)(768 / 1.5);
            //graphics.IsFullScreen = true;

            cameraPosition = FieldConstants.C * new Vector3(FieldConstants.WIDTH / 2, FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 4.5f);
            view = Matrix.CreateLookAt(cameraPosition,
            FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 2* FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT), Vector3.Up);
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D innerWheel =  Content.Load<Texture2D>("innerWheel");
            Texture2D plaction =  Content.Load<Texture2D>("plaction");
            Texture2D blackBox =  Content.Load<Texture2D>("blackBox");
            Texture2D greenBox =  Content.Load<Texture2D>("greenBox");
            Texture2D ballLogo = Content.Load<Texture2D>("ballLogo");

            robots = new List<Robot>();
            for (int i = 0; i < 3; i++)
                robots.Add(new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH * (i + 1) / 4,
                0, FieldConstants.HEIGHT * 0.6f),
                Vector2.Zero, 0, i % 2 == 0 ? blackBox : greenBox, innerWheel, plaction, null));
            for (int i = 0; i < 3; i++)
            {
                Robot r = new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH * (i + 1) / 4,
                0, FieldConstants.HEIGHT * 0.37f),
                Vector2.Zero, 0, i % 2 == 0 ? greenBox : blackBox, innerWheel, plaction, null);
                r.Orientation += MathHelper.Pi;
                robots.Add(r);
            }

            balls = new List<GameBall>();
            for (int i = 0; i < 6; i++)
            {
                balls.Add(new GameBall(Vector3.Zero, Vector2.Zero, 0, null, ballLogo));
                balls[i].PutOnRobot();
            }

            BallMenuEntry drivers = new BallMenuEntry(FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.71f,
                FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET + FieldConstants.TRUSS_SQUARE_EDGE,
                FieldConstants.HEIGHT / 2),
                Content.Load<Texture2D>("drivers"), Content.Load<Texture2D>("drivers2"));
            BallMenuEntry programmer = new BallMenuEntry(FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.57f,
                FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET + FieldConstants.TRUSS_SQUARE_EDGE,
                FieldConstants.HEIGHT / 2),
                Content.Load<Texture2D>("programmer"), Content.Load<Texture2D>("programmer2"));
            BallMenuEntry match = new BallMenuEntry(FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.43f,
                FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET + FieldConstants.TRUSS_SQUARE_EDGE,
                FieldConstants.HEIGHT / 2),
                Content.Load<Texture2D>("match"), Content.Load<Texture2D>("match2"));
            BallMenuEntry exit = new BallMenuEntry(FieldConstants.C * new Vector3(FieldConstants.WIDTH * 0.29f,
                FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET + FieldConstants.TRUSS_SQUARE_EDGE,
                FieldConstants.HEIGHT / 2),
                Content.Load<Texture2D>("exit"), Content.Load<Texture2D>("exit2"));

            entries = new List<BallMenuEntry>();
            entries.Add(drivers); entries.Add(programmer); entries.Add(match); entries.Add(exit);
            entries[selectionIndex].Select();

            field = new Field(Content);


            //float aspectRatio = GraphicsDevice.Viewport.AspectRatio;// 640/480 
            defaultViewport = GraphicsDevice.Viewport;

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
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            int lastIndex = selectionIndex;

            // Allows the game to exit
            if (newState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
                return;
            }
            else if (newState.IsKeyDown(Keys.Enter))
            {
                GameMode mode = (GameMode)selectionIndex;
                switch (mode)
                {
                    case GameMode.Drivers: Components.Add(new DriversGame() as IGameComponent);
                        break;
                    case GameMode.Programmer: Components.Add(new DriversGame() as IGameComponent);
                        break;
                    case GameMode.Match: Components.Add(new DriversGame() as IGameComponent);
                        break;
                    case GameMode.Exit: this.Exit();
                        break;
                }
                Components.Remove(this as IGameComponent);
                return;
            }

            
            if (newState.IsKeyDown(Keys.Right))
            {
                // key Right has just been pressed.
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    selectionIndex++;
                    if (selectionIndex == entries.Count)
                        selectionIndex = 0;
                }
            }

            if (newState.IsKeyDown(Keys.Left))
            {
                // key Right has just been pressed.
                if (!oldState.IsKeyDown(Keys.Left))
                {
                    selectionIndex--;
                    if (selectionIndex == -1)
                        selectionIndex = entries.Count - 1;
                }
            }

            if (!entries[selectionIndex].Selected)
            {
                entries[selectionIndex].Select();
                entries[lastIndex].UnSelect();
            }

            oldState = newState;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (BallMenuEntry entry in entries)
                entry.Update(dt);

            for (int i = 0; i < 6; i++)
                balls[i].Update(dt, robots[i].GetBoundingSphere(),
                    robots[i].Velocity, robots[i].AngularVelocity);
                

            //robot.TankDrive(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y);
            //robot.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            //ball.Update((float)gameTime.ElapsedGameTime.TotalSeconds, robot.GetBoundingSphere(), robot.Velocity, robot.AngularVelocity);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spritebatch enables cull mode, and disable depth calculations
            Restore3DSettings();
            effect3D.View = view;
            effectWithLighting.View = effect3D.View;

            foreach (BallMenuEntry entry in entries)
                entry.Draw(GraphicsDevice, effectWithLighting);

            foreach (Robot r in robots)
                r.Draw(GraphicsDevice, effect3D, effectWithLighting);

            foreach (GameBall b in balls)
                b.Draw(GraphicsDevice, effectWithLighting);

            field.Draw(GraphicsDevice, effect3D);

            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
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
