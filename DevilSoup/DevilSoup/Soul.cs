using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace DevilSoup
{
    public class Soul
    {
        Camera camera;
        public Asset soul { get; set; }
        Vector3 soulPosition;
       
        public int lifes { get; set; }

        private bool ifPlay = false;

        public bool IfPlay
        {
            get { return soul.ifPlay; }
            set {
                ifPlay = value;
                soul.ifPlay = ifPlay;
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

        public Soul(ContentManager content, string modelPath)
        {
            soul = new Asset();
            lifes = randomNumber(Enum.GetValues(typeof(LifeColors)).Length);
            soul.loadModel(content, modelPath);

        }

        public Soul(ContentManager content, GraphicsDevice graphicsDevice, string modelPath, string colorTexturePath, string normalTexturePath, string specularTexturePath, string shaderPath = "Assets/Effects/CNS")
        {
            soul = new Asset();
            lifes = randomNumber(Enum.GetValues(typeof(LifeColors)).Length);
            soul.loadModel(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, shaderPath);
        }

        public void Initialize(Camera camera)
        {
            this.camera = camera;
            this.soul.PlayClip(this.soul.Clips[0]);
        }

        private int randomNumber(int range)
        {
            return Randomizer.GetRandomNumber(1, range + 1);
        }

        public void setSoulPosition(Vector3 position)
        {
            if (this.soul == null)
                return;

            this.soulPosition = position;
            this.soul.world = Matrix.CreateTranslation(position);
            this.soul.scaleAset(0.3f, 0.5f, 0.5f);
        }

        private Vector3 defineColor()
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

        public void Update(GameTime gameTime)
        {
            if (this.soul == null) return;

            if (this.soul.HasAnimation)
            {
                this.soul.animationUpdate(gameTime);

                if (!this.soul.ifPlay && this.soul.IfDamageAfterPlay)
                {
                    this.soul.ifPlay = true;
                }

                if (this.soul.finishedAnimation && this.soul.IfDamageAfterPlay)
                {
                    killSoul();
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (this.soul != null)
                this.soul.Draw(gameTime, camera.view, camera.projection, defineColor());
        }

        public void killSoulWithAnimation()
        {
            float xCoord = 1.001f, yCoord = 0.95f, zCoord = 1.001f;
            for (int i = 0; i < 70; i++)
            {
                Thread.Sleep(20);
                if (this.soul != null)
                    this.soul.scaleAset(xCoord, yCoord, zCoord);
            }
        }

        public void killSoul()
        {
            this.soul.unloadModel();
            this.soul = null;
        }
    }
}
