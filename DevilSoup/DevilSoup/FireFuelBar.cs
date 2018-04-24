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
    public class FireFuelBar
    {
        public Rectangle barRectangle;
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        //int fuelValue;

        public FireFuelBar(Vector2 _position, string texturePath, ContentManager content)
        {
            position = _position;
            texture = content.Load<Texture2D>(texturePath);
            barRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            //fuelValue = texture.Width;
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
        //public float getFuelValue()
        //{
        //    return fuelValue;
        //}
        public bool isBarEmpty()
        {
            if (barRectangle.Width <= 0)
                return true;
            return false;
        }
    }   
}
