using System;
using EvilEngine.Core;
using EvilEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvilEngine.Lab
{
    public enum PlayerStatus
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Jump = 4,
        Dash = 8,
        OnGround = 16
    }

    public class States
    {
        public Vector2 Acceleration;
        public Vector2 Position;
        public PlayerStatus Status;

        public void Copy(States other)
        {
            Status = other.Status;
            Position = other.Position;
            Acceleration = other.Acceleration;
        }
    }

    public class Player
    {
        public readonly States CurrentState;

        public readonly GameCore Game;
        public readonly States LastState;

        public Transform Hitbox;
        public Vector2 HitboxOffset;

        public Vector2 Scale;
        public Texture2D Texture;

        public Player(GameCore game)
        {
            Game = game;

            CurrentState = new States
            {
                Status = PlayerStatus.Jump,
                Position = Vector2.Zero,
                Acceleration = Vector2.Zero
            };

            LastState = new States();
            LastState.Copy(CurrentState);

            Hitbox = new Transform(0, 0, 12, 40);
            HitboxOffset = new Vector2(6, 20);

            Scale = Vector2.One;
        }

        public void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("player");
            Console.WriteLine(Texture.Bounds.ToString());
            Hitbox = new Transform(Texture.Bounds);
            HitboxOffset = Hitbox.Size / 2;
        }

        public void Update()
        {
            UpdatePhysics();
        }

        private void UpdatePhysics()
        {
            LastState.Copy(CurrentState);

            if (CurrentState.Status == PlayerStatus.Jump)
                CurrentState.Acceleration.Y += 40.0f * (float) Game.DeltaTime;


            if (CurrentState.Position.Y >= 400)
            {
                CurrentState.Status = PlayerStatus.OnGround;
                CurrentState.Acceleration = Vector2.Zero;
            }


            Hitbox.Center = CurrentState.Position + HitboxOffset;
            CurrentState.Position += CurrentState.Acceleration * (float) Game.DeltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CurrentState.Position, Color.White);
        }
    }
}