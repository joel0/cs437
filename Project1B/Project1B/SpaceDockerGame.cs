using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1B
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SpaceDockerGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model cubeModel;

        // Copied from documentation
        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        BasicEffect basicEffect;

        public SpaceDockerGame()
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
            // TODO: Add your initialization logic here
            float tilt = MathHelper.ToRadians(0);  // 0 degree angle
                                                   // Use the world matrix to tilt the cube along x and y axes.
            worldMatrix = Matrix.CreateRotationX(tilt) * Matrix.CreateRotationY(tilt);
            viewMatrix = Matrix.CreateLookAt(new Vector3(200, 500, 600), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.Width /
                (float)GraphicsDevice.Viewport.Height,
                1.0f, 10000.0f);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            basicEffect.EnableDefaultLighting();

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
            cubeModel = Content.Load<Model>("cube2");

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[cubeModel.Bones.Count];
            cubeModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in cubeModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        worldMatrix // Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(Vector3.Zero); // modelPosition);
                    effect.View = viewMatrix; // Matrix.CreateLookAt(cameraPosition,
                                              //Vector3.Zero, Vector3.Up);
                    effect.Projection = projectionMatrix; /* Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);*/
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }


            base.Draw(gameTime);
        }
    }
}
