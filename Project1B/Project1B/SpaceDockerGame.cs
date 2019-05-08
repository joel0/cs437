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
        GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        public bool IsGameOver = false;

        public Space Space { get; private set; }

        SpriteFont mFont;
        Ship mShip;
        Model mMothershipModel;
        Model mAsteroidModel;

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
            Components.Add(new Asteroid(this, Vector3.Zero, Vector3.Zero, Vector3.Zero));
            //Components.Add(new Asteroid(this, Vector3.Left * 1000, Vector3.Right * 200, Vector3.UnitX));
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
            mMothershipModel = Content.Load<Model>("Models/mothership");
            mAsteroidModel = Content.Load<Model>("Models/astroid");
            mFont = Content.Load<SpriteFont>("Font");
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

            //DrawMothership();

            base.Draw(gameTime);

            mSpriteBatch.Begin();
            if (IsGameOver) {
                mSpriteBatch.DrawString(mFont, "Game Over", new Vector2(250, 200), Color.White);
            }
            mSpriteBatch.End();
        }

        private void DrawMothership() {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mMothershipModel.Bones.Count];
            mMothershipModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mMothershipModel.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index];
                    effect.View = ViewMatrix;
                    effect.Projection = ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
