using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class DanceArea
    {

        private const int numberOfAreas = 8;
        private Vector3 origin;
        private SingleArea[] singleAreas;
        private float radius;
       // private Game1 game1;
        public float escape_height = 51.0f;
        public int level = 0;


        public DanceArea(Asset cauldron)
        {
            this.radius = cauldron.radius / 3;
            this.origin = cauldron.center;
            singleAreas = new SingleArea[numberOfAreas];
        }

        private Vector3 computePosition(Vector3 origin, float radius, int id)
        {
            Vector3 result = origin;
            float angle = (float) (id * 360.0f / numberOfAreas * Math.PI / 180.0f);
            result.X += (float) (radius * Math.Cos(angle));
            result.Z += (float)(radius * Math.Sin(angle));

            return result;
        }

        public void createSoul(ContentManager content)
        {

            int i = Randomizer.GetRandomNumber(0, numberOfAreas);
            if (singleAreas[i] == null || singleAreas[i].soul == null)          
            {
                
                singleAreas[i] = new SingleArea(content, computePosition(origin, radius, i));
            }
                
        }

        /*
        public void reset()
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if (singleAreas[i] != null) 
                {
                    if(singleAreas[i].soul!=null)
                    {
                        singleAreas[i].soul.killSoul();
                        singleAreas[i].soul = null;
                    }
                } 

            }
        }*/
        public void moveSoul(Matrix view, Matrix projection)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {                
                if (singleAreas[i] != null && singleAreas[i].ifSoulIsAlive)
                {
                    Vector3 newPos = singleAreas[i].soulPosition;
                    newPos.Y += 0.05f;
                    Console.WriteLine("y " + newPos.Y);
                    singleAreas[i].moveSoul(newPos);
                    updateSoul(view, projection);
                   // if(newPos.Y <= escape_height)
                   // {
                        //game1.Escaped(singleAreas[i].soul.lifes * 10);
                       // singleAreas[i].soul.killSoul();
                   // }
                    //else if(singleAreas[i].soul.lifes<=0)
                   // {
                       // game1.Killed();
                       // singleAreas[i].soul.killSoul();
                        
                   // }
                }

            }
        }

        private void updateSoul(Matrix view, Matrix projection)
        {
            for (int i = 0; i < numberOfAreas; i++)
            {
                if(singleAreas[i] != null)
                singleAreas[i].updateSoul(view, projection);
            }

        }
    }
}
