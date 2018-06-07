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
        public double fireBoostValue { get; set; }
        public bool isDestroyable { get; set; }
        public bool isLogCreated { get; set; }

        public bool isLogDestroyed { get; set; }

        public bool isWoodActive { get; set; }

        private Camera camera;
        public double decayValue { get; set; }
        public WoodenLog()
        {
            isLogCreated = true;
            isDestroyable = false;
            isWoodActive = false;
            position = new Vector3(150f, 0, 0);
            fireBoostValue = 2;
            decayValue = 3;
        }

        public WoodenLog(ContentManager content, string path)
        {
            isLogCreated = true;
            isDestroyable = false;
            isWoodActive = false;
            position = new Vector3(110f, 0, 10);
            log = new Asset();
            fireBoostValue = 2;
            decayValue = 3;
            //log.LoadContentFile(content, "Wood", path);
            log.loadModel(content, path);

            isLogDestroyed = false;
        }

        public WoodenLog(ContentManager content, string path, Vector3 _position)
        {
            isLogCreated = true;
            isDestroyable = false;
            isWoodActive = false;
            //position = new Vector3(100f, 0, 10);
            log = new Asset();
            fireBoostValue = 2;
            decayValue = 3;
            //log.LoadContentFile(content, "Wood", path);
            log.loadModel(content, path);

            isLogDestroyed = false;
            position = _position;
        }

        public void Initialization(Camera camera)
        {
            this.camera = camera;
        }

        public void setPosition(Vector3 _position)
        {
            if (log.ifPlay || !this.isWoodActive) return;

            this.position = _position;
            this.log.world = Matrix.CreateTranslation(position);
            this.log.scaleAset(4f);
            
        }


        public void Draw(GameTime gameTime)
        {
            if (isLogCreated && isWoodActive && this.log != null)
            {
               this.log.Draw(gameTime, camera.view, camera.projection);
            }
        }

        public void destroyLog()
        {
            if (this.log == null || !this.isWoodActive) return;

            Console.WriteLine("Zniszczono drewno");
            this.log.ifDamageAfterPlay = true;
        }

        private void woodLogDestroyFailedToHit()
        {
            this.isLogCreated = false;
        }

        private void moveLog()
        {
            if (log.ifPlay || !this.isWoodActive) return;

            Vector3 newLogPosition = position;

            newLogPosition.X -= 1f;
            newLogPosition.Y = -(newLogPosition.X * newLogPosition.X) / 200 + 50;
            setPosition(newLogPosition);
            if (position.Y > 48.5f)
                isDestroyable = true;
            else
                isDestroyable = false;

            if (newLogPosition.X < -110f)
                woodLogDestroyFailedToHit();
        }

        private void moveLog(Vector3 offset)
        {
            if (log.ifPlay || !this.isWoodActive) return;

            Vector3 newLogPosition = position;
            newLogPosition += offset / 3;
            setPosition(newLogPosition);
        }

        private void toDraw()
        {
            this.log.world = Matrix.CreateTranslation(position);
            this.log.scaleAset((float)decayValue);
        }

        public void StaticUpdate(GameTime gameTime)
        {
            if (this.log == null || !this.isWoodActive) return;
            toDraw();
            if (this.log.HasAnimation())
            {
                if (this.log.Clips.Count > 0)
                    this.log.animationUpdate(gameTime);

                if (!this.log.ifPlay && this.log.ifDamageAfterPlay)
                {
                    this.log.PlayClip(this.log.Clips[0]);
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

        public void Update(GameTime gameTime)
        {
            if (this.log == null || !this.isWoodActive) return;

            moveLog();

            if (this.log.HasAnimation())
            {
                if (this.log.Clips.Count > 0)
                    this.log.animationUpdate(gameTime);

                if (!this.log.ifPlay && this.log.ifDamageAfterPlay)
                {
                    this.log.PlayClip(this.log.Clips[0]);
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
