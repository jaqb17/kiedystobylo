using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class SingleArea
    {
        public Vector3 areaCenter { get; private set; }
        public Vector3 soulPosition { get; private set; }
        private Soul soul;
        private String path = "Assets\\Souls\\bryla";

        public SingleArea(ContentManager content, Vector3 areaCenter)
        {
            this.areaCenter = areaCenter;
            this.soul = new Soul(content, this.path);
            this.soulPosition = this.areaCenter;
        }

        public void moveSoul(Vector3 position)
        {
            this.soulPosition = position;
            this.soul.setSoulPosition(this.soulPosition);
        }

        public void updateSoul(Matrix view, Matrix projection)
        {
            this.soul.drawSoul(view, projection);
        }
    }
}
