using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using TexturePackerLoader;

namespace DevilSoup
{
    public class HandTip
    {
        SpriteSheetLoader spriteSheetLoader;
        SpriteSheet spriteSheet;
        SpriteBatch spriteBatch;
        SpriteRender spriteRender;

        int imageID = 1;
        string imageName = HandWave.Hand_001;
        int frameDelay = 0;
        int frameLimit = 5;
        int currentLoop = 0;
        int loopsLimit = 2;
        bool displayTip = false;

        public bool DisplayTip
        {
            get { return displayTip; }
            set { displayTip = value; }
        }

        public HandTip(int howManyLoops)
        {
            this.loopsLimit = howManyLoops;
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            spriteSheetLoader = new SpriteSheetLoader(content);
            spriteSheet = spriteSheetLoader.Load("Assets\\handTipAnim\\animation");
            this.spriteBatch = spriteBatch;
            spriteRender = new SpriteRender(this.spriteBatch);
        }

        public void Draw()
        {
            // render the scene to the active render target

            if (displayTip)
            {
                frameDelay++;
                if (frameDelay > frameLimit)
                {
                    imageName = "00" + imageID;
                    imageID++;

                    if (imageID > 8)
                    {
                        imageID = 1;
                        currentLoop++;
                    }

                    frameDelay = 0;

                    if (currentLoop >= loopsLimit)
                        displayTip = false;
                }

                this.spriteRender.Draw(

                    this.spriteSheet.Sprite(imageName),
                    new Vector2(GlobalVariables.SCREEN_WIDTH * 4 / 5, GlobalVariables.SCREEN_HEIGTH * 3 / 5));

            }

        }
    }
}

