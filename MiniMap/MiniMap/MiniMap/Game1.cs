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
        Robot robot;

        float metersToPixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 741 / 2;
            graphics.PreferredBackBufferHeight = 335 / 2;
            metersToPixel = (float)graphics.PreferredBackBufferWidth / 16.45f;
            robot = new Robot(new Vector2(graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2), metersToPixel); 
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

            map = Content.Load<Texture2D>("carpet");
            chassis = Content.Load<Texture2D>("chassis");            
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
            KeyboardState keyboardState = Keyboard.GetState();
            //DirectInputGamepad gamePad = DirectInputGamepad.Gamepads[0]
            //DirectInputGamepad.Gamepads[0].Buttons.List.

            // Allows the game to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
                return;
            }
            else if (keyboardState.IsKeyDown(Keys.W))
            {
                robot.TankDrive(0.5f, 0.5f);
            }
            else if (keyboardState.IsKeyDown(Keys.A))
            {
                robot.TankDrive(0.9f, 1f);
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                robot.TankDrive(-0.3f, 0.3f);
            }

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

            spriteBatch.Begin();
            spriteBatch.Draw(map, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(chassis, robot.Position, null, Color.White, robot.Orientation,
                new Vector2(50, 25), (metersToPixel) / 100, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
