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
        private SpriteFont font;
        private Vector3 cameraPos, cauldronPos;
        private Camera camera;
        private Asset cauldron;
        private Pad gamepad;
        private DanceArea danceArea;
        private Player player;
        private Combo combo;
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
            //graphics.IsFullScreen = true;
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
            cameraPos = new Vector3(0, 110, 40);
            //cameraPos = new Vector3(0f, 0f, 200f);
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

            gamepad = new Pad();

            danceArea = new DanceArea(cauldron);
            font = Content.Load<SpriteFont>("HP");

            player = Player.getPlayer();
            combo = Combo.createCombo();
            danceArea.FuelBarInitialize(Content);

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

            danceArea.currentKeyPressed = Keyboard.GetState();
            int keyPressed = gamepad.getKeyState();
            

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
                danceArea.readKey(keyPressed);
                danceArea.NumPadHitMapping();
                
                if (ifCreateSoul)
                {
                    danceArea.createSoul(Content);
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
                    //gamepad.accelerometerStatus();
                    ifCheckAccelerometer = false;
                    accelTimeDelay = 10;
                    danceArea.fuelBar.fuelValueChange(-1);
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
                    danceArea.createLog(Content);
                }
                if (danceArea.isLogCreated == true)
                {
                    danceArea.moveLog();
                    //danceArea.moveLog(gamepad.accelerometerStatus());
                    if (gamepad.swung() > 4.0f)
                        danceArea.woodLogDestroySuccessfulHit(15);
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

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            spriteBatch.Begin();
            danceArea.DrawFuelBar(spriteBatch);
            // TODO: Add your drawing code here
            if (player.hp > 0 && danceArea.fuelBar.isBarEmpty() == false)
            {
                switch (danceArea.level)
                {
                    case 0:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: easy", new Vector2(100, 100), Color.Black);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: medium", new Vector2(100, 100), Color.Black);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: hard", new Vector2(100, 100), Color.Black);
                        break;
                }

                for(int i = 0; i < 9; i++)
                {
                    spriteBatch.Draw(combo.drawMap(graphics), combo.getRectangleCoord(graphics, i), combo.getColor());
                }
            }
            else
            {
                spriteBatch.DrawString(font, "Przegranko", new Vector2(100, 100), Color.Black);
                started = false;
            }
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            if (started)
            {
                danceArea.moveSoul(camera.view, camera.projection);
                if (danceArea.isLogCreated == true)
                    danceArea.woodLog.drawWoodenLog(camera.view, camera.projection);
            }

           cauldron.DrawModel(camera.view, camera.projection);
            //cauldron2.DrawModel(camera.view, camera.projection);
            
            base.Draw(gameTime);
        }


    }
}
