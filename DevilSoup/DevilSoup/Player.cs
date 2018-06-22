using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Player
    {
        public int hp = 100;
        public int points = 0;
        private static Player instance = null;

        public static Player getPlayer()
        {
            if (instance == null)
                instance = new Player();

            return instance;
        }

        private Player()
        {
            this.hp = 100;
            this.points = 0;
        }

        public static void reset()
        {
            instance = new Player();
        }
        
    }
}
