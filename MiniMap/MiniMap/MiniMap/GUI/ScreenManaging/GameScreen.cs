using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Simulator.GUI
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff
    }

    abstract class GameScreen
    {
        public bool BlocksUpdate { get; set; }
        public bool IsExiting { get; set; }

        public ScreenState State { get; set; }

        public TimeSpan TransitionOnTime { get; protected set; }
        public TimeSpan TransitionOffTime { get; protected set; }

        public ScreenManager ScreenManager { get; set; }

        public GraphicsDevice GraphicsDevice { get; set; }
        public SpriteBatch spriteBatch;
        public ContentManager Content { get; set; }

        float transitionPosition = 1f;
        public float TransitionPosition { get { return transitionPosition; } set { transitionPosition = value; } }
        public float TransitionAlpha { get { return 1f - TransitionPosition; } }

        public GameScreen()
        {
        }

        public virtual void LoadContent() 
        {
            GraphicsDevice = ScreenManager.GraphicsDevice;
            spriteBatch = ScreenManager.SpriteBatch;
            Content = ScreenManager.Game.Content;
        }

        public virtual void UnloadContent() { }

        public virtual void BlockedUpdate(GameTime gameTime, KeyboardState state) { }

        public virtual void Update(GameTime gameTime, KeyboardState state)
        {
            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                State = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, TransitionOnTime, -1))
                {
                    // Still busy transitioning.
                    State = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    State = ScreenState.Active;
                }
            }
        }

        public virtual void Draw(GameTime gameTime) { }

        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (TransitionPosition <= 0)) ||
                ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                IsExiting = true;
            }
        }

        public virtual void OnExiting() { }
    }
}
