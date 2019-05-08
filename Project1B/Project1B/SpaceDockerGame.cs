using BEPUphysics;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1B {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SpaceDockerGame : Game {
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        public bool IsGameOver = false;
        public bool IsWin = false;

        public Space Space { get; private set; }

        SpriteFont mFont;
        SpriteFont mFontHUD;
        Texture2D mBlueTexture;

        Ship mShip;

        Matrix mTestViewMatrix;
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix OrientationMatrix {
            get {
                return mShip.OrientationMatrix;
            }
        }

        public SpaceDockerGame() {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Space = new Space();
            mShip = new Ship(this, Vector3.Backward * 2000);
            //Components.Add(new Asteroid(this, Vector3.Zero, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Forward * 2000, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Forward * 4000, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Forward * 6000, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Forward * 8000, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Left * 1000, Vector3.Right * 200, Vector3.UnitX));
            //Components.Add(new Mothership(this, Vector3.Forward * 12000));
            Components.Add(new Powerup(this, Vector3.Zero));
            Components.Add(mShip);
            Components.Add(new Skybox(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.Width /
                GraphicsDevice.Viewport.Height,
                1.0f, 1000000000.0f);

            mTestViewMatrix = Matrix.CreateLookAt(new Vector3(4000, 4000, 4000), Vector3.Zero, Vector3.Up);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mFont = Content.Load<SpriteFont>("Font");
            mFontHUD = Content.Load<SpriteFont>("FontHUD");
            mBlueTexture = Content.Load<Texture2D>("BlueTexture");
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
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Space.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            ViewMatrix = Matrix.Invert(Matrix.CreateTranslation(-400, 400, 1800) * mShip.WorldMatrix);
            //ViewMatrix = mTestViewMatrix;

            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);

            mSpriteBatch.Begin();
            if (IsGameOver) {
                if (IsWin) {
                    mSpriteBatch.DrawString(mFont, "You Win!!", new Vector2(250, 200), Color.White);
                } else {
                    mSpriteBatch.DrawString(mFont, "Game Over", new Vector2(250, 200), Color.White);
                }
            }
            // Health
            mSpriteBatch.DrawString(mFontHUD, "Health:", new Vector2(300, 10), Color.SkyBlue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(350, 10, 100, 20), Color.Blue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(350, 10, mShip.Health, 20), Color.White);
            // Fuel
            mSpriteBatch.DrawString(mFontHUD, "Fuel:", new Vector2(10, 10), Color.SkyBlue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(50, 10, 100, 20), Color.Blue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(50, 10, (int)(100 * mShip.Fuel / 100), 20), Color.White);
            // Torpedos
            mSpriteBatch.DrawString(mFontHUD, "Torpedoes:", new Vector2(600, 10), Color.SkyBlue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(680, 10, 100, 20), Color.Blue);
            mSpriteBatch.Draw(mBlueTexture, new Rectangle(680, 10, 100 * mShip.TorpedoesAvail / Ship.TORPEDO_MAX_AVAIL, 20), Color.White);
            // Torpedo aim
            mShip.DrawHUD(mSpriteBatch, mBlueTexture);
            mSpriteBatch.End();
        }
    }
}
