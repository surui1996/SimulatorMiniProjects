using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulator.GUI
{
    abstract class MenuScreen : GameScreen
    {
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        KeyboardState oldState;
        int selectedEntry = 0;
        string menuTitle;

        protected List<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (State == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }

        bool firstTime = true;
        public override void Update(GameTime gameTime, KeyboardState state)
        {
            base.Update(gameTime, state);

            if (IsExiting)
                return;

            if (firstTime)
            {
                oldState = state;
                firstTime = false;
                return;
            }

            int lastEntry = selectedEntry;
           
            if (state.IsKeyDown(Keys.Down))
            {
                // key Right has just been pressed.
                if (!oldState.IsKeyDown(Keys.Down))
                {
                    selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
                }
            }
            else if (state.IsKeyDown(Keys.Up))
            {
                // key Right has just been pressed.
                if (!oldState.IsKeyDown(Keys.Up))
                {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }
            }
            else if (state.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                    menuEntries[selectedEntry].OnSelectEntry();
            }
            else if (state.IsKeyDown(Keys.Escape))
            {
                if (!oldState.IsKeyDown(Keys.Escape))
                    ExitScreen();
            }

            oldState = state;
        }

        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = i == selectedEntry;

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(ScreenManager.Game.Window.ClientBounds.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
