using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jelly
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        private PrimitiveBatch _primitiveBatch;
        private Jelly _jelly;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState oldMouse;

        public static Viewport Screen { get; private set; }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Viewport screen = graphics.GraphicsDevice.Viewport;
            _jelly = new Jelly(new Vector2(screen.Width / 2, 0), 100, 5);
            _jelly.Floor = new Rectangle(0, (int)(screen.Height * 0.75f), screen.Width, (int)(screen.Height * 0.25f));
            oldMouse = Mouse.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Screen = GraphicsDevice.Viewport;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _jelly.Update(gameTime);

            #region Sur click effectue un splash
            MouseState mouse = Mouse.GetState();
            
            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                Vector2 mousePos = mouse.Position.ToVector2();
                _jelly.Splash(mousePos);
            }
            
            oldMouse = mouse;
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawRectangle(_jelly.Floor, Color.Red);
            spriteBatch.End();

            _primitiveBatch.Begin(PrimitiveType.TriangleList);
            _jelly.Draw(_primitiveBatch, gameTime);
            _primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}
