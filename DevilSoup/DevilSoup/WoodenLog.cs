using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    class WoodenLog
    {
        Asset log;
        public Vector3 position { get; set; }
        //private float fireBoostValue { get; set;}
        public bool isDestroyable { get; set; }
        public WoodenLog()
        {
            isDestroyable = false;
            position = new Vector3(200f, 0, 0);
        }
        public WoodenLog(ContentManager content, string path)
        {
            isDestroyable = false;
            position = new Vector3(200f, 0, 0);
            log = new Asset();
            log.loadModel(content, path);
        }
        public void setPosition(Vector3 _position)
        {
            this.position = _position;
            //this.log.world = Matrix.CreateTranslation(position);
            //this.log.scaleAset(0.3f);
            Console.WriteLine(this.position);
        }
        public void drawWoodenLog(Matrix view, Matrix projection)
        {
            if (this.log != null)
                this.log.DrawModel(view, projection);
        }
        public void destroyLog()
        {
            Console.WriteLine("Zniszczono");
            //this.log.unloadModel();
            //this.log = null;
        }
    }
}
