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

        public  float WalkSpeed = 250;
        public  float JumpSpeed = 400;
        public readonly Vector2 Gravity = Vector2.UnitY * 550 ;

        public  float DashTime = 0.2f;
        private float _dashCounter;
        public float DashForce = 650;
        

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

        public void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("spritesheet");
            Console.WriteLine(Texture.Bounds.ToString());
            CurrentState.Hitbox = new Transform(0, 0, 29, 65);
            TextureOffset = Vector2.Zero;
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
            {
                _dashCounter += Game.DeltaTime;

                if (_dashCounter >= DashTime)
                {
                    CurrentState.Status = PlayerStatus.Air;
                    CurrentState.Velocity.AddForce(ForceType.Gravity, Gravity);
                    CurrentState.Speed = Vector2.Zero;
                }
            }
            else if (Game.Input.Key.Is.Press(Keys.Space))
            {
                _dashCounter = 0;
               
                CurrentState.Status = PlayerStatus.Dash;
                
                if (CurrentState.Speed.X > 0)
                CurrentState.Speed = DashForce * Vector2.UnitX;
                else if (CurrentState.Speed.X < 0)
                    CurrentState.Speed = DashForce * -Vector2.UnitX;
                else
                {
                    CurrentState.Speed = DashForce * -Vector2.UnitY;
                }
                
                CurrentState.Velocity.RemoveForce(ForceType.Gravity);
            }
            else
            {
                if (Game.Input.Key.Is.Down(Keys.Left))
                {
                    CurrentState.Speed.X = -WalkSpeed;
                }
                else if (Game.Input.Key.Is.Down(Keys.Right))
                {
                    CurrentState.Speed.X = WalkSpeed;
                }
                else
                {
                    CurrentState.Speed.X = 0;
                }

                if (Game.Input.Key.Is.Press(Keys.Up) && CurrentState.Status == PlayerStatus.Ground)
                {
                    CurrentState.Speed.Y = -JumpSpeed;
                    CurrentState.Status = PlayerStatus.Air;
                }
            }
            
            

            if (CurrentState.Hitbox.Y + CurrentState.Speed.Y * Game.DeltaTime > 400 && CurrentState.Status != PlayerStatus.Dash)
            {
                CurrentState.Status = PlayerStatus.Ground;
                CurrentState.Hitbox.Y = 400;
                CurrentState.Speed.Y = 0;
            }
            
            CurrentState.Speed += CurrentState.Velocity.Value * Game.DeltaTime;

            CurrentState.Hitbox.Position += CurrentState.Speed * Game.DeltaTime;
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
                $"Status: {_debugStates.Status} \n Speed: {_debugStates.Speed} \n Velocity: {_debugStates.Velocity.Value} \n Position: {_debugStates.Hitbox.Position} \n DashTime: {_dashCounter}";

            spriteBatch.DrawString(Game.DefaultFont, debug, Vector2.Zero, Color.White);
        }
#endif
    }
}