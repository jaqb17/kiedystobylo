using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private int width = 1280;
        private int height = 640;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect eff;
        Effect CCPP;

        RenderTarget2D renderTarget;
        Texture2D screenTexture;

        private Vector3 cameraPos, cauldronPos, czachaPos;

        private Camera camera;
        private Asset cauldron;
        private Asset czacha;
        private DanceArea danceArea;
        private Player player;
        private Combo combo;

        private Sprites sprites;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 128f / 64f, 0.1f, 1000000000f);



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
            cameraPos = new Vector3(0, 130, 65);


            cauldronPos = new Vector3(0f, 0f, 0f);
            czachaPos = cauldronPos;

            camera = new Camera();
            camera.Position = cameraPos;
            camera.setWorldMatrix(camera.Position);
            camera.view = Matrix.CreateLookAt(camera.Position, cauldronPos, Vector3.UnitY);


            camera.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); //Bardzo ważne! Głębokość na jaką patrzymy!
            IsFixedTimeStep = false; //False - update i draw są wywoływane po kolei, true - update jest wywoływane 60 razy/sek, draw może być porzucone w celu nadrobienia jeżeli gra działa wolno 
            cauldron = new Asset(Content, "Assets/Cauldron/hKociol/kociol",
                                            "Assets/Cauldron/hKociol/kociol1d_Albedo",
                                            "Assets/Cauldron/hKociol/kociol1d_Normal"
                                            , "Assets/Cauldron/hKociol/kociol1d_Specular",
                                            camera
                                            );
            cauldron.setShine(5f); //less = more shiny ^^
            
            
            cauldron.world = Matrix.CreateTranslation(cauldronPos);

            czacha = new Asset(Content, "Assets/test/vs",
                                        "Assets/test/vsc",
                                        "Assets/test/vsn",
                                        "Assets/test/vss",
                                        camera);

            danceArea = new DanceArea(cauldron);
            danceArea.Initialize(Content, camera);
            sprites = new Sprites();
            sprites.Initialize(Content);

            player = Player.getPlayer();
            combo = Combo.createCombo();
            combo.Initialize(graphics);
            danceArea.FuelBarInitialize(Content);

            renderTarget = new RenderTarget2D(GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            CCPP = Content.Load<Effect>("Assets/Effects/CC");

            //CCPP.Parameters["colorMul"].SetValue(new Vector3(1.2f, .8f, 1));

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
          //  cauldron.setSpecularColor(new Vector4(1, 0, 0, 1));
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            //  world = Matrix.CreateRotationY(-1f * (MathHelper.Pi / 180f)) * world;
            // czacha.world = world;
            GraphicsDevice.Clear(Color.DimGray);
            //cauldron.SimpleDraw(camera.view,camera.projection, new Vector3((float)danceArea.heatValue/100f, 0f, 0f));
           // cauldron.world = world;
            cauldron.SimpleDraw(camera.view, camera.projection, danceArea.currentColor);
            // czacha.SimpleDraw(camera.view, camera.projection);
            //animTemplate.Draw(gameTime, camera.view, camera.projection);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Drop the render target
            spriteBatch.Begin();

          

            //danceArea.DrawFuelBar(spriteBatch);
            danceArea.Draw(gameTime);
            combo.Draw(gameTime);

            sprites.Draw(danceArea);

            //danceArea.DrawFuelBar(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            

            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            CCPP.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);

     
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //billboardRect.DrawRect(eff, graphics.GraphicsDevice, camera.view, camera.projection, camera.world);



            base.Draw(gameTime);
        }


    }
}
