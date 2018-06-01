using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    class Water
    {
        Asset asset;
        public Vector3 position { get; set; }
        private float fireBoostValue { get; set; }
        public bool isDestroyable { get; set; }
        public Water()
        {
            Initialize();
        }

        public Water(ContentManager content, string path)
        {
            Initialize();
            asset = new Asset();
            asset.loadModel(content, path);
        }

        public void SetPosition(Vector3 _position)
        {
            this.position = _position;
            //this.log.world = Matrix.CreateTranslation(position);
            //this.log.scaleAset(0.3f);
            //Console.WriteLine(this.position);
        }

        public void Initialize()
        {
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
            fireBoostValue = -0.1f;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            
            if (this.asset != null)
                this.asset.Draw(gameTime, view, projection,new Vector3(0,255,0));
        }

        public void DestroyWater()
        {
            Console.WriteLine("Woda wzieta");
            //this.log.unloadModel();
            //this.log = null;
        }
    }
}
