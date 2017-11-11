using EvilEngine.Lab;
using InputStateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvilEngine.Core
{
    public class GameCore : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Player _player;

        public readonly InputManager Input = new InputManager();

        private SpriteBatch _spriteBatch;

        public double DeltaTime;

        protected GameCore()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";

            _player = new Player(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _player.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update();
            _player.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _player.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}