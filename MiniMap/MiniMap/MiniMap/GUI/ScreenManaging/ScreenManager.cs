using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulator.GUI
{
    class ScreenManager : DrawableGameComponent
    {
        Texture2D blankTexture;
        List<GameScreen> screens = new List<GameScreen>();
        bool isInitialized = false;

        public SpriteBatch SpriteBatch { get; set; }
        public SpriteFont Font { get; set; }
        public List<GameScreen> Screens { get { return screens; } }
        public Texture2D TextureBlank { get { return blankTexture; } }

        public ScreenManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("labelFont");
            blankTexture = content.Load<Texture2D>("blank");

            foreach (GameScreen screen in screens)
                screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            int lastCount = screens.Count;
            bool updateBlocked = false;
            for (int i = 0; i < screens.Count; i++)
            {
                GameScreen popped = screens[screens.Count - i - 1];

                if (updateBlocked)
                    popped.BlockedUpdate(gameTime, state);
                else
                    popped.Update(gameTime, state);

                if (lastCount != screens.Count) 
                    break;

                if (popped.BlocksUpdate)
                    updateBlocked = true;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                screen.Draw(gameTime);
            }

        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
                screen.LoadContent();

            screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            screen.OnExiting();

            if (isInitialized)
                screen.UnloadContent();

            screens.Remove(screen);
        }

        public void FadeBackBufferToColor(float alpha, Color color)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();

            SpriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             color * alpha);

            SpriteBatch.End();
        }

        public void FadeBackBufferFromColor(float alpha, Color color)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();


            SpriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             color * (1 - alpha));

            SpriteBatch.End();
        }

        public void Exit()
        {
            Game.Exit();
        }
    }
}
