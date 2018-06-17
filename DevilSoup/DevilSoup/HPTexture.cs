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
    public class HPTexture
    {
        SpriteSheetLoader spriteSheetLoader;
        SpriteSheet spriteSheet;
        SpriteBatch spriteBatch;
        SpriteRender spriteRender;
        bool display = true;
        Player player;
        Vector2? position = null;

        string imageName = Images.HP_100;

        public bool Display
        {
            get { return display; }
            set { display = value; }
        }

        public HPTexture(Vector2 position)
        {
            this.position = position;
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            spriteSheetLoader = new SpriteSheetLoader(content);
            spriteSheet = spriteSheetLoader.Load("Assets\\GUI\\Hp\\meat");
            this.spriteBatch = spriteBatch;
            spriteRender = new SpriteRender(this.spriteBatch);
        }

        public void Draw()
        {
            if (display)
            {
                player = Player.getPlayer();
                imageName = player.hp.ToString();

                if (player.hp < 0) imageName = 0.ToString();

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
