﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        int timeDelayed = 0;
        bool availableToChange = true;

        int createSoulTimeDelay = 0;
        bool ifCreateSoul = true;


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
            cameraPos = new Vector3(0, 110, 50);
            cauldronPos = new Vector3(0f, 0f, 0f);

            camera = new Camera();
            camera.setWorldMatrix(cameraPos);
            camera.view = Matrix.CreateLookAt(cameraPos, cauldronPos, Vector3.UnitY);
            camera.projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); //Bardzo ważne! Głębokość na jaką patrzymy!

            cauldron = new Asset();
            cauldron.loadModel(Content, "Assets\\Cauldron\\RictuCauldron");
            cauldron.world = Matrix.CreateTranslation(cauldronPos);

            gamepad = new Pad();

            danceArea = new DanceArea(cauldron);
            font = Content.Load<SpriteFont>("HP");

            player = Player.getPlayer();


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
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            int keyPressed = gamepad.getKeyState();

            // TODO: Add your update logic here
            if (keyPressed == 9 && availableToChange)
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
            }

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

            // TODO: Add your drawing code here
            if (player.hp > 0)
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
                danceArea.moveSoul(camera.view, camera.projection);

            cauldron.DrawModel(camera.view, camera.projection);

            base.Draw(gameTime);
        }


    }
}
