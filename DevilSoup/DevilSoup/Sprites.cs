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
        private Texture2D hpIcon;
        private Texture2D fireIcon;

        public Sprites()
        {
            handTip = new HandTip(2);
        }

        public void Initialize(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.content = content;
            this.font = content.Load<SpriteFont>("HP");
            this.endFont = content.Load<SpriteFont>("CHuj");
            this.center = getCenterCoord(graphics);
            this.hpIcon = content.Load<Texture2D>("Assets/HP");
            this.fireIcon = content.Load<Texture2D>("Assets/Fire");

        }

        public void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            this.handTip.LoadContent(content, spriteBatch);
            this.spriteBatch = spriteBatch;
            progressBar = new ProgressBar(game, new Rectangle(10, 10, 500, 25));
            progressBar.minimum = 0;
            progressBar.maximum = 50;
        }

        public Vector2 getCenterCoord(GraphicsDeviceManager graphics)
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Width/2, graphics.GraphicsDevice.Viewport.Height/2);
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
                spriteBatch.Draw(hpIcon, new Vector2(50, przesuwanie - 20), null, Color.White, 0f, new Vector2(0, 0), .6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "" + player.hp, new Vector2(110, przesuwanie + 50), new Color(255, 246, 186));
                spriteBatch.DrawString(font, "POINTS: " + player.points, new Vector2(50, 137+ przesuwanie), textColor);
                switch (danceArea.level)
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
                spriteBatch.DrawString(font, "Stage: " + danceArea.stage, new Vector2(50, 211 + przesuwanie), textColor);
                spriteBatch.DrawString(font, "HV: " + Math.Round(danceArea.heatValue, 1), new Vector2(50, 248 + przesuwanie), textColor);
                spriteBatch.DrawString(font, "" + danceArea.fuelBar.fuelValue, new Vector2(110, 390+ przesuwanie), textColor);
                spriteBatch.Draw(fireIcon, new Vector2(50, 300 + przesuwanie), null, Color.White, 0f, new Vector2(0, 0), .6f, SpriteEffects.None, 0f);
            }
            else
            {                
                spriteBatch.DrawString(endFont, "Przegranko", center - new Vector2(185,82), new Color(206, 2, 2));
                spriteBatch.DrawString(font, "SCORE: " + player.points, center - new Vector2(60, -10), textColor);
            }

          
        }
    }
}
