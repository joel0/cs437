using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        SpriteFont scoreFont;
        Rectangle whiteColor;

        string score;
        Vector2 scoreLocation = new Vector2();
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

            colorStrip = Content.Load<Texture2D>("colorStrip");
            scoreFont = Content.Load<SpriteFont>("scoreFont");
            controller.soundPaddle = Content.Load<SoundEffect>("paddle");
            controller.soundWin = Content.Load<SoundEffect>("win");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() { }

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
            score = string.Format("{0}:{1}", model.player1Score, model.player2Score);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            scoreLocation.X = (float)GraphicsDevice.Viewport.Width / 2 - 40;
            scoreLocation.Y = (float)GraphicsDevice.Viewport.Height / 30;

            spriteBatch.Begin();
            spriteBatch.Draw(colorStrip, player1Rect, whiteColor, Color.White);
            spriteBatch.Draw(colorStrip, player2Rect, whiteColor, Color.Red);
            spriteBatch.Draw(colorStrip, ball, whiteColor, Color.Green);
            spriteBatch.DrawString(scoreFont, score, scoreLocation, Color.Lime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
