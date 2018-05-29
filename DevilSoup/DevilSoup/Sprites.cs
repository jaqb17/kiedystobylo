using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Sprites
    {
        private SpriteFont font;
        private SpriteBatch spriteBatch;

        public Sprites() { }

        public void Initialize(ContentManager content)
        {
            this.font = content.Load<SpriteFont>("HP");
        }

        public void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void Draw(DanceArea danceArea)
        {
            Player player = Player.getPlayer();
            if (player.hp > 0)
            {
                switch (danceArea.level)
                {
                    case 0:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: easy", new Vector2(100, 100), Color.Black);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: medium", new Vector2(100, 100), Color.Black);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points + "\nLEVEL: hard", new Vector2(100, 100), Color.Black);
                        break;
                }

            }
            else
            {
                spriteBatch.DrawString(font, "Przegranko", new Vector2(100, 100), Color.Black);
            }

            spriteBatch.DrawString(font, "HV: " + danceArea.heatValue, new Vector2(100, 150), Color.Black);
            spriteBatch.DrawString(font, "Fire Temperature: " + danceArea.fuelBar.fuelValue, new Vector2(100, 175), Color.Black);
        }
    }
}
