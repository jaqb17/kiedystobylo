using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;

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
        private Combo combo;
        public float baseSoulsSpeed { get; set; }
        public float escape_height = 51.0f;
        public int level = 0;
        public double heatValue = 2f;
        private Player player;
        public WoodenLog woodLog { get; set; }
        public Ice iceCube { get; set; }
        public FireFuelBar fuelBar { get; set; }

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
        public DanceArea(Asset cauldron)
        {
            this.radius = cauldron.radius / 2.5f;
            this.origin = cauldron.center;
            singleAreas = new SingleArea[numberOfAreas];
            player = Player.getPlayer();
            combo = Combo.createCombo();
        }

        public void Initialize(ContentManager content, Camera camera)
        {
            this.content = content;
            gamepad = new Pad();
            this.camera = camera;
        }

        public void Update(GameTime gameTime)
        {
            int keyPressed;
            currentKeyPressed = Keyboard.GetState();

            if (gamepad.USBMatt != null)  keyPressed = gamepad.getKeyState();
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

                if(isIceCreated == false)
                {
                    createIce();
                }
                if(isIceCreated == true)
                {
                    iceCube.Update(gameTime);
                }
                // tymczasowo wylaczone

                //if (isLogCreated == false)
                //{
                //    createLog();
                //}
                //if (isLogCreated == true)
                //{
                //    woodLog.Update(gameTime);

                //    //danceArea.moveLog(gamepad.accelerometerStatus());
                //    if (gamepad.swung() > 6.5f && woodLog.isDestroyable == true)
                //    {
                //        woodLogDestroySuccessfulHit(15);
                //        //billboardRect = new BBRectangle("Assets\\OtherTextures\\slashTexture", Content, danceArea.woodLog.position);
                //        //billboardRect = new BBRectangle("Assets\\OtherTextures\\slashTexture", Content, danceArea.woodLog.position, graphics.GraphicsDevice);
                //    }
                //}
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
                    singleAreas[i].Draw(gameTime);

                    if (!singleAreas[i].ifSoulIsAlive && !singleAreas[i].ifSoulIsAnimated)
                        singleAreas[i] = null;

                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (ifGameStarted)
            {
                moveSouls(gameTime);

                if (isLogCreated == true)
                    woodLog.Draw(gameTime);
                if (isIceCreated == true)
                    iceCube.Draw(gameTime);
                //if (billboardRect != null)
                //billboardRect.DrawRect(cameraPos, eff, graphics.GraphicsDevice, camera);
            }

            switch (level)
            {
                case 0:
                    baseSoulsSpeed = 0.03f;
                    break;
                case 1:
                    baseSoulsSpeed = 0.04f;
                    break;
                case 2:
                    baseSoulsSpeed = 0.05f;
                    break;
            }
        }

        private Vector3 computePosition(Vector3 origin, float radius, int id)
        {
            origin.X += 7f;
            Vector3 result = origin;

            float angle = (float)(id * 360.0f / numberOfAreas * Math.PI / 180.0f);
            result.X += (float)(radius * Math.Cos(angle));
            result.Z += (float)(radius * Math.Sin(angle));

            return result;
        }

        private void createSoul()
        {
            int i = Randomizer.GetRandomNumber(0, numberOfAreas);
            if (singleAreas[i] == null || singleAreas[i].soul == null)
            {
                singleAreas[i] = new SingleArea(content, computePosition(origin, radius, i));
                singleAreas[i].Initialize(camera);
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
            }

            this.Killed();
        }

        //WoodLog Methods
        private void createLog()
        {
            //woodLog = new WoodenLog();
            //woodLog = new WoodenLog(content, "Assets/Ice/lodAnim.fbx");

            //woodLog = new WoodenLog(content, "Assets\\TestAnim\\muchomorStadnyAtak");
            //woodLog = new WoodenLog(content, "\\Assets\\Ice\\lodStable");
            woodLog = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");
            woodLog.Initialization(camera);
        }
        private void createIce()
        {
            iceCube = new Ice(content, "Assets\\Souls\\bryla");
            iceCube.Initialization(camera);
        }
        private void calculateHeatValue(double _var)
        {
            double difference = _var - heatValue;

            heatValue += difference / 500;
        }

        private void woodLogDestroySuccessfulHit(int _fuelValueChange)
        {
            //Add fuel to the flames
            woodLog.destroyLog();
            //fuelBar.fuelValueChange(_fuelValueChange);
            fuelBar.fuelValue += 1f;
        }

        private void comboFunction(int areaPressed)
        {
            if (combo.getIfComboIsActive())
            {
                int[] killedSoulIds = combo.areaPressed(areaPressed);
                if (killedSoulIds != null)
                {
                    foreach (int index in killedSoulIds)
                        this.takeAllSoulHP(index);
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
            if (currentKeyPressed.IsKeyDown(Keys.NumPad5) && woodLog != null && woodLog.isDestroyable == true)
            {
                woodLogDestroySuccessfulHit(25);
            }
        }

        public void FuelBarInitialize(ContentManager content)
        {
            fuelBar = new FireFuelBar(new Vector2(100, 60), "Assets\\OtherTextures\\slashTexture", content);
        }

        public void DrawFuelBar(SpriteBatch _batch)
        {
            _batch.Draw(fuelBar.texture, fuelBar.barRectangle, Color.White);
        }
    }
}
