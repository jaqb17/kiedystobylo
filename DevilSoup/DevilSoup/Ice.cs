﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DevilSoup
{
    public class Ice
    {
        public Asset iceModel { get; private set; }
        public Vector3 position { get; set; }
        public double fireBoostValue { get; set; }
        public bool isDestroyable { get; set; }
        public bool isIceCreated { get; set; }
        public bool isIceDestroyed { get; set; }
        public bool isIceActive { get; set; }
        private Camera camera;

        public Ice()
        {
            isIceCreated = true;
            isDestroyable = false;
            isIceActive = true;
            position = new Vector3(150f, 0, 0);
        }

        public Ice(ContentManager content, GraphicsDevice graphicsDevice, string modelPath,
            string colorTexturePath, string normalTexturePath, string specularTexturePath,
            string shaderPath = "Assets/Effects/Refraction", string skyboxPath = "Assets/Skybox/helll")
        {
            isIceCreated = true;
            isDestroyable = false;
            isIceActive = true;
            isIceDestroyed = false;
            position = new Vector3(100f, 0, 10);
            iceModel = new Asset();
            fireBoostValue = -1.5;
            iceModel.loadModel(content, graphicsDevice, modelPath, normalTexturePath, shaderPath, skyboxPath);

            //iceModel.loadModel(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, shaderPath, specularTexturePath);   
        }

        public void Initialization(Camera camera)
        {
            this.camera = camera;
        }

        public void setPosition(Vector3 _position)
        {
            if (iceModel.ifPlay || !this.isIceActive) return;

            this.position = _position;
            this.iceModel.world = Matrix.CreateTranslation(position);
            this.iceModel.scaleAset(3.5f);
            //Console.WriteLine(this.position);
        }

        public void Draw(GameTime gameTime)
        {
            if (isIceCreated && isIceActive && this.iceModel != null)
            {
                this.iceModel.Draw(gameTime, camera.view, camera.projection);
            }
        }

        public void destroyIce()
        {
            if (this.iceModel == null || !this.isIceActive) return;

            Console.WriteLine("Zniszczono lod");
            this.iceModel.IfDamageAfterPlay = true;
        }

        private void iceCubeDestroyFailedToHit()
        {
            this.isIceCreated = false;
        }

        private void moveIce()
        {
            if (iceModel.ifPlay || !this.isIceActive) return;

            Vector3 newIcePosition = position;

            newIcePosition.X -= 1f;
            newIcePosition.Y = -(newIcePosition.X * newIcePosition.X) / 200 + 60;
            setPosition(newIcePosition);
            if (position.Y > 58.5f)
                isDestroyable = true;
            else
                isDestroyable = false;

            if (newIcePosition.X < -100f)
                iceCubeDestroyFailedToHit();
        }

        private void moveIce(Vector3 offset)
        {
            if (iceModel.ifPlay || !this.isIceActive) return;

            Vector3 newLogPosition = position;
            newLogPosition += offset / 3;
            setPosition(newLogPosition);
        }

        public void Update(GameTime gameTime)
        {
            if (this.iceModel == null || !this.isIceActive) return;

            moveIce();

            if (this.iceModel.HasAnimation)
            {

                if (!this.iceModel.ifPlay && this.iceModel.IfDamageAfterPlay)
                {
                    this.iceModel.ifPlay = true;
                }

                if (this.iceModel.finishedAnimation && this.iceModel.IfDamageAfterPlay)
                {
                    this.iceModel.unloadModel();
                    this.iceModel = null;
                    isIceCreated = false;
                }
            }
        }
    }
}
