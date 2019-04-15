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
        Model mShipModel;
        Model mMothershipModel;

        // Copied from documentation
        Matrix mTestViewMatrix;
        Matrix mProjectionMatrix;
        Matrix mViewMatrix;
        BasicEffect mBasicEffect;

        Matrix mShipLocationMatrix;
        Matrix mShipOrientationMatrix;

        public SpaceDockerGame()
        {
            mGraphics = new GraphicsDeviceManager(this);
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
            mProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.Width /
                GraphicsDevice.Viewport.Height,
                1.0f, 10000.0f);

            mBasicEffect = new BasicEffect(mGraphics.GraphicsDevice)
            {
                //World = mWorldMatrix,
                View = mTestViewMatrix,
                Projection = mProjectionMatrix
            };

            mTestViewMatrix = Matrix.CreateLookAt(new Vector3(4000, 4000, 4000), Vector3.Zero, Vector3.Up);
            mShipLocationMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 1000));
            mShipOrientationMatrix = mShipLocationMatrix;

            mBasicEffect.EnableDefaultLighting();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mShipModel = Content.Load<Model>("Models/p1_wedge");
            mMothershipModel = Content.Load<Model>("Models/mothership");
            // TODO load asteroid model
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
            float yaw = MathHelper.ToRadians(0);
            float pitch = MathHelper.ToRadians(0);
            float roll = MathHelper.ToRadians(0);
            Vector3 location = Vector3.Zero;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                yaw -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                pitch += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                roll -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;

                location.Z += (GamePad.GetState(PlayerIndex.One).Triggers.Left - GamePad.GetState(PlayerIndex.One).Triggers.Right) * 50f;
            }

            mShipOrientationMatrix = Matrix.CreateFromYawPitchRoll(yaw * 0.1f, pitch * 0.1f, roll * 0.1f) * mShipOrientationMatrix;
            mShipLocationMatrix = Matrix.Invert(mShipOrientationMatrix) * Matrix.CreateTranslation(location) * mShipOrientationMatrix * mShipLocationMatrix;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Matrix shipViewMatrix = mShipOrientationMatrix * mShipLocationMatrix;
            mViewMatrix = Matrix.Invert(Matrix.CreateTranslation(0, 700, 3000) * shipViewMatrix);
            //mViewMatrix = mTestViewMatrix;

            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            DrawShip(shipViewMatrix);
            DrawMothership();

            base.Draw(gameTime);
        }

        private void DrawShip(Matrix shipViewMatrix)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mShipModel.Bones.Count];
            mShipModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mShipModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * shipViewMatrix;
                    effect.View = mViewMatrix;
                    effect.Projection = mProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        private void DrawMothership()
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mMothershipModel.Bones.Count];
            mMothershipModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mMothershipModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index];
                    effect.View = mViewMatrix;
                    effect.Projection = mProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
