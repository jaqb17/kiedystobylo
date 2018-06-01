﻿using Microsoft.Xna.Framework;
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
        private Camera camera;

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
            log.loadModel(content, path);
        }

        public void Initialization(Camera camera)
        {
            this.camera = camera;
            this.log.initializeClip("Take 001");
        }

        public void setPosition(Vector3 _position)
        {
            if (log.ifPlay) return;

            this.position = _position;
            this.log.world = Matrix.CreateTranslation(position);
            this.log.scaleAset(4f);
            //Console.WriteLine(this.position);
        }

        public void Draw(GameTime gameTime)
        {
            if (isLogCreated && this.log != null)
            {
                this.log.Draw(gameTime, camera.view, camera.projection);
            }
        }

        public void destroyLog()
        {
            if (this.log == null) return;

            Console.WriteLine("Zniszczono");
            this.log.ifDamageAfterPlay = true;
        }

        private void woodLogDestroyFailedToHit()
        {
            this.isLogCreated = false;
        }

        private void moveLog()
        {
            if (log.ifPlay) return;

            Vector3 newLogPosition = position;

            newLogPosition.X -= 1f;
            newLogPosition.Y = -(newLogPosition.X * newLogPosition.X) / 200 + 50;
            //newLogPosition.Y += 0.1f;
            //if (newLogPosition.X < 0f)
            //    newLogPosition.Y -= 0.1f;
            setPosition(newLogPosition);
            if (position.Y > 48.5f)
                isDestroyable = true;
            else
                isDestroyable = false;

            if (newLogPosition.X < -100f)
                woodLogDestroyFailedToHit();
        }

        public void Update(GameTime gameTime)
        {
            if (this.log == null) return;

            moveLog();

            if (this.log.HasAnimation)
            {
                //if (this.log.Clips.Count > 0)
                //    this.log.animationUpdate(gameTime);


                if (this.log.ifPlay)
                    this.log.animationUpdate(gameTime);

                if (!this.log.ifPlay && this.log.ifDamageAfterPlay)
                {
                    //this.log.PlayClip(this.log.Clips[0], false);
                    this.log.initializeClip("Take 001");
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
