using EvilEngine.Lab;
using InputStateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvilEngine.Core
{
    public class GameCore : Game
    {
        public static GameCore Instance { get; private set; }
        public static float DeltaTime { get; private set; }
        public static AssetManager Assets = new AssetManager();

        private readonly GraphicsDeviceManager _graphics;
        private readonly Player _player;

        public static readonly InputManager Input = new InputManager();

        private SpriteBatch _spriteBatch;

        public SpriteFont DefaultFont;

        
        protected GameCore()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";

            _player = new Player();
             
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            IsFixedTimeStep = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultFont = Content.Load<SpriteFont>("debug");
            _player.LoadContent();
            
            _player.AfterLoad();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            DeltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update();
            _player.Update();
            _player.AfterUpdate();
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