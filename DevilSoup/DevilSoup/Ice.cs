using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    class Ice
    {
        Asset asset;
        public Vector3 position { get; set; }
        private float fireBoostValue { get; set; }
        public bool isDestroyable { get; set; }
        public Ice()
        {
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
        }
        public Ice(ContentManager content, string path)
        {
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
            asset = new Asset();
            asset.loadModel(content, "Water", path);
            fireBoostValue = -0.1f;
        }
        public void setPosition(Vector3 _position)
        {
            this.position = _position;
            //this.log.world = Matrix.CreateTranslation(position);
            //this.log.scaleAset(0.3f);
            //Console.WriteLine(this.position);
        }
        public void drawWater(Matrix view, Matrix projection)
        {
            
            if (this.asset != null)
                this.asset.DrawModel(view, projection,new Vector3(0,255,0));
        }
        public void destroyWater()
        {
            Console.WriteLine("Woda wzieta");
            //this.log.unloadModel();
            //this.log = null;
        }
    }
}
