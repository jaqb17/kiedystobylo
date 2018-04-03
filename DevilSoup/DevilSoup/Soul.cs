using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Soul
    {
        Asset soul;
        Vector3 soulPosition;
        public int lifes { get; set; }

        public Soul(ContentManager content, String path)
        {
            soul = new Asset();
            lifes = randomNumber(Enum.GetValues(typeof(LifeColors)).Length);
            soul.loadModel(content, "Assets\\Souls\\bryla");
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
            LifeColors color = (LifeColors) Enum.Parse(typeof(LifeColors), Enum.GetName(typeof(LifeColors), lifes));

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
        
        public void drawSoul(Matrix view, Matrix projection)
        {
            this.soul.DrawModel(view, projection, defineColor());
        }

        public void killSoul()
        {
            this.soul.unloadModel();
            this.soul = null;
        }
    }
}
