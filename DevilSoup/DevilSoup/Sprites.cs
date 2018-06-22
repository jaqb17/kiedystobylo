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
        private SpriteFont endFont;
        private SpriteBatch spriteBatch;
        private HandTip handTip;
        private ContentManager content;
        private bool ifLogHaveFlownAlready = false;
        private ProgressBar progressBar;
        private Color textColor = Color.White;
        private float przesuwanie = 90;
        private Vector2 center;
        private Thermometer thermometer;
        private HPTexture hpTexture;

        public Sprites()
        {
            handTip = new HandTip(2);
            hpTexture = new HPTexture(new Vector2(80, 30));
            thermometer = new Thermometer(new Vector2(80, 200));
        }

        public void Initialize(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.content = content;
            this.font = content.Load<SpriteFont>("HP");
            this.endFont = content.Load<SpriteFont>("CHuj");
            this.center = getCenterCoord(graphics);
        }

        public void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            this.handTip.LoadContent(content, spriteBatch);
            this.hpTexture.LoadContent(content, spriteBatch);
            this.thermometer.LoadContent(content, spriteBatch);
            this.spriteBatch = spriteBatch;
            progressBar = new ProgressBar(game, new Rectangle(GlobalVariables.SCREEN_WIDTH / 4, 10, GlobalVariables.SCREEN_WIDTH / 2, 25));
            progressBar.minimum = 0;
            progressBar.maximum = 50;
        }

        public Vector2 getCenterCoord(GraphicsDeviceManager graphics)
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Width/2, graphics.GraphicsDevice.Viewport.Height/2);
        }

        public void Update(GameTime gameTime, DanceArea danceArea)
        {
            thermometer.UpdateTemperature(danceArea.fuelBar.fuelValue);
            progressBar.maximum = danceArea.combo.pointsLimitToActivateCombo;
            progressBar.value = danceArea.combo.comboPointsProgressBar;
            progressBar.Update(gameTime);
        }

        public void Draw(DanceArea danceArea)
        {
            Player player = Player.getPlayer();
            if (danceArea.isLogCreated && !danceArea.ifLogHaveFlownAlready)
            {
                handTip.DisplayTip = true;
                danceArea.ifLogHaveFlownAlready = true;
            }
            
            thermometer.Draw();
            hpTexture.Draw();
            handTip.Draw();

            progressBar.Draw(spriteBatch);

            if (player.hp > 0 && danceArea.heatValue > 0 && danceArea.heatValue < 8.5)
            {
                spriteBatch.DrawString(font, "" + player.hp, new Vector2(155, 80), new Color(255, 246, 186));
                spriteBatch.DrawString(font, "Stage: " + danceArea.stage, new Vector2(GlobalVariables.SCREEN_WIDTH * 4 / 5, 80), textColor);
                spriteBatch.DrawString(font, "Points: " + player.points, new Vector2(GlobalVariables.SCREEN_WIDTH * 4 / 5, 230), textColor);
                /*switch (danceArea.level)
                {
                    case 0:
                        spriteBatch.DrawString(font, "LEVEL: easy" , new Vector2(50, 174 + przesuwanie), textColor);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "LEVEL: medium", new Vector2(50, 174 + przesuwanie), textColor);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "LEVEL: hard", new Vector2(50, 174 + przesuwanie), textColor);
                        break;
                }
                spriteBatch.DrawString(font, "HV: " + Math.Round(danceArea.heatValue, 1), new Vector2(50, 248 + przesuwanie), textColor);*/
                spriteBatch.DrawString(font, "" + danceArea.fuelBar.fuelValue * 200, new Vector2(155, 230), textColor);
            }
            else
            {                
                spriteBatch.DrawString(endFont, "Przegranko", center - new Vector2(185,82), new Color(206, 2, 2));
                spriteBatch.DrawString(font, "SCORE: " + player.points, center - new Vector2(60, -10), textColor);
            }

          
        }
    }
}
