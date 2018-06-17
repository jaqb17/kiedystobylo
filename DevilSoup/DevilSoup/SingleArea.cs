using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private Bubble bubble;
        private String modelPath = "Assets\\Souls\\grzesznikT\\TreadingWater";
        private String aldeboMapPath = "Assets\\Souls\\grzesznikT\\grzesznik_Albedo";
        private String normalMapPath = "Assets\\Souls\\grzesznikT\\grzesznik_Normal";
        private String specularMapPath = null;

        //private String path = "Assets\\Souls\\T-pose";
        private float escape_height = 51.0f;
        private Player player;
        public float baseSoulsSpeed { get; set; }
        public double heatValue = 2f;
        public int level = 0;

        private bool ifPlay = false;
        public bool IfPlay
        {
            get { return soul.IfPlay; }
            set
            {
                ifPlay = value;
                soul.IfPlay = ifPlay;
            }
        }

        private bool ifDamageAfterPlay = false;

        public bool IfDamageAfterPlay
        {
            get { return soul.IfDamageAfterPlay; }
            set
            {
                ifDamageAfterPlay = value;
                soul.IfDamageAfterPlay = ifDamageAfterPlay;
            }
        }


        public SingleArea(ContentManager content, GraphicsDevice graphicsDevice, Vector3 areaCenter)
        {
            this.areaCenter = areaCenter;
            //this.soul = new Soul(content, this.modelPath);
            this.soul = new Soul(content, graphicsDevice, modelPath, aldeboMapPath, normalMapPath, null);
            this.bubble = new Bubble(content);
            this.soulPosition = this.areaCenter;
            this.ifSoulIsAlive = true;
        }

        public void Initialize(Camera camera)
        {
            this.camera = camera;
            player = Player.getPlayer();
            this.soul.Initialize(camera);
            this.bubble.Initialize(camera, soulPosition);
        }

        private void moveSoul(Vector3 position)
        {
            if (this.soul == null)
                return;

            this.soulPosition = position;
            this.soul.setSoulPosition(this.soulPosition);
        }

        public void Update(GameTime gameTime)
        {
            if(this.soul != null && this.ifSoulIsAlive && !this.ifSoulIsAnimated)
            {
                this.soul.Update(gameTime);
            }
        }

        private Vector3 defineColor(int lifes)
        {
            if (lifes == 0) return new Vector3(255.0f, 0.0f, 0.0f);
            LifeColors color = (LifeColors)Enum.Parse(typeof(LifeColors), Enum.GetName(typeof(LifeColors), lifes));

            switch (color)
            {
                case LifeColors.Red:
                    return new Vector3(255.0f, 0.0f, 0.0f);
                case LifeColors.Green:
                    return new Vector3(0.0f, 255.0f, 0.0f);
                case LifeColors.Blue:
                    return new Vector3(0.0f, 0.0f, 255.0f);
                case LifeColors.Brown:
                    return new Vector3(210.0f, 105.0f, 30.0f);
            }

            return new Vector3(255.0f, 0.0f, 0.0f);
        }

        public void Draw(GameTime gameTime)
        {
            if (this.soul != null && this.ifSoulIsAlive)
            {
                Vector3 newPos = soulPosition;
                if (this.soul.lifes > 0)
                    newPos.Y += baseSoulsSpeed * (float)heatValue;

                moveSoul(newPos);
                bubble.setPosition(newPos);
                bubble.scale(0.2f, 0.25f, 0.2f);
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
                bubble.Draw(defineColor(soul.lifes) * .001f);
            }
        }

        private void Killed()
        {
            player = Player.getPlayer();
            Console.WriteLine("sa1hp=" + player.hp + " gameover=" + player.gameOver);
            if (player.hp <= 0)
                return;
            Console.WriteLine("sa2hp=" + player.hp + " gameover=" + player.gameOver);
            if (player.hp > 0)
                player.points += (this.level + 1); 

        }

        private void Escaped(int power)
        {
            player = Player.getPlayer();
            if (player.hp <= 0)
                return;
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
            this.bubble.kill();
            this.ifSoulIsAlive = false;
        }

        public bool takeSoulLife()
        {
            if (this.soul == null) return true;
            if (this.soul.lifes == 0) return false;

            this.soul.lifes -= 1;
            this.bubble.takeLife();

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
