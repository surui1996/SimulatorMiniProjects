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
using Microsoft.Xna.Framework.Content;

namespace Simulator.Main
{
    enum GameMode { Drivers = 0, Programmer = 1, Match = 2, Exit = 3 }

    class MainMenu : GameScreen
    {
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
        Vector3 cameraPosition, cameraTarget;
        float cameraSpeed = 2f;
        bool moveCamera;

        Viewport defaultViewport;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 10.0f;
        static float farClip = 1000.0f;

        public static KeyboardState oldState;

        public MainMenu(bool moveCamera)
        {
            this.moveCamera = moveCamera;
            if (moveCamera)
                cameraPosition = FieldConstants.C * new Vector3(-FieldConstants.WIDTH / 3f, FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 7f);
            else
                cameraPosition = FieldConstants.C * new Vector3(FieldConstants.WIDTH / 2f, FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT / 4.5f);
            cameraTarget = FieldConstants.C * 0.5f * new Vector3(FieldConstants.WIDTH, 2 * FieldConstants.TRUSS_HEIGHT_ABOVE_CARPET, FieldConstants.HEIGHT);
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            Texture2D innerWheel = Content.Load<Texture2D>("innerWheel");
            Texture2D plaction = Content.Load<Texture2D>("plaction");
            Texture2D blackBox = Content.Load<Texture2D>("blackBox");
            Texture2D greenBox = Content.Load<Texture2D>("greenBox");
            Texture2D ballLogo = Content.Load<Texture2D>("ballLogo");

            robots = new List<Robot>();
            for (int i = 0; i < 3; i++)
                robots.Add(new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH * (i + 1) / 4,
                0, FieldConstants.HEIGHT * 0.6f), 0, 0,
                i % 2 == 0 ? blackBox : greenBox, innerWheel, plaction, null));
            for (int i = 0; i < 3; i++)
            {
                Robot r = new Robot(FieldConstants.C * new Vector3(FieldConstants.WIDTH * (i + 1) / 4,
                0, FieldConstants.HEIGHT * 0.37f), 0, 0,
                i % 2 == 0 ? greenBox : blackBox, innerWheel, plaction, null);
                r.Orientation += MathHelper.Pi;
                robots.Add(r);
            }

            balls = new List<GameBall>();
            for (int i = 0; i < 6; i++)
            {
                balls.Add(new GameBall(Vector3.Zero, 0, null, ballLogo));
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
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, KeyboardState state)
        {
            base.Update(gameTime, state);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < 6; i++)
                balls[i].Update(dt, robots[i].GetBoundingSphere(),
                    robots[i].Velocity, robots[i].AngularVelocity);

            foreach (BallMenuEntry entry in entries)
                entry.Update(dt);

            if (moveCamera && cameraPosition.X < FieldConstants.C * FieldConstants.WIDTH / 2)
            {
                cameraPosition.X += cameraSpeed;
                cameraPosition.Z = 0.005f * cameraPosition.X * cameraPosition.X;
                return;
            }                
            
            if (this.IsExiting)
                return;

            int lastIndex = selectionIndex;
            
            if (state.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ScreenManager.Exit();
                return;
            }
            else if (state.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    GameMode mode = (GameMode)selectionIndex;
                    switch (mode)
                    {
                        case GameMode.Drivers: ExitScreen(); ScreenManager.AddScreen(new DriversGame());
                            break;
                        case GameMode.Programmer: ExitScreen(); ScreenManager.AddScreen(new SimulatorGame(false));
                            break;
                        case GameMode.Match: ExitScreen(); ScreenManager.AddScreen(new SimulatorGame(true));
                            break;
                        case GameMode.Exit: ScreenManager.Exit();
                            break;
                    }
                    ScreenManager.Game.ResetElapsedTime();
                    return;
                }
            }


            if (state.IsKeyDown(Keys.Right))
            {
                // key Right has just been pressed.
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    selectionIndex++;
                    if (selectionIndex == entries.Count)
                        selectionIndex = 0;
                }
            }

            if (state.IsKeyDown(Keys.Left))
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

            oldState = state;
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
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            effect3D.View = view;
            effectWithLighting.View = effect3D.View;

            foreach (BallMenuEntry entry in entries)
                entry.Draw(GraphicsDevice, effectWithLighting);

            foreach (Robot r in robots)
                r.Draw(GraphicsDevice, effect3D, effectWithLighting);

            foreach (GameBall b in balls)
                b.Draw(GraphicsDevice, effectWithLighting);

            field.Draw(GraphicsDevice, effect3D);

            ScreenManager.FadeBackBufferFromColor(TransitionAlpha, Color.Black);
        }

        //protected override void OnExiting(Object sender, EventArgs args)
        //{
        //    base.OnExiting(sender, args);
        //    Environment.Exit(1);
        //}

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
