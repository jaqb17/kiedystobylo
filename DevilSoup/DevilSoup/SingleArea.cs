using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Threading;

namespace DevilSoup
{
    public class SingleArea
    {
        public Vector3 areaCenter { get; private set; }
        public Vector3 soulPosition { get; private set; }
        public bool ifSoulIsAlive { get; private set; }
        public bool ifSoulIsAnimated { get; set; } = false;
        private Camera camera;
        public Soul soul;
        private String modelPath = "Assets\\Souls\\bryla";
        //private String path = "Assets\\Souls\\T-pose";
        private float escape_height = 51.0f;
        private Player player;
        public float baseSoulsSpeed { get; set; }
        public double heatValue = 2f;
        public int level = 0;

        public SingleArea(ContentManager content, Vector3 areaCenter)
        {
            this.areaCenter = areaCenter;
            this.soul = new Soul(content, this.modelPath);
            this.soulPosition = this.areaCenter;
            this.ifSoulIsAlive = true;
        }

        public void Initialize(Camera camera)
        {
            this.camera = camera;
            player = Player.getPlayer();
            this.soul.Initialize(camera);
        }

        private void moveSoul(Vector3 position)
        {
            if (this.soul == null)
                return;

            this.soulPosition = position;
            this.soul.setSoulPosition(this.soulPosition);
        }

        public void Draw(GameTime gameTime)
        {
            if (this.soul != null && this.ifSoulIsAlive)
            {
                Vector3 newPos = soulPosition;
                if (this.soul.lifes > 0)
                    newPos.Y += baseSoulsSpeed * (float)heatValue;

                moveSoul(newPos);
                if (newPos.Y >= escape_height)
                {
                    this.Escaped(soul.lifes * 10);
                    soul.killSoul();
                    ifSoulIsAlive = false;
                }
                else if (soul.lifes < 0)
                {
                    this.Killed();
                    soul.killSoul();
                    ifSoulIsAlive = false;
                }
            }
            if (soul != null)
            {
                this.soul.Draw(gameTime);
            }
        }

        private void Killed()
        {
            player = Player.getPlayer();
            player.points += (this.level + 1);
        }

        private void Escaped(int power)
        {
            player = Player.getPlayer();
            player.hp -= power;
        }

        public void killWithAnimation()
        {
            if (this.soul == null) return;

            ThreadStart starter = new ThreadStart(() => { this.soul.killSoulWithAnimation(); });
            starter += () =>
            {
                Console.WriteLine("Killed!");

                this.soul.killSoul();
                this.soul = null;
            };

            Thread animatedKill = new Thread(starter) { IsBackground = true };
            animatedKill.Name = "Animated killing thread";
            animatedKill.Start();
            this.soul.lifes = 0;
            this.ifSoulIsAlive = false;
        }

        public bool takeSoulLife()
        {
            if (this.soul == null) return true;
            if (this.soul.lifes == 0) return false;

            this.soul.lifes -= 1;

            if (this.soul.lifes <= 0)
            {
                this.ifSoulIsAlive = false;
                this.soul.killSoul();
                this.soul = null;
                return true;
            }

            return false;
        }
    }
}
