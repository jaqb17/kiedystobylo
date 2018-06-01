using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using WiimoteLib;


//http://www.miszalok.de/C_3D_XNA/C4_Controller/XNAC4_Wiimote_e.htm
namespace DevilSoup
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect eff;

        private Vector3 cameraPos, cauldronPos;
        private Camera camera;
        private Asset cauldron;
        private DanceArea danceArea;
        private Player player;
        private Combo combo;
        //private Asset animTemplate;
        //private ModelsInstancesClass models;
        private Sprites sprites;

        //private BBRectangle billboardRect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 1280;
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
            //models = new ModelsInstancesClass();
            cameraPos = new Vector3(0, 110, 40);
            //cameraPos = new Vector3(0f, 0f, 4f);
            cauldronPos = new Vector3(0f, 0f, 0f);
            camera = new Camera();
            camera.setWorldMatrix(cameraPos);
            camera.view = Matrix.CreateLookAt(cameraPos, cauldronPos, Vector3.UnitY);
            //camera.view = Matrix.CreateLookAt(cameraPos, cauldronPos, Vector3.UnitY);
            camera.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); //Bardzo ważne! Głębokość na jaką patrzymy!
            IsFixedTimeStep = false; //False - update i draw są wywoływane po kolei, true - update jest wywoływane 60 razy/sek, draw może być porzucone w celu nadrobienia jeżeli gra działa wolno 
            cauldron = new Asset();
            cauldron.loadModel(Content, "Assets\\Cauldron\\RictuCauldron");
            cauldron.world = Matrix.CreateTranslation(cauldronPos);

            danceArea = new DanceArea(cauldron);
            danceArea.Initialize(Content, camera);
            sprites = new Sprites();
            sprites.Initialize(Content);

            player = Player.getPlayer();
            combo = Combo.createCombo();
            combo.Initialize(graphics);
            danceArea.FuelBarInitialize(Content);

            /*animTemplate = new Asset();
            animTemplate.loadModel(Content, "Assets\\TestAnim\\muchomorStadnyAtak");
            animTemplate.world = Matrix.CreateTranslation(cauldronPos);
            animTemplate.scaleAset(0.5f);
            animTemplate.cameraPos = camera.Position;
            */
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
            combo.LoadContent(spriteBatch);
            sprites.LoadContent(spriteBatch);
            eff = new BasicEffect(GraphicsDevice);

            //billboardRect = new BBRectangle("Assets\\OtherTextures\\slashTexture", Content, new Vector3(0, 0, 0), graphics.GraphicsDevice);

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
        /// 
        protected override void Update(GameTime gameTime)
        {
            //GamePadState xPadState = GamePad.GetState(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.Two).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            danceArea.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            cauldron.Draw(gameTime, camera.view, camera.projection, new Vector3((float)danceArea.heatValue, 1f, 1f));

            //animTemplate.Draw(gameTime, camera.view, camera.projection);

            spriteBatch.Begin();
            //danceArea.DrawFuelBar(spriteBatch);
            danceArea.Draw(gameTime);
            combo.Draw(gameTime);

            sprites.Draw(danceArea);

            //danceArea.DrawFuelBar(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //billboardRect.DrawRect(eff, graphics.GraphicsDevice, camera.view, camera.projection, camera.world);



            base.Draw(gameTime);
        }


    }
}
