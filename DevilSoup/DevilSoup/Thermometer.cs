using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;

namespace DevilSoup
{
    public class Thermometer
    {
        SpriteSheetLoader spriteSheetLoader;
        SpriteSheet spriteSheet;
        SpriteBatch spriteBatch;
        SpriteRender spriteRender;
        bool display = true;
        int temperature = 200;
        Vector2? position = null;

        string imageName = Images.Temp_200;

        public bool Display
        {
            get { return display; }
            set { display = value; }
        }

        public Thermometer(Vector2 position)
        {
            this.position = position;
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            spriteSheetLoader = new SpriteSheetLoader(content);
            spriteSheet = spriteSheetLoader.Load("Assets\\GUI\\Termometr\\termo");
            this.spriteBatch = spriteBatch;
            spriteRender = new SpriteRender(this.spriteBatch);
        }

        public void UpdateTemperature(double temp)
        {
            int[] tempArr = { 200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000, 2200 };

            int distance = 10000;
            foreach (int tempInd in tempArr)
            {
                if (Math.Abs(tempInd - temp * 200) < distance)
                {
                    distance = (int)Math.Abs(tempInd - temp * 200);
                    this.temperature = tempInd;
                }
            }
        }

        public void Draw()
        {

            if (display)
            {
                if (temperature > Images.TempTopLimit) temperature = Images.TempTopLimit;
                else if (temperature < Images.TempBottomLimit) temperature = Images.TempBottomLimit;

                imageName = temperature.ToString();

                this.spriteRender.Draw(

                    this.spriteSheet.Sprite(imageName),
                    (Vector2) position,
                    null,
                    0,
                    0.055f
                );

            }
        }

    }
}
