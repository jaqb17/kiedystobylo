using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class WoodenLog
    {
        public Asset log { get; private set; }
        public Vector3 position { get; set; }
        private float fireBoostValue { get; set; }
        public bool isDestroyable { get; set; }
        public bool isLogCreated { get; set; }

        public WoodenLog()
        {
            isLogCreated = true;
            isDestroyable = false;
            position = new Vector3(150f, 0, 0);
        }
        public WoodenLog(ContentManager content, string path)
        {
            isLogCreated = true;
            isDestroyable = false;
            position = new Vector3(100f, 0, 10);
            log = new Asset();

            //log.LoadContentFile(content, "Wood", path);
            log.loadModel(content, "Wood", path);
        }
        public void setPosition(Vector3 _position)
        {
            if (log.ifPlay) return;

            this.position = _position;
            this.log.world = Matrix.CreateTranslation(position);
            this.log.scaleAset(4f);
            //Console.WriteLine(this.position);
        }
        public void drawWoodenLog(GameTime gameTime, Matrix view, Matrix projection)
        {
            if (this.log != null)
            {
                this.log.DrawModel(view, projection);
                Update(gameTime);
            }
        }
        public void destroyLog()
        {
            if (this.log == null) return;

            Console.WriteLine("Zniszczono");
            this.log.ifDamageAfterPlay = true;
        }

        public void Update(GameTime gameTime)
        {
            if (this.log == null) return;

            if (this.log.HasAnimation())
            {
                if (this.log.Clips.Count > 0)
                    this.log.animationUpdate(gameTime);

                if (!this.log.ifPlay && this.log.ifDamageAfterPlay)
                {
                    this.log.PlayClip(this.log.Clips[0], false);
                    this.log.ifPlay = true;
                }

                if (this.log.finishedAnimation && this.log.ifDamageAfterPlay)
                {
                    this.log.unloadModel();
                    this.log = null;
                    isLogCreated = false;
                }
            }
        }
    }
}
