﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DevilSoup
{
    public class DanceArea
    {
        public KeyboardState currentKeyPressed;
        public KeyboardState pastKeyPressed;

        private const int numberOfAreas = 8;
        private Vector3 origin;
        private SingleArea[] singleAreas;
        private float radius;
        public Combo combo { get; private set; }
        public float baseSoulsSpeed { get; set; }
        public float escape_height = 51.0f;
        public int level = 0;
        public double heatValue = 2f;
        private Player player;
        public WoodenLog woodLog { get; set; }
        public Ice iceCube { get; set; }
        public Fireplace fuelBar { get; set; }
        public int stage = 1;
        public bool ifLogHaveFlownAlready = false;

        private Asset soup;
        private Camera camera;
        private Pad gamepad;
        private ContentManager content;
        int timeDelayed = 0;
        bool availableToChange = true;
        int createSoulTimeDelay = 0;
        bool ifCreateSoul = true;
        bool ifCheckAccelerometer = true;
        int accelTimeDelay = 0;
        private bool ifGameStarted = false;
        private bool balance = true;
        private bool isWoodEnabled = false;
        private bool isIceEnabled = false;

        //Cauldron Colors
        private Vector3 standard, yellow, orange, red;
        public Vector3 currentColor { get; set; }

        //Sounds
        private Song boil;
        private SoundEffect soulDeath;
        private const int woodChopSounds = 3;
        private SoundEffect[] woodChopSoundTable;
        private bool isBoilingSoundActive;
        private GraphicsDevice graphicsDevice;

        //Billboard test
        BillboardSys slashes;
        Vector3[] ParPositions;

        public bool IfGameStarted
        {
            get { return ifGameStarted; }
            set { ifGameStarted = value; }
        }

        public bool isLogCreated
        {
            get
            {
                if (this.woodLog != null)
                    return woodLog.isLogCreated;
                else return false;
            }
        }
        public bool isIceCreated
        {
            get
            {
                if (this.iceCube != null)
                    return iceCube.isIceCreated;
                else return false;
            }
        }
        public bool isFlyingObjCreated
        {
            get
            {
                if (this.woodLog != null && !woodLog.isLogCreated)
                {
                    iceCube.isIceCreated = false;
                    return woodLog.isLogCreated;
                }
                else if (this.iceCube != null && !iceCube.isIceCreated)
                {
                    woodLog.isLogCreated = false;
                    return iceCube.isIceCreated;
                }

                else return false;
            }
        }
        public DanceArea(Asset cauldron)
        {
            this.radius = cauldron.radius / 2.5f;
            this.origin = cauldron.center;
            singleAreas = new SingleArea[numberOfAreas];
            player = Player.getPlayer();
            combo = Combo.createCombo();
            isBoilingSoundActive = false;
            woodChopSoundTable = new SoundEffect[woodChopSounds];
            #region Vectors cauldron colors 
            standard = new Vector3(0f, 0f, 0f);
            yellow = new Vector3(0.1f, 0.1f, 0);
            orange = new Vector3(0.2f, 0.05f, 0);
            red = new Vector3(0.3f, 0f, 0f);
            #endregion
        }

        public void Initialize(ContentManager content, Camera camera, GraphicsDevice graphicsDevice)
        {

            this.content = content;
            gamepad = new Pad();
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            fuelBar = new Fireplace(content, graphicsDevice);
            fuelBar.Initialization(camera);
            currentColor = yellow;
            boil = content.Load<Song>("Assets\\Sounds\\BoilingSoup\\boilP");
            soulDeath = content.Load<SoundEffect>("Assets\\Sounds\\SoulDeath\\soulDeath1");
            for (int i = 0; i < woodChopSounds; i++)
                woodChopSoundTable[i] = content.Load<SoundEffect>("Assets\\Sounds\\WoodChop\\wood" + (i + 1));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            //BB Test
            Random r = new Random();
            ParPositions = new Vector3[2];
            ParPositions[0] = new Vector3(65, 0, 0);
            ParPositions[1] = new Vector3(80, 0, 0);
            //ParPositions[2] = new Vector3(90, 20, 10);

            Vector3 soupPosition = new Vector3(0, 0, 0);
            soup = new Asset(content, "Assets/Soup/zupaModel",
                                       "Assets/Soup/zupa_Albedo",
                                       "Assets/Soup/zupa_Normal",
                                       "Assets/Soup/zupa_Metallic",
                                       "Assets/Skybox/helll",
                                       camera);

            soup.world = Matrix.CreateTranslation(soupPosition);
            soup.scaleAset(3f);
            soup.setShine(3f);
            soup.setAmbientIntensity(.8f);

            slashes = new BillboardSys(graphicsDevice, content, content.Load<Texture2D>("Assets\\OtherTextures\\slashTexture"), new Vector2(15), ParPositions);
        }

        public void Update(GameTime gameTime)
        {
            combo.actualGarnekColor = currentColor;
            int keyPressed;
            int Val = 10;
            currentKeyPressed = Keyboard.GetState();
            cauldronColorLogic();
            stage = (player.points / Val) + 1;
            if (stage == 1 && heatValue <= 1f)
                stage += 1;
            if (stage > 5)
                stage = 5;
            switch (stage)
            {
                case 1:
                    isWoodEnabled = false;
                    isIceEnabled = false;
                    if (this.woodLog != null)
                        woodLog.isWoodActive = false;
                    else if (this.iceCube != null)
                        iceCube.isIceActive = true;
                    break;
                case 2:
                    isWoodEnabled = true;
                    isIceEnabled = false;
                    if (this.woodLog != null)
                        woodLog.isWoodActive = true;
                    else if (this.iceCube != null)
                        iceCube.isIceActive = true;
                    break;
                case 3:
                    isWoodEnabled = true;
                    isIceEnabled = true;
                    if (this.woodLog != null)
                        woodLog.isWoodActive = true;
                    else if (this.iceCube != null)
                        iceCube.isIceActive = true;
                    break;
                case 4:
                    isWoodEnabled = true;
                    isIceEnabled = true;
                    if (this.woodLog != null)
                        woodLog.isWoodActive = true;
                    else if (this.iceCube != null)
                        iceCube.isIceActive = true;
                    if (balance)
                    {
                        baseSoulsSpeed += 0.005f;
                        balance = !balance;
                    }
                    break;
                case 5:
                    isWoodEnabled = true;
                    isIceEnabled = true;
                    if (this.woodLog != null)
                        woodLog.isWoodActive = true;
                    else if (this.iceCube != null)
                        iceCube.isIceActive = true;
                    if (balance)
                    {
                        baseSoulsSpeed += 0.005f;
                        balance = !balance;
                    }
                    break;
            }
            if (((player.points / Val) + 1) == 1)
                balance = true;
            if (gamepad.USBMatt != null) keyPressed = gamepad.getKeyState();
            else keyPressed = -1;

            if ((keyPressed == 9 || currentKeyPressed.IsKeyDown(Keys.V)) && availableToChange)
            {
                ifGameStarted = !ifGameStarted;
                combo.IfGameStarted = ifGameStarted;
                availableToChange = false;
                timeDelayed = 60;           // 60fps czyli 30 to 0.5 sekundy

                if (ifGameStarted)
                {
                    Player.reset();
                    player = Player.getPlayer();
                    combo.startComboLoop();
                }
                else this.reset();
            }

            if (!availableToChange)
            {
                timeDelayed--;
                if (timeDelayed <= 0)
                {
                    availableToChange = true;
                }
            }

            if (ifGameStarted)
            {
                if (isBoilingSoundActive == false)
                {
                    MediaPlayer.Play(boil);
                    isBoilingSoundActive = !isBoilingSoundActive;
                }
                fuelBar.Update(gameTime);
                calculateHeatValue(fuelBar.fuelValue);
                readKey(keyPressed);
                NumPadHitMapping();

                if (heatValue <= 1)
                    heatValue = 1;

                if (ifCreateSoul)
                {
                    createSoul();
                    ifCreateSoul = false;
                    createSoulTimeDelay = 60 / (level + 1);
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

                if (isIceCreated == false && isIceEnabled)
                {
                    createIce();

                }
                if (isIceCreated == true)
                {
                    iceCube.Update(gameTime);
                    if (gamepad.swung() > 6.5f && iceCube.isDestroyable == true && iceCube.isIceDestroyed == false)
                    {
                        iceCube.isIceDestroyed = true;
                        heatValue = -iceCube.fireBoostValue;
                        iceCube.destroyIce();
                    }
                }
                // tymczasowo wylaczone

                if (isLogCreated == false && isWoodEnabled)
                {
                    createLog();
                }
                if (isLogCreated == true)
                {
                    woodLog.Update(gameTime);


                    if (gamepad.swung() > 6.5f && woodLog.isDestroyable == true && woodLog.isLogDestroyed == false)
                    {
                        fuelBar.addLogBeneathCauldron(content, camera);
                        woodLog.isLogDestroyed = true;
                        woodLogDestroySuccessfulHit();
                    }
                }
                //fuelBar.Update(gameTime);
            }

            pastKeyPressed = currentKeyPressed;
        }

        private void moveSouls(GameTime gameTime)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null)
                {
                    singleAreas[i].baseSoulsSpeed = baseSoulsSpeed;
                    singleAreas[i].heatValue = heatValue;
                    singleAreas[i].Update(gameTime);
                    singleAreas[i].Draw(gameTime);

                    if (!singleAreas[i].ifSoulIsAlive && !singleAreas[i].ifSoulIsAnimated)
                        singleAreas[i] = null;

                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            slashes.Draw(camera.view, camera.projection, camera.Up, camera.Right);
            soup.SimpleDraw(camera.view, camera.projection);
            if (ifGameStarted)
            {
                moveSouls(gameTime);
                fuelBar.Draw(gameTime, camera);

                if (isLogCreated == true)
                    woodLog.Draw(gameTime);
                if (isIceCreated == true)
                    iceCube.Draw(gameTime);
                
            }

            switch (level)
            {
                case 0:
                    baseSoulsSpeed = 0.02f;
                    break;
                case 1:
                    baseSoulsSpeed = 0.03f;
                    break;
                case 2:
                    baseSoulsSpeed = 0.04f;
                    break;
            }
        }

        private void cauldronColorLogic()
        {
            if (heatValue < 2)
                currentColor = standard;
            if (heatValue > 2 && heatValue < 4)
                currentColor = yellow;
            if (heatValue > 4 && heatValue < 7)
                currentColor = orange;
            if (heatValue > 7 && heatValue < 11)
                currentColor = red;
        }

        private Vector3 computePosition(Vector3 origin, float radius, int id)
        {
            origin.Z -= 5f;
            Vector3 result = origin;
            float angle = (float)(id * 360.0f / numberOfAreas * Math.PI / 180.0f);
            result.X += (float)(1.0f * radius * Math.Cos(angle));
            result.Z += (float)(1.0f * radius * Math.Sin(angle));

            return result;
        }

        private void createSoul()
        {
            int i = Randomizer.GetRandomNumber(0, numberOfAreas);
            if (singleAreas[i] == null || singleAreas[i].soul == null)
            {
                singleAreas[i] = new SingleArea(content, graphicsDevice, computePosition(origin, radius, i));
                singleAreas[i].Initialize(camera);
                singleAreas[i].IfPlay = true;
            }
        }

        private void reset()
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null)
                {
                    if (singleAreas[i].soul != null)
                    {
                        singleAreas[i].soul.killSoul();
                        singleAreas[i].soul = null;
                    }
                }
            }
            combo.stopComboLoop();
        }

        private void Killed()
        {
            player = Player.getPlayer();
            player.points += (this.level + 1);
        }

        private void hurtSoul(int id)
        {
            bool ifKilled = false;
            if (singleAreas[id] != null)
                ifKilled = singleAreas[id].takeSoulLife();

            if (ifKilled)
                this.Killed();
        }

        private void takeAllSoulHP(int id)
        {
            if (singleAreas[id] != null)
            {
                singleAreas[id].ifSoulIsAnimated = true;
                singleAreas[id].killWithAnimation();
                this.Killed();
            }
        }

        //WoodLog Methods
        private void createLog()
        {
            if (heatValue > 0f && heatValue <= 4.8f)
            {
                string modelPath = "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz";
                string colorTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Albedo";
                string normalTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Normal";
                string specularTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Metallic";
                woodLog = new WoodenLog(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, specularTexturePath);
                woodLog.Initialization(camera);
                ifLogHaveFlownAlready = true;
            }
        }
        private void createIce()
        {
            if (heatValue > 5.2f && heatValue <= 10f)
            {
                string modelPath = "Assets\\Ice\\lodAn";
                string colorTexturePath = "Assets\\Ice\\lod_Albedo";
                string normalTexturePath = "Assets\\Ice\\lod_Normal";
                string specularTexturePath = "Assets\\Ice\\lod_Metallic";
                iceCube = new Ice(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, specularTexturePath);
                iceCube.Initialization(camera);
                ifLogHaveFlownAlready = true;
            }
        }

        private void calculateHeatValue(double _var)
        {
            double difference = _var - heatValue;

            heatValue += difference / 500;
        }

        private void woodLogDestroySuccessfulHit()
        {
            //Add fuel to the flames
            woodLog.destroyLog();

        }

        private void comboFunction(int areaPressed)
        {
            if (combo.getIfComboIsActive())
            {
                int[] killedSoulIds = combo.areaPressed(areaPressed);
                if (killedSoulIds != null)
                {
                    foreach (int index in killedSoulIds)
                    {
                        this.takeAllSoulHP(index);
                    }
                }
            }
        }

        //Keymapping
        private void readKey(int key)
        {
            switch (key)
            {
                case 0:
                    hurtSoul((int)SingleAreasIndexes.Up);
                    comboFunction((int)SingleAreasIndexes.Up);
                    break;
                case 1:
                    hurtSoul((int)SingleAreasIndexes.Bottom);
                    comboFunction((int)SingleAreasIndexes.Bottom);
                    break;
                case 2:
                    hurtSoul((int)SingleAreasIndexes.Left);
                    comboFunction((int)SingleAreasIndexes.Left);
                    break;
                case 3:
                    hurtSoul((int)SingleAreasIndexes.Right);
                    comboFunction((int)SingleAreasIndexes.Right);
                    break;
                case 4:
                    hurtSoul((int)SingleAreasIndexes.BottomLeft);
                    comboFunction((int)SingleAreasIndexes.BottomLeft);
                    break;
                case 5:
                    hurtSoul((int)SingleAreasIndexes.BottomRight);
                    comboFunction((int)SingleAreasIndexes.BottomRight);
                    break;
                case 6:
                    hurtSoul((int)SingleAreasIndexes.UpperLeft);
                    comboFunction((int)SingleAreasIndexes.UpperLeft);
                    break;
                case 7:
                    hurtSoul((int)SingleAreasIndexes.UpperRight);
                    comboFunction((int)SingleAreasIndexes.UpperRight);
                    break;
                case 8:
                    level++;
                    if (level > 2)
                        level = 0;
                    break;
                default:
                    return;
            }
        }

        private void NumPadHitMapping()
        {
            if (currentKeyPressed.IsKeyDown(Keys.NumPad1) && pastKeyPressed.IsKeyUp(Keys.NumPad1))
            {
                hurtSoul((int)SingleAreasIndexes.BottomLeft);
                comboFunction((int)SingleAreasIndexes.BottomLeft);
            }

            if (currentKeyPressed.IsKeyDown(Keys.NumPad2) && pastKeyPressed.IsKeyUp(Keys.NumPad2))
            {
                hurtSoul((int)SingleAreasIndexes.Bottom);
                comboFunction((int)SingleAreasIndexes.Bottom);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad3) && pastKeyPressed.IsKeyUp(Keys.NumPad3))
            {
                hurtSoul((int)SingleAreasIndexes.BottomRight);
                comboFunction((int)SingleAreasIndexes.BottomRight);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad4) && pastKeyPressed.IsKeyUp(Keys.NumPad4))
            {
                hurtSoul((int)SingleAreasIndexes.Left);
                comboFunction((int)SingleAreasIndexes.Left);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad6) && pastKeyPressed.IsKeyUp(Keys.NumPad6))
            {
                hurtSoul((int)SingleAreasIndexes.Right);
                comboFunction((int)SingleAreasIndexes.Right);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad7) && pastKeyPressed.IsKeyUp(Keys.NumPad7))
            {
                hurtSoul((int)SingleAreasIndexes.UpperLeft);
                comboFunction((int)SingleAreasIndexes.UpperLeft);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad8) && pastKeyPressed.IsKeyUp(Keys.NumPad8))
            {
                hurtSoul((int)SingleAreasIndexes.Up);
                comboFunction((int)SingleAreasIndexes.Up);
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad9) && pastKeyPressed.IsKeyUp(Keys.NumPad9))
            {
                hurtSoul((int)SingleAreasIndexes.UpperRight);
                comboFunction((int)SingleAreasIndexes.UpperRight);
            }
            if (currentKeyPressed.IsKeyDown(Keys.C) && pastKeyPressed.IsKeyUp(Keys.C))
            {
                level++;
                if (level > 2)
                    level = 0;
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad5) && woodLog != null && woodLog.isDestroyable == true && woodLog.isLogDestroyed == false)
            {
                woodLog.isLogDestroyed = true;
                fuelBar.addLogBeneathCauldron(content, camera);
                woodLogDestroySuccessfulHit();
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad5) && iceCube != null && iceCube.isDestroyable == true && iceCube.isIceDestroyed == false)
            {
                iceCube.isIceDestroyed = true;
                iceCube.destroyIce();
                heatValue += iceCube.fireBoostValue;
            }
            
                
        }

        public void FuelBarInitialize(ContentManager content)
        {
            fuelBar = new Fireplace(content, graphicsDevice);
        }

        public float getHeatIntensity()
        {
            float intensity = 0;

            if(currentColor == standard)
            {
                intensity = 0.05f;
            }
            else if(currentColor == yellow)
            {
                intensity = 0.15f;
            }
            else if(currentColor == orange)
            {
                intensity = 0.35f;
            }
            else
            {
                intensity = 0.5f;
            }

            return intensity;
        }

        public float getHeatAmp()
        {
            float amp = 0;

            if (currentColor == standard)
            {
                amp = 0.0001f;
            }
            else if (currentColor == yellow)
            {
                amp = 0.0005f;
            }
            else if (currentColor == orange)
            {
                amp = 0.001f;
            }
            else
            {
                amp = 0.004f;
            }

            return amp;
        }


    }
}