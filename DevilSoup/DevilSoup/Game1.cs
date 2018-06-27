using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect eff;
        Effect CCPP;

        RenderTarget2D renderTarget;
        Skybox skybox;

        private Vector3 cameraPos, cauldronPos, czachaPos;
        Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Camera camera;
        private Asset cauldron;
        
        private Asset czacha;

        private Asset edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8;
        private DanceArea danceArea;
        private Player player;
        private Combo combo;
        private SkullRing skullRing;

        private Sprites sprites;

        //Matrix world = Matrix.CreateTranslation(0, 0, 0);
        //Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        //Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 128f / 64f, 0.1f, 1000000000f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = GlobalVariables.SCREEN_HEIGTH;
            graphics.PreferredBackBufferWidth = GlobalVariables.SCREEN_WIDTH;
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
            cameraPos = new Vector3(0, 130, 65);
            //cameraPos = new Vector3(100, 200, 70);
            //cameraPos = new Vector3(0, 0, 250);

            cauldronPos = new Vector3(0f, 0f, 0f);
            czachaPos = cauldronPos;

            camera = new Camera();
            camera.Position = cameraPos;
            camera.setWorldMatrix(camera.Position);
            camera.view = Matrix.CreateLookAt(camera.Position, cauldronPos, Vector3.UnitY);
            camera.Forward = camera.Position - cauldronPos;
            camera.Forward = Vector3.Normalize(camera.Forward);
            camera.Right = Vector3.Cross(new Vector3(0, 1f, 0), camera.Forward);
            camera.Up = Vector3.Cross(camera.Forward, camera.Right);

            camera.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); //Bardzo ważne! Głębokość na jaką patrzymy!
            IsFixedTimeStep = false; //False - update i draw są wywoływane po kolei, true - update jest wywoływane 60 razy/sek, draw może być porzucone w celu nadrobienia jeżeli gra działa wolno 
            cauldron = new Asset(Content, "Assets/Cauldron/hKociol/kociol",
                                            "Assets/Cauldron/hKociol/kociol1d_Albedo",
                                            "Assets/Cauldron/hKociol/kociol1d_Normal"
                                            , "Assets/Cauldron/hKociol/kociol1d_Specular",
                                            "Assets/Skybox/helll",
                                            camera
                                            );
            //cauldron.setShine(5f); //less = more shiny ^^

            cauldron.world = Matrix.CreateTranslation(cauldronPos);

            //NIE OTWIERAć
            {
                string color = "Assets/Cauldron/hKociol/kociol1d_Albedo";
                string normal = "Assets/Cauldron/hKociol/kociol1d_Normal";
                string specular = "Assets/Cauldron/hKociol/kociol1d_Specular";

                //ostrzegałem...
                edge1 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaLewyG", color, normal, specular, camera);
                edge2 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaSrodekG", color, normal, specular, camera);
                edge3 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaPrawyG", color, normal, specular, camera);
                edge4 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaLewyBok", color, normal, specular, camera);
                edge5 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaPrawyBok", color, normal, specular, camera);
                edge6 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaLewyD", color, normal, specular, camera);
                edge7 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaSrodekD", color, normal, specular, camera);
                edge8 = new Asset(Content, "Assets/Cauldron/hKociol/krawedzKotlaPrawyD", color, normal, specular, camera);

                edge1.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge2.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge3.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge4.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge5.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge6.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge7.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                edge8.world = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            }


            czacha = new Asset(Content, "Assets/test/vs",
                                        "Assets/test/vsc",
                                        "Assets/test/vsn",
                                        "Assets/test/vss",
                                         "Assets/Skybox/helll",
                                        camera);
            czacha.world = world;

            danceArea = new DanceArea(cauldron);
            danceArea.Initialize(Content, camera, GraphicsDevice);
            sprites = new Sprites();
            sprites.Initialize(Content, graphics);

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

            skybox = new Skybox(Content);
            skullRing = new SkullRing(Content, danceArea, camera);

            // CCPP.Parameters["colorMul"].SetValue(new Vector3(.6f, 1f, .7f));
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
            sprites.LoadContent(this, spriteBatch);
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

            sprites.Update(gameTime, danceArea);
            //  cauldron.setSpecularColor(new Vector4(1, 0, 0, 1));

            CCPP.Parameters["timer"].SetValue((float)(gameTime.TotalGameTime.TotalMilliseconds / 1000.0 * 22 * 3.14159 * danceArea.getHeatIntensity()));
            CCPP.Parameters["amp"].SetValue((float)danceArea.getHeatAmp());

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

            GraphicsDevice.Clear(Color.DimGray);

            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(camera.view, camera.projection, camera.Position);

            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;

            cauldron.SimpleDraw(camera.view, camera.projection, danceArea.currentColor);

            {
                if (danceArea.combo.ifComboActive)
                {
                    edge1.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.UpperLeft));
                    edge2.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.Up));
                    edge3.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.UpperRight));
                    edge4.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.Left));
                    edge5.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.Right));
                    edge6.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.BottomLeft));
                    edge7.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.Bottom));
                    edge8.SimpleDraw(camera.view, camera.projection, danceArea.combo.getColor((int)SingleAreasIndexes.BottomRight));
                }
            }

            // czacha.SimpleDraw(camera.view, camera.projection);
            // czacha.setShine(1f);
            danceArea.Draw(gameTime);
            skullRing.render(camera);
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            CCPP.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            spriteBatch.Begin();

            combo.Draw(gameTime);

            sprites.Draw(danceArea);

            //danceArea.DrawFuelBar(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            base.Draw(gameTime);
        }
    }
}
