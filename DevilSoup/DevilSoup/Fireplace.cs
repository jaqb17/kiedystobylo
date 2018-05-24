using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Fireplace
    {
        public Rectangle barRectangle;
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        private WoodenLog[] LogsUnderCauldron;
        public double fuelValue;
        private const int maxLogsInFire = 4;

        public Fireplace(Vector2 _position, string texturePath, ContentManager content)
        {
            position = _position;
            texture = content.Load<Texture2D>(texturePath);
            barRectangle = new Rectangle((int)_position.X, (int)_position.Y, 400, 400);
            fuelValue = 3;
            LogsUnderCauldron = new WoodenLog[4];
            for (int i = 0; i < maxLogsInFire; i++)
                LogsUnderCauldron[i] = null;
        }
        public void fuelValueChange (int changeValue)
        {
            //fuelValue += changeValue;
            //if (fuelValue >= 1)
            //    fuelValue = 1;
            //Console.WriteLine("Paliwo "+fuelValue);
            //Console.WriteLine("Szerokosc " + barRectangle.Width);
            //fuelValue += changeValue;
            barRectangle.Width += changeValue;
            if (barRectangle.Width > texture.Width)
                barRectangle.Width = texture.Width;
        }
        //public double getFuelValue()
        //{
        //    return fuelValue;
        //}
        public bool isBarEmpty()
        {
            if (barRectangle.Width <= 0)
                return true;
            return false;
        }
        public void addLogUnderCauldron()
        {
            //LogsUnderCauldron.Add
        }
        //public float calculateFireValue()
        //{
        //    return LogsUnderCauldron.Count;
        //}
        //public void updateLogDecayRatio()
        //{
        //    foreach (var log in LogsUnderCauldron)
        //    {
        //        log.decayValue -= 0.01;
        //        if (log.decayValue <= 0)
        //            LogsUnderCauldron.Remove(log);
        //    }
        //}

        public void drawWoodenLogs(GameTime gameTime, Matrix view, Matrix projection)
        {
          for (int i=0;i<maxLogsInFire;i++)
            {
                if (LogsUnderCauldron[i] != null)
                    LogsUnderCauldron[i].drawWoodenLog(gameTime, view, projection);
            }
        }
        
    }   
}
