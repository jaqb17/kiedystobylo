using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public abstract class Component
    {
        public GameObject Parent { get; set; }
        public abstract void UpdateComponent();
    }
}
