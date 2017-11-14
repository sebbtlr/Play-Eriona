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
        public ForceList Velocity;
        public Vector2 Speed;
        public Transform Hitbox;
        public PlayerStatus Status;

        public void Copy(States other)
        {
            Status = other.Status;
            Hitbox = new Transform(other.Hitbox);
            Speed = other.Speed;
            other.Velocity.CopyIn(out Velocity);
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

        public const float WALKSPEED = 250;
        public const float JUMPSPEED = 400;
        public readonly Vector2 GRAVITY = Vector2.UnitY * 550;
        

        public Player(GameCore game)
        {
            Game = game;

            CurrentState = new States
            {
                Status = PlayerStatus.Air,
                Hitbox = new Transform(),
                Speed = Vector2.Zero,
                Velocity = new ForceList()
            };

            LastState = new States();
            CurrentState.Velocity.CopyIn(out LastState.Velocity);

            Scale = Vector2.One;

            CurrentState.Velocity.AddForce("GRAVITY", GRAVITY);
        }

        public void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("spritesheet");
            Console.WriteLine(Texture.Bounds.ToString());
            CurrentState.Hitbox = new Transform(0, 0, 29, 65);
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
            if (Game.Input.Key.Is.Down(Keys.Left))
            {
                CurrentState.Speed.X = -WALKSPEED;
            }
            else if (Game.Input.Key.Is.Down(Keys.Right))
            {
                CurrentState.Speed.X = WALKSPEED;
            }
            else
            {
                CurrentState.Speed.X = 0;
            }

            if (CurrentState.Status == PlayerStatus.Ground && Game.Input.Key.Is.Press(Keys.Up))
            {
                CurrentState.Speed.Y = -JUMPSPEED;
                CurrentState.Status = PlayerStatus.Air;
            }
        }

        private void UpdatePhysics()
        {
            CurrentState.Speed += CurrentState.Velocity.Value * (float) Game.DeltaTime;


            CurrentState.Hitbox.Position += CurrentState.Speed * (float) Game.DeltaTime;

            if (CurrentState.Hitbox.Y > 400)
            {
                CurrentState.Status = PlayerStatus.Ground;
                CurrentState.Hitbox.Y = 400;
                CurrentState.Speed.Y = 0;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CurrentState.Hitbox.Position + TextureOffset, new Rectangle(212, 0, 29, 65),
                Color.White);
#if DEBUG
            DebugDraw(spriteBatch);
#endif
        }

#if DEBUG

        private float _wait = 5;
        private States _debugStates = new States();

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (_wait < 5)
            {
                _wait++;
            }
            else
            {
                _debugStates.Copy(LastState);
                _wait = 0;
            }

            string debug =
                $"Status: {_debugStates.Status} \n Speed: {_debugStates.Speed} \n Velocity: {_debugStates.Velocity.Value} \n Position: {_debugStates.Hitbox.Position} ";

            spriteBatch.DrawString(Game.DefaultFont, debug, Vector2.Zero, Color.White);
        }
#endif
    }
}