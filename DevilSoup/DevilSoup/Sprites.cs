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
        private HandTip handTip;
        private ContentManager content;
        private bool ifLogHaveFlownAlready = false;
        private ProgressBar progressBar;

        public Sprites()
        {
            handTip = new HandTip(2);
        }

        public void Initialize(ContentManager content)
        {
            this.content = content;
            this.font = content.Load<SpriteFont>("HP");
        }

        public void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            this.handTip.LoadContent(content, spriteBatch);
            this.spriteBatch = spriteBatch;
            progressBar = new ProgressBar(game, new Rectangle(10, 10, 500, 25));
            progressBar.minimum = 0;
            progressBar.maximum = 50;
        }
        
        public void Update(GameTime gameTime, DanceArea danceArea)
        {
            progressBar.maximum = danceArea.combo.pointsLimitToActivateCombo;
            progressBar.value = danceArea.combo.comboPointsProgressBar;
            progressBar.Update(gameTime);
        }

        public void Draw(DanceArea danceArea)
        {
            Player player = Player.getPlayer();
            if (!ifLogHaveFlownAlready && danceArea.ifLogHaveFlownAlready)
            {
                handTip.DisplayTip = true;
                ifLogHaveFlownAlready = true;
            }

            handTip.Draw();

            progressBar.Draw(spriteBatch);

            if (player.hp > 0 && danceArea.heatValue > 0 && danceArea.heatValue < 8.5)
            {
                switch (danceArea.level)
                {
                    case 0:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points +
                            "\nLEVEL: easy" + "\nStage: " + danceArea.stage, new Vector2(100, 100), Color.Black);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points +
                            "\nLEVEL: medium" + "\nStage: " + danceArea.stage, new Vector2(100, 100), Color.Black);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "HP: " + player.hp + "\nPOINTS: " + player.points +
                            "\nLEVEL: hard" + "\nStage: " + danceArea.stage, new Vector2(100, 100), Color.Black);
                        break;
                }

            }
            else
            {
                spriteBatch.DrawString(font, "Przegranko", new Vector2(100, 100), Color.Black);
            }

            spriteBatch.DrawString(font, "\n\nHV: " + danceArea.heatValue, new Vector2(100, 150), Color.Black);
            spriteBatch.DrawString(font, "\n\nFire Temperature: " + danceArea.fuelBar.fuelValue, new Vector2(100, 175), Color.Black);
        }
    }
}
