using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS437_Pong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PongGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PongModel model;
        PongController controller;

        Texture2D colorStrip;
        Rectangle whiteColor;

        Rectangle player1Rect = new Rectangle();
        Rectangle player2Rect = new Rectangle();
        Rectangle ball = new Rectangle();

        public PongGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            model = new PongModel();
            controller = new PongController(model);

            colorStrip = Content.Load<Texture2D>("colorStrip");
            whiteColor = new Rectangle(1, 0, 0, 0);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Disable vsync
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            // TODO: use this.Content to load your game content here
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

            controller.updateModel(gameTime);
            model.getPlayer1Rect(GraphicsDevice.Viewport, ref player1Rect);
            model.getPlayer2Rect(GraphicsDevice.Viewport, ref player2Rect);
            model.getBallRect(GraphicsDevice.Viewport, ref ball);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            spriteBatch.Begin();
            spriteBatch.Draw(colorStrip, player1Rect, whiteColor, Color.White);
            spriteBatch.Draw(colorStrip, player2Rect, whiteColor, Color.Red);
            spriteBatch.Draw(colorStrip, ball, whiteColor, Color.Green);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
