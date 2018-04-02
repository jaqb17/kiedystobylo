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
        private Vector3 cauldronPosition;
        private SingleArea[] singleAreas;

        public DanceArea(Vector3 cauldronPosition)
        {
            this.cauldronPosition = cauldronPosition;
            singleAreas = new SingleArea[numberOfAreas];
        }

        public void createSoul(ContentManager content)
        {
            singleAreas[0] = new SingleArea(content, cauldronPosition);
        }

        public void moveSoul(Matrix view, Matrix projection)
        {
            Vector3 newPos = singleAreas[0].soulPosition;
            newPos.Y += 0.05f;
            singleAreas[0].moveSoul(newPos);

            updateSoul(view, projection);
        }

        private void updateSoul(Matrix view, Matrix projection)
        {
            singleAreas[0].updateSoul(view, projection);
        }
    }
}
