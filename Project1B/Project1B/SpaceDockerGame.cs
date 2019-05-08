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
        
        public Space Space { get; private set; }
        Model mShipModel;
        Model mMothershipModel;
        Model mAsteroidModel;

        Matrix mTestViewMatrix;
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix ViewMatrix { get; private set; }

        Matrix mShipLocationMatrix;
        Matrix mShipOrientationMatrix;

        private BoundingSphere ShipBoundingSphere {
            get {
                BoundingSphere sphere = mShipModel.Meshes[0].BoundingSphere.Transform(mShipOrientationMatrix * mShipLocationMatrix);
                sphere.Radius *= 0.5f; // Hand tuned parameter to match the ship model.
                return sphere;
            }
        }

        public SpaceDockerGame()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Space = new Space();
            Components.Add(new Asteroid(this, Vector3.Zero, Vector3.Zero, Vector3.Zero));
            Components.Add(new Asteroid(this, Vector3.Up * 3000, Vector3.Down * 200, Vector3.UnitX));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.Width /
                GraphicsDevice.Viewport.Height,
                1.0f, 10000.0f);

            mTestViewMatrix = Matrix.CreateLookAt(new Vector3(4000, 4000, 4000), Vector3.Zero, Vector3.Up);
            mShipLocationMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 1000));
            mShipOrientationMatrix = mShipLocationMatrix;

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
            mAsteroidModel = Content.Load<Model>("Models/astroid");
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
            // PROCESS INPUT
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

            // UPDATE OBJECTS
            mShipOrientationMatrix = Matrix.CreateFromYawPitchRoll(yaw * 0.1f, pitch * 0.1f, roll * 0.1f) * mShipOrientationMatrix;
            mShipLocationMatrix = Matrix.Invert(mShipOrientationMatrix) * Matrix.CreateTranslation(location) * mShipOrientationMatrix * mShipLocationMatrix;

            Space.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Matrix shipWorldMatrix = mShipOrientationMatrix * mShipLocationMatrix;
            ViewMatrix = Matrix.Invert(Matrix.CreateTranslation(-400, 400, 1800) * shipWorldMatrix);
            //mViewMatrix = mTestViewMatrix;

            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            DrawShip(shipWorldMatrix);
            //DrawMothership();

            base.Draw(gameTime);
        }

        private void DrawShip(Matrix shipWorldMatrix)
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
                    effect.World = transforms[mesh.ParentBone.Index] * shipWorldMatrix;
                    effect.View = ViewMatrix;
                    effect.Projection = ProjectionMatrix;
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
                    effect.View = ViewMatrix;
                    effect.Projection = ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
