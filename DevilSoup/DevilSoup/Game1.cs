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
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect eff;
        private SpriteFont font;
        private Vector3 cauldronPos;
        private Camera camera;
        private Asset cauldron;
        private Pad gamepad;
        private DanceArea danceArea;
        private Player player;
        private Combo combo;
        private Asset animTemplate;
        private ThreadSafeContentManager _contentManager;
        private AnimatedModelShader _animatedModelShader;

        //private BBRectangle billboardRect;
        int timeDelayed = 0;
        bool availableToChange = true;
        int createSoulTimeDelay = 0;
        bool ifCreateSoul = true;
        bool ifCheckAccelerometer = true;
        int accelTimeDelay = 0;

        private bool started = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;

            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 1280;
            
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;

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
            //cameraPos = new Vector3(0f, 0f, 4f);
            cauldronPos = new Vector3(0f, 0f, 0f);
            camera = new Camera();
            camera.setWorldMatrix(camera.Position);
            camera.view = Matrix.CreateLookAt(camera.Position, cauldronPos, Vector3.UnitY);
            //camera.view = Matrix.CreateLookAt(cameraPos, cauldronPos, Vector3.UnitY);
            camera.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); //Bardzo ważne! Głębokość na jaką patrzymy!
            IsFixedTimeStep = false; //False - update i draw są wywoływane po kolei, true - update jest wywoływane 60 razy/sek, draw może być porzucone w celu nadrobienia jeżeli gra działa wolno 
            cauldron = new Asset();
            cauldron.loadModel(Content, "Assets\\Cauldron\\RictuCauldron");
            cauldron.world = Matrix.CreateTranslation(cauldronPos);
            cauldron.cameraPos = camera.Position;

            gamepad = new Pad();
            
            danceArea = new DanceArea(cauldron);
            font = Content.Load<SpriteFont>("HP");

            player = Player.getPlayer();
            combo = Combo.createCombo();
            danceArea.FuelBarInitialize(Content);

            animTemplate = new Asset();

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
            eff = new BasicEffect(GraphicsDevice);
            _contentManager = new ThreadSafeContentManager(Content.ServiceProvider) { RootDirectory = "Content" };

            _animatedModelShader = new AnimatedModelShader();
            _animatedModelShader.Initialize(graphics.GraphicsDevice);
            _animatedModelShader.Load(_contentManager, "ShaderModules/AnimatedModelShader/AnimatedModelShader");

            animTemplate.LoadContentFile<Asset>(_contentManager, "Assets/TestAnim/muchomorStadnyAtak.fbx");
            animTemplate.world = Matrix.CreateTranslation(cauldronPos);
            animTemplate.scaleAset(0.2f);
            animTemplate.cameraPos = camera.Position;
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
            int keyPressed;
            danceArea.currentKeyPressed = Keyboard.GetState();
            if (gamepad.USBMatt != null)
            {
                keyPressed = gamepad.getKeyState();
            }
            else
            {
                keyPressed = -1;
            }

            // TODO: Add your update logic here
            if ((keyPressed == 9 || danceArea.currentKeyPressed.IsKeyDown(Keys.V)) && availableToChange)
            {
                started = !started;
                availableToChange = false;
                timeDelayed = 60;           // 60fps czyli 30 to 0.5 sekundy

                if (started)
                {
                    Player.reset();
                    player = Player.getPlayer();
                    combo.startComboLoop();
                }
                else danceArea.reset();
            }

            if (!availableToChange)
            {
                timeDelayed--;
                if (timeDelayed <= 0)
                {
                    availableToChange = true;
                }
            }

            if (started)
            {
                if (danceArea.fuelBar.fuelValue < 0)
                    danceArea.fuelBar.fuelValue = 0;
                danceArea.fuelBar.fuelValue -= 0.006;
                danceArea.calculateHeatValue(danceArea.fuelBar.fuelValue);
                danceArea.readKey(keyPressed);
                danceArea.NumPadHitMapping();
                if (danceArea.heatValue <= 1)
                    danceArea.heatValue = 1;
                if (ifCreateSoul)
                {
                    danceArea.createSoul(Content, camera.Position, graphics.GraphicsDevice);
                    ifCreateSoul = false;
                    createSoulTimeDelay = 60 / (danceArea.level + 1);
                }

                if (!ifCreateSoul)
                {
                    createSoulTimeDelay--;
                    if (createSoulTimeDelay <= 0)
                    {
                        ifCreateSoul = true;
                    }
                }

                if (ifCheckAccelerometer)
                {
                    ifCheckAccelerometer = false;
                    accelTimeDelay = 10;
                }

                if (!ifCheckAccelerometer)
                {
                    accelTimeDelay--;
                    if (accelTimeDelay <= 0)
                    {
                        ifCheckAccelerometer = true;
                    }
                }

                if (danceArea.isLogCreated == false)
                {
                    danceArea.isLogCreated = true;
                    danceArea.createLog(Content, camera.Position, graphics.GraphicsDevice);
                }
                if (danceArea.isLogCreated == true)
                {
                    danceArea.moveLog();
                    //danceArea.moveLog(gamepad.accelerometerStatus());
                    if (gamepad.swung() > 6.5f && danceArea.woodLog.isDestroyable == true)
                    {
                        danceArea.woodLogDestroySuccessfulHit(15);
                        //billboardRect = new BBRectangle("Assets\\OtherTextures\\slashTexture", Content, danceArea.woodLog.position);
                        //billboardRect = new BBRectangle("Assets\\OtherTextures\\slashTexture", Content, danceArea.woodLog.position, graphics.GraphicsDevice);
                    }
                }
                
            }

            danceArea.pastKeyPressed = danceArea.currentKeyPressed;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            AnimatedModelShader.EffectPasses pass = AnimatedModelShader.EffectPasses.Unskinned;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //cauldron.DrawModel(camera.view, camera.projection, new Vector3((float)danceArea.heatValue, 1f, 1f));

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            animTemplate.DrawModel(camera.view, camera.projection, 
                _animatedModelShader, AnimatedModelShader.EffectPasses.UnskinnedDepth, true);


            GraphicsDevice.SetRenderTarget(null);
            animTemplate.DrawModel(camera.view, camera.projection, 
                _animatedModelShader, pass, false);


            spriteBatch.Begin();
            //danceArea.DrawFuelBar(spriteBatch);
            // TODO: Add your drawing code here
            if (player.hp > 0)
            {
                switch (danceArea.level)
                {
                    case 0:
                        danceArea.baseSoulsSpeed = 0.03f;
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: easy", new Vector2(100, 100), Color.Black);
                        break;
                    case 1:
                        danceArea.baseSoulsSpeed = 0.04f;
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: medium", new Vector2(100, 100), Color.Black);
                        break;
                    case 2:
                        danceArea.baseSoulsSpeed = 0.05f;
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: hard", new Vector2(100, 100), Color.Black);
                        break;
                }

                if (combo.getIfComboIsActive() && started)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        spriteBatch.Draw(combo.drawMap(graphics, i), combo.getRectangleCoord(graphics, i), combo.getColor());
                    }
                }
            }
            else
            {
                spriteBatch.DrawString(font, "Przegranko", new Vector2(100, 100), Color.Black);
                combo.stopComboLoop();
                //started = false;
            }
            spriteBatch.DrawString(font, "HV: " + danceArea.heatValue, new Vector2(100, 150), Color.Black);
            spriteBatch.DrawString(font, "Fire Temperature: " + danceArea.fuelBar.fuelValue, new Vector2(100, 175), Color.Black);
            //danceArea.DrawFuelBar(spriteBatch);
            spriteBatch.End();
            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //billboardRect.DrawRect(eff, graphics.GraphicsDevice, camera.view, camera.projection, camera.world);
            if (started)
            {
                danceArea.moveSoul(camera.view, camera.projection);
                if (danceArea.isLogCreated == true)
                    danceArea.woodLog.drawWoodenLog(camera.view, camera.projection);
                //if (billboardRect != null)
                    //billboardRect.DrawRect(cameraPos, eff, graphics.GraphicsDevice, camera);
            }

            
            base.Draw(gameTime);
        }


    }
}
