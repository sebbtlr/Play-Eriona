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
        public Transform Hitbox;
        public Vector2 Speed;
        public PlayerStatus Status;
        public ForceList Velocity;

        public void Copy(States other)
        {
            Status = other.Status;
            Hitbox = new Transform(other.Hitbox);
            Speed = other.Speed;
            other.Velocity.CopyIn(out Velocity);
        }
    }

    // TODO: Jump Height
    // Apply an intial jump force and then iterate another force until the end of the timer
    public class Player
    {
        public const float WALK_SPEED = 250;
        public const float JUMP_INITIAL = -350;
        public const float JUMP_FORCE = -100;
        public const float JUMP_TIME_MAX = 0.5f;

        public const float DASH_TIME = 0.2f;
        public const float DASH_FORCE = 650;
        public readonly States CurrentState;

        public readonly GameCore Game;
        public readonly Vector2 Gravity = new Vector2(0, 550);
        public readonly States LastState;
        private bool _canMove = true;
        private float _dashCounter;
        private float _jumpTimeCounter;

        public Vector2 Scale;
        public Texture2D Texture;
        public Vector2 TextureOffset;

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

            CurrentState.Velocity.AddForce(ForceType.Gravity, Gravity);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("spritesheet");
            Console.WriteLine(Texture.Bounds.ToString());
            CurrentState.Hitbox = new Transform(0, 0, 29, 65);
            TextureOffset = Vector2.Zero;
        }

        public void AfterLoad()
        {
        }

        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public void Update()
        {
            LastState.Copy(CurrentState);

            if (CurrentState.Status == PlayerStatus.Dash)
                DashUpdate();
            else if (Game.Input.Key.Is.Press(Keys.Space))
                StartDash();

            if (_canMove)
                InputUpdate();

#if DEBUG
            if (CurrentState.Hitbox.Y + CurrentState.Speed.Y * Game.DeltaTime > 400 &&
                CurrentState.Status != PlayerStatus.Dash)
            {
                CurrentState.Status = PlayerStatus.Ground;
                CurrentState.Hitbox.Y = 400;
                CurrentState.Speed.Y = 0;
            }
#endif
            CurrentState.Speed += CurrentState.Velocity.Value * Game.DeltaTime;

            CurrentState.Hitbox.Position += CurrentState.Speed * Game.DeltaTime;
        }

        private void StartDash()
        {
            _dashCounter = 0;

            CurrentState.Status = PlayerStatus.Dash;

            if (Game.Input.Key.Is.Down(Keys.Right))
                CurrentState.Speed = DASH_FORCE * Vector2.UnitX;
            else if (Game.Input.Key.Is.Down(Keys.Left))
                CurrentState.Speed = DASH_FORCE * -Vector2.UnitX;
            else
                CurrentState.Speed = DASH_FORCE * -Vector2.UnitY;

            if (Game.Input.Key.Is.Down(Keys.Up))
                CurrentState.Speed.Y = -DASH_FORCE;
            else if (Game.Input.Key.Is.Down(Keys.Down))
                CurrentState.Speed.Y = DASH_FORCE;

            CurrentState.Velocity.RemoveForce(ForceType.Gravity);

            _canMove = false;
        }

        private void DashUpdate()
        {
            _dashCounter += Game.DeltaTime;

            if (_dashCounter >= DASH_TIME)
            {
                CurrentState.Status = PlayerStatus.Air;
                CurrentState.Velocity.AddForce(ForceType.Gravity, Gravity);
                CurrentState.Speed = Vector2.Zero;
                _canMove = true;
            }
        }

        private void InputUpdate()
        {
            if (Game.Input.Key.Is.Down(Keys.Left))
                CurrentState.Speed.X = -WALK_SPEED;
            else if (Game.Input.Key.Is.Down(Keys.Right))
                CurrentState.Speed.X = WALK_SPEED;
            else
                CurrentState.Speed.X = 0;

            if (Game.Input.Key.Is.Press(Keys.Up) && CurrentState.Status == PlayerStatus.Ground)
            {
                CurrentState.Speed.Y = JUMP_FORCE;
                CurrentState.Status = PlayerStatus.Air;
                _jumpTimeCounter = 0;
            }
            else if (Game.Input.Key.Was.Down(Keys.Up) && Game.Input.Key.Is.Down(Keys.Up) &&
                     _jumpTimeCounter < JUMP_TIME_MAX)
            {
                CurrentState.Speed.Y = MathHelper.Lerp(JUMP_INITIAL, JUMP_FORCE, 0.1f);
                _jumpTimeCounter += Game.DeltaTime;
            }

            if (Game.Input.Key.Is.Release(Keys.Up))
                _jumpTimeCounter = JUMP_TIME_MAX;
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
        private readonly States _debugStates = new States();

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

            var debug =
                $"Status: {_debugStates.Status} \n Speed: {_debugStates.Speed} \n Velocity: {_debugStates.Velocity.Value} \n Position: {_debugStates.Hitbox.Position} \n DashTime: {_dashCounter} \n JumpCounter: {_jumpTimeCounter}";

            spriteBatch.DrawString(Game.DefaultFont, debug, Vector2.Zero, Color.White);
        }
#endif
    }
}