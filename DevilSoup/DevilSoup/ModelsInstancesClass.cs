using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class ModelsInstancesClass
    {
        public static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public ModelsInstancesClass()
        {
            models.Add("Cauldron", null);
            models.Add("Soul", null);
            models.Add("Ice", null);
            models.Add("Wood", null);
            models.Add("Water", null);
        }
    }
}
