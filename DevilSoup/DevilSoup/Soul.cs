using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Soul
    {
        Asset soul;
        Vector3 soulPosition;

        public Soul(ContentManager content, String path)
        {
            soul = new Asset();
            soul.loadModel(content, "Assets\\Souls\\bryla");
        }

        public void setSoulPosition(Vector3 position)
        {
            this.soulPosition = position;
            this.soul.world = Matrix.CreateTranslation(position);
        }
        
        public void drawSoul(Matrix view, Matrix projection)
        {
            this.soul.DrawModel(view, projection);
        }
    }
}
