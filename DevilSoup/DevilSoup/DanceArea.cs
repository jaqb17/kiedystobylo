using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private bool[] killWithAnimation;
        private float radius;
        private Combo combo;
        public float escape_height = 51.0f;
        public int level = 0;
        private Player player;
        public WoodenLog woodLog { get; set; }
        public FireFuelBar fuelBar { get; set; }

        public bool isLogCreated = false;

        public DanceArea(Asset cauldron)
        {
            this.radius = cauldron.radius / 2.5f;
            this.origin = cauldron.center;
            singleAreas = new SingleArea[numberOfAreas];
            killWithAnimation = new bool[numberOfAreas];
            for (int i = 0; i < numberOfAreas; i++) killWithAnimation[i] = false;
            player = Player.getPlayer();
            combo = Combo.createCombo();
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

        public void createSoul(ContentManager content)
        {

            int i = Randomizer.GetRandomNumber(0, numberOfAreas);
            if (singleAreas[i] == null || singleAreas[i].soul == null)
            {
                singleAreas[i] = new SingleArea(content, computePosition(origin, radius, i));
            }

        }

        public void reset()
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
        public void moveSoul(Matrix view, Matrix projection)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null && singleAreas[i].soul != null)
                {
                    if (singleAreas[i].ifSoulIsAlive)
                    {
                        Vector3 newPos = singleAreas[i].soulPosition;
                        if (this.singleAreas[i].soul.lifes > 0)
                            newPos.Y += 0.05f;

                        singleAreas[i].moveSoul(newPos);
                        if (newPos.Y >= escape_height)
                        {
                            this.Escaped(singleAreas[i].soul.lifes * 10);
                            singleAreas[i].soul.killSoul();
                            singleAreas[i] = null;
                        }
                        else if (singleAreas[i].soul.lifes < 0)
                        {
                            this.Killed();
                            singleAreas[i].soul.killSoul();
                            singleAreas[i] = null;
                        }
                    }
                    updateSoul(view, projection);
                }
            }
        }

        private void updateSoul(Matrix view, Matrix projection)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null)
                {
                    singleAreas[i].updateSoul(view, projection);
                    if (this.killWithAnimation[i])
                    {
                        singleAreas[i].killWithAnimation(view, projection);
                        this.killWithAnimation[i] = false;
                    }
                }
            }
        }

        public void Escaped(int power)
        {
            player = Player.getPlayer();
            player.hp -= power;
        }

        public void Killed()
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
                killWithAnimation[id] = true;

            this.Killed();
        }

        //WoodLog Methods
        public void createLog(ContentManager content)
        {
            //woodLog = new WoodenLog();
            woodLog = new WoodenLog(content, "Assets\\Souls\\bryla");
        }
        public void moveLog()
        {

            Vector3 newLogPosition = woodLog.position;

            newLogPosition.X -= 1f;
            newLogPosition.Y = -(newLogPosition.X * newLogPosition.X) / 200 + 50;
            //newLogPosition.Y += 0.1f;
            //if (newLogPosition.X < 0f)
            //    newLogPosition.Y -= 0.1f;
            woodLog.setPosition(newLogPosition);
            if (woodLog.position.Y > 48.5f)
                woodLog.isDestroyable = true;
            else
                woodLog.isDestroyable = false;
            if (newLogPosition.X < -100f)
                woodLogDestroyFailedToHit();

        }

        public void moveLog(Vector3 offset)
        {
            Vector3 newLogPosition = woodLog.position;
            newLogPosition += offset / 3;
            woodLog.setPosition(newLogPosition);
        }

        private void woodLogDestroyFailedToHit()
        {
            isLogCreated = false;
            this.woodLog = null;
        }

        public void woodLogDestroySuccessfulHit(int _fuelValueChange)
        {
            //Add fuel to the flames
            isLogCreated = false;
            woodLog.destroyLog();
            fuelBar.fuelValueChange(_fuelValueChange);
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
        public void readKey(int key)
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

        public void NumPadHitMapping()
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
            if (currentKeyPressed.IsKeyDown(Keys.NumPad5) && woodLog.isDestroyable == true)
            {
                woodLogDestroySuccessfulHit(25);
            }
        }
        public void FuelBarInitialize(ContentManager content)
        {
            fuelBar = new FireFuelBar(new Vector2(50, 30), "Assets\\FireFuelBar\\bar", content);
        }
        public void DrawFuelBar(SpriteBatch _batch)
        {
            _batch.Draw(fuelBar.texture, fuelBar.barRectangle, Color.White);
        }
    }
}
