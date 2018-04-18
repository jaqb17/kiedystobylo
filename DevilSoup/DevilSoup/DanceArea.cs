using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class DanceArea
    {

        public enum Areas
        {
            BottomRight = 1,
            Right = 0,
            Bottom = 2,
            BottomLeft = 3,
            Left = 4,
            UpperLeft = 5,
            Up = 6,
            UpperRight = 7,
        }

        public KeyboardState currentKeyPressed;
        public KeyboardState pastKeyPressed;

        private const int numberOfAreas = 8;
        private Vector3 origin;
        private SingleArea[] singleAreas;
        private float radius;
        public float escape_height = 51.0f;
        public int level = 0;
        private Player player;
        private WoodenLog woodLog;

        public bool isLogCreated = false;

        public DanceArea(Asset cauldron)
        {
            this.radius = cauldron.radius / 2.5f;
            this.origin = cauldron.center;
            singleAreas = new SingleArea[numberOfAreas];
            player = Player.getPlayer();
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
        }
        public void moveSoul(Matrix view, Matrix projection)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null && singleAreas[i].ifSoulIsAlive)
                {
                    Vector3 newPos = singleAreas[i].soulPosition;
                    newPos.Y += 0.05f;
                    //Console.WriteLine("y " + newPos.Y);
                    singleAreas[i].moveSoul(newPos);
                    if (newPos.Y >= escape_height)
                    {
                        this.Escaped(singleAreas[i].soul.lifes * 10);
                        singleAreas[i].soul.killSoul();
                        singleAreas[i] = null;
                    }
                    else if (singleAreas[i].soul != null && singleAreas[i].soul.lifes <= 0)
                    {
                        this.Killed();
                        singleAreas[i].soul.killSoul();
                        singleAreas[i] = null;
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
                    singleAreas[i].updateSoul(view, projection);
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
        //WoodLog Methods
        public void createLog(ContentManager content)
        {
            woodLog = new WoodenLog();
        }
        public void moveLog()
        {

            Vector3 newLogPosition = woodLog.position;
            newLogPosition.X -= 1f;
            newLogPosition.Y = (-(newLogPosition.X * newLogPosition.X) / 400)+100;
            woodLog.setPosition(newLogPosition);
            if (woodLog.position.Y > 75f)
                woodLog.isDestroyable = true;
            else
                woodLog.isDestroyable = false;
            if (newLogPosition.X < -200f)
                woodLogDestroyFailedToHit();

        }

        private void woodLogDestroyFailedToHit()
        {
            isLogCreated = false;
            this.woodLog = null;
        }
        
        private void woodLogDestroySuccessfulHit()
        {
            //Add fuel to the flames
            //destroy woodlog
        }

        
        //Keymapping
        public void readKey(int key)
        {

            switch (key)
            {
                case 0:
                    hurtSoul(6);
                    break;
                case 1:
                    hurtSoul(2);
                    break;
                case 2:
                    hurtSoul(4);
                    break;
                case 3:
                    hurtSoul(0);
                    break;
                case 4:
                    hurtSoul(3);
                    break;
                case 5:
                    hurtSoul(1);
                    break;
                case 6:
                    hurtSoul(5);
                    break;
                case 7:
                    hurtSoul(7);
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
                hurtSoul((int)Areas.BottomLeft);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad2) && pastKeyPressed.IsKeyUp(Keys.NumPad2))
                hurtSoul((int)Areas.Bottom);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad3) && pastKeyPressed.IsKeyUp(Keys.NumPad3))
                hurtSoul((int)Areas.BottomRight);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad4) && pastKeyPressed.IsKeyUp(Keys.NumPad4))
                hurtSoul((int)Areas.Left);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad6) && pastKeyPressed.IsKeyUp(Keys.NumPad6))
                hurtSoul((int)Areas.Right);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad7) && pastKeyPressed.IsKeyUp(Keys.NumPad7))
                hurtSoul((int)Areas.UpperLeft);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad8) && pastKeyPressed.IsKeyUp(Keys.NumPad8))
                hurtSoul((int)Areas.Up);
            if (currentKeyPressed.IsKeyDown(Keys.NumPad9) && pastKeyPressed.IsKeyUp(Keys.NumPad9))
                hurtSoul((int)Areas.UpperRight);
            if (currentKeyPressed.IsKeyDown(Keys.C) && pastKeyPressed.IsKeyUp(Keys.C))
            {
                level++;
                if (level > 2)
                    level = 0;
            }
            if (currentKeyPressed.IsKeyDown(Keys.NumPad5) && woodLog.isDestroyable == true)
            {
                woodLog.destroyLog();
                isLogCreated = false;
            }
        }
    }
}
