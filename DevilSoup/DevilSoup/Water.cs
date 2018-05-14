﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
        }
        public Water(ContentManager content, string path, Vector3 cameraPos, GraphicsDevice graphicsDevice)
        {
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
            asset = new Asset();
            asset.loadModel(content, path);
            asset.cameraPos = cameraPos;
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
                this.asset.DrawModel(view, projection, new Vector3(0,255,0));
        }
        public void destroyWater()
        {
            Console.WriteLine("Woda wzieta");
            //this.log.unloadModel();
            //this.log = null;
        }
    }
}
