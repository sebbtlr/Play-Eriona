using System;
using EvilEngine.Core;
using EvilEngine.Graphics;
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
        public Vector2 Position;
        public Transform Hitbox;
        public Vector2 Speed;
        public PlayerStatus Status;
        public ForceList Velocity;

        public void Copy(States other)
        {
            Position = other.Position;
            Status = other.Status;
            Hitbox = new Transform(other.Hitbox);
            Speed = other.Speed;
            other.Velocity.CopyIn(out Velocity);
        }
    }
[Flags]
    public enum PlayerAnimation
    {
        Walk,
        Jump,
        Fall,
        Idle,
        Breath
    }

    // TODO: Jump Height
    // Apply an intial jump force and then iterate another force until the end of the timer
    public class Player
    {
        public const float WALK_SPEED = 250;
        public const float JUMP_INITIAL = -600;
        public const float JUMP_FORCE = 1.05f;
        public const float JUMP_TIME_MAX = 0.35f;

        public const float DASH_TIME = 0.2f;
        public const float DASH_FORCE = 650;
        public readonly States CurrentState;

        public readonly Vector2 Gravity = new Vector2(0, 550);
        public readonly States LastState;
        private bool _canMove = true;
        private float _dashCounter;
        private float _jumpTimeCounter;

        public Vector2 Scale;
        public readonly AnimationManager Animation;

        public Player()
        {

            CurrentState = new States
            {
                Position = Vector2.Zero,
                Status = PlayerStatus.Air,
                Hitbox = new Transform(),
                Speed = Vector2.Zero,
                Velocity = new ForceList()
            };

            LastState = new States();
            CurrentState.Velocity.CopyIn(out LastState.Velocity);

            Scale = Vector2.One / 5;
            

            CurrentState.Velocity.AddForce(ForceType.Gravity, Gravity);
            
            Animation = new AnimationManager();
            
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__000");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__001");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__002");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__003");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__004");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__005");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__006");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__007");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__008");
            GameCore.Assets.LoadAndAdd<Texture2D>("ninja/Idle__009");
            
            LoadAnimations();
        }

        public void LoadAnimations()
        {

            Animation.AddAnimation(PlayerAnimation.Idle.ToString(), true, new[]
            {
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__000"), new Transform(0, 0, 232, 439), null,
                    -new Vector2(116, 439))
            } );
            
            Animation.AddAnimation(PlayerAnimation.Breath.ToString(),0.1f, new []
            {
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__000"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__001"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__002"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__003"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__004"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__005"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__006"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__007"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__008"), new Transform(0,0,232,439), null, -new Vector2(116, 439)),
                new Sprite(GameCore.Assets.Get<Texture2D>("ninja/Idle__009"), new Transform(0,0,232,439), null, -new Vector2(116, 439))
            } ); 
            
            Animation.ChangeAnimation(PlayerAnimation.Breath.ToString());
        }

        public void AfterLoad()
        {
            LoadAnimations();
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
            else if (GameCore.Input.Key.Is.Press(Keys.Space))
                StartDash();

            if (_canMove)
            {
                InputUpdate();
                JumpUpdate();
            }

            if (CurrentState.Position.Y + CurrentState.Speed.Y * GameCore.DeltaTime > 590 &&
                CurrentState.Status != PlayerStatus.Dash)
            {
                CurrentState.Status = PlayerStatus.Ground;
                CurrentState.Position.Y = 590;
                CurrentState.Speed.Y = 0;
            }
            
            UpdatePhysics();
        }

        private void StartDash()
        {
            _dashCounter = 0;

            CurrentState.Status = PlayerStatus.Dash;

            if (GameCore.Input.Key.Is.Down(Keys.Right))
                CurrentState.Speed = DASH_FORCE * Vector2.UnitX;
            else if (GameCore.Input.Key.Is.Down(Keys.Left))
                CurrentState.Speed = DASH_FORCE * -Vector2.UnitX;
            else
                CurrentState.Speed = DASH_FORCE * -Vector2.UnitY;

            if (GameCore.Input.Key.Is.Down(Keys.Up))
                CurrentState.Speed.Y = -DASH_FORCE;
            else if (GameCore.Input.Key.Is.Down(Keys.Down))
                CurrentState.Speed.Y = DASH_FORCE;

            CurrentState.Velocity.RemoveForce(ForceType.Gravity);

            _canMove = false;
        }

        private void DashUpdate()
        {
            _dashCounter += GameCore.DeltaTime;

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
            if (GameCore.Input.Key.Is.Down(Keys.Left))
                CurrentState.Speed.X = -WALK_SPEED;
            else if (GameCore.Input.Key.Is.Down(Keys.Right))
                CurrentState.Speed.X = WALK_SPEED;
            else
                CurrentState.Speed.X = 0;
        }

        public void JumpUpdate()
        {
            if (GameCore.Input.Key.Is.Press(Keys.Up) && CurrentState.Status == PlayerStatus.Ground)
            {
                CurrentState.Status = PlayerStatus.Air;
                _jumpTimeCounter = 0;
            }
            else if (_jumpTimeCounter >= 0.0f && _jumpTimeCounter < JUMP_TIME_MAX)
            {
                CurrentState.Speed.Y = JUMP_INITIAL * (1.0f - (float)Math.Pow(_jumpTimeCounter / JUMP_TIME_MAX, JUMP_FORCE));
                _jumpTimeCounter += GameCore.DeltaTime;
            }
            else
                _jumpTimeCounter = JUMP_TIME_MAX;
        }

        public void UpdatePhysics()
        {
            CurrentState.Speed += CurrentState.Velocity.Value * GameCore.DeltaTime;

            CurrentState.Position += CurrentState.Speed * GameCore.DeltaTime;
        }

        public void AfterUpdate()
        {
            CurrentState.Hitbox = Animation.Hitbox;
            CurrentState.Hitbox.Size *= Scale;
            
            Animation.Update();
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Animation.Texture, CurrentState.Position + Animation.TextureOffset * Scale, Animation.TextureClip,
                Animation.TextureColorFilter, 0.0f, Vector2.Zero, Scale,SpriteEffects.None, 0);
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
                $"Status: {_debugStates.Status} \n " +
                $"Speed: {_debugStates.Speed} \n " +
                $"Velocity: {_debugStates.Velocity.Value} \n " +
                $"Position: {_debugStates.Position} \n " +
                $"DashTime: {_dashCounter} \n " +
                $"JumpCounter: {_jumpTimeCounter} \n " +
                $"Mouse : X =>{GameCore.Input.Mouse.Is.X} / Y => {GameCore.Input.Mouse.Is.Y}";

            spriteBatch.DrawString(GameCore.Instance.DefaultFont, debug, Vector2.Zero, Color.White);
        }
#endif
    }
}