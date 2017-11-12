using System;
using EvilEngine.Core;
using EvilEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvilEngine.Lab
{
    public enum PlayerStatus
    {
        None,
        Ground,
        Air,
        Dash
    }

    public class States
    {
        public Vector2 Acceleration;
        public Transform Hitbox;
        public PlayerStatus Status;

        public void Copy(States other)
        {
            Status = other.Status;
            Hitbox = other.Hitbox;
            Acceleration = other.Acceleration;
        }
    }

    // TODO: Jump Height
    public class Player
    {
        public readonly States CurrentState;

        public readonly GameCore Game;
        public readonly States LastState;
        public Vector2 TextureOffset;

        public Vector2 Scale;
        public Texture2D Texture;

        public Player(GameCore game)
        {
            Game = game;

            CurrentState = new States
            {
                Status = PlayerStatus.Air,
                Hitbox = new Transform(),
                Acceleration = Vector2.Zero
            };

            LastState = new States();
            LastState.Copy(CurrentState);

            Scale = Vector2.One;
        }

        public void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("spritesheet");
            Console.WriteLine(Texture.Bounds.ToString());
            CurrentState.Hitbox = new Transform(0,0,29,65);
            TextureOffset = Vector2.Zero;
        }

        public void Update()
        {
            LastState.Copy(CurrentState);
            UpdateInputs();
            UpdatePhysics();
            
            
        }

        private void UpdateInputs()
        {
            if (CurrentState.Status == PlayerStatus.Air || CurrentState.Status == PlayerStatus.Ground)
            {
                if (Game.Input.Key.Is.Down(Keys.Left))
                {
                    CurrentState.Acceleration.X = -180;
                }
                else if (Game.Input.Key.Is.Down(Keys.Right))
                {
                    CurrentState.Acceleration.X = 180;
                }
                else
                {
                    CurrentState.Acceleration.X = 0;
                }
            }

            if (CurrentState.Status == PlayerStatus.Ground && Game.Input.Key.Is.Press(Keys.Up))
            {
                    CurrentState.Acceleration.Y = -300;
                    CurrentState.Status = PlayerStatus.Air;
            }
            else if (CurrentState.Status == PlayerStatus.Air && CurrentState.Acceleration.Y < 0 && Game.Input.Key.Is.Down(Keys.Up))
            {
                CurrentState.Acceleration.Y = Math.Max(CurrentState.Acceleration.Y, -400);
            }
        }

        private void UpdatePhysics()
        {
            if (CurrentState.Status == PlayerStatus.Air)
                CurrentState.Acceleration.Y += 500.0f * (float) Game.DeltaTime;
            
            
            CurrentState.Hitbox.Position += CurrentState.Acceleration * (float) Game.DeltaTime;

            if (CurrentState.Hitbox.Y >= 400 && LastState.Status != PlayerStatus.Ground)
            {
                CurrentState.Status = PlayerStatus.Ground;
                CurrentState.Acceleration.Y = 0;
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CurrentState.Hitbox.Position + TextureOffset, new Rectangle(212, 0, 29, 65), Color.White);

            string debug = $"Status:  {CurrentState.Status} \n Acceleration: {CurrentState.Acceleration} \n Position: {CurrentState.Hitbox.Position} ";
            
            spriteBatch.DrawString(Game.DefaultFont, debug, Vector2.Zero, Color.White);
        }
    }
}