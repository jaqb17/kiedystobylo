using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Soul
    {
        Camera camera;
        Asset soul;
        Vector3 soulPosition;
        public int lifes { get; set; }

        public Soul(ContentManager content, String path)
        {
            soul = new Asset();
            lifes = randomNumber(Enum.GetValues(typeof(LifeColors)).Length);
            soul.loadModel(content, path);
        }

        public void Initialize(Camera camera)
        {
            this.camera = camera;
        }

        private int randomNumber(int range)
        {
            return Randomizer.GetRandomNumber(1, range + 1);
        }

        public void setSoulPosition(Vector3 position)
        {
            this.soulPosition = position;
            this.soul.world = Matrix.CreateTranslation(position);
            this.soul.scaleAset(1.0f);
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
