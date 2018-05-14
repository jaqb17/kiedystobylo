using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class SingleArea
    {
        public Vector3 areaCenter { get; private set; }
        public Vector3 soulPosition { get; private set; }
        public bool ifSoulIsAlive { get; private set; }
        public Soul soul;
        private String path = "Assets\\Souls\\bryla";

        public SingleArea(ContentManager content, Vector3 areaCenter, Vector3 cameraPos, GraphicsDevice graphicsDevice)
        {
            this.areaCenter = areaCenter;
            this.soul = new Soul(content, this.path, cameraPos, graphicsDevice);
            this.soulPosition = this.areaCenter;
            this.ifSoulIsAlive = true;
        }

        public void moveSoul(Vector3 position)
        {
            if (this.soul == null)
                return;

            this.soulPosition = position;
            this.soul.setSoulPosition(this.soulPosition);
        }

        public void killWithAnimation(Matrix view, Matrix projection)
        {
            if (this.soul == null) return;

            ThreadStart starter = new ThreadStart(() => { this.soul.killSoulWithAnimation(view, projection); });
            starter += () =>
            {
                Console.WriteLine("Killed!");

                this.soul.killSoul();
                this.soul = null;
            };
            Thread animatedKill = new Thread(starter) { IsBackground = true };
            animatedKill.Name = "Animated killing thread";
            animatedKill.Start();
            this.soul.lifes = 0;
            this.ifSoulIsAlive = false;
        }

        public bool takeSoulLife()
        {
            if (this.soul == null) return true;
            if (this.soul.lifes == 0) return false;

            this.soul.lifes -= 1;
            if (this.soul.lifes <= 0)
            {
                this.ifSoulIsAlive = false;
                this.soul.killSoul();
                this.soul = null;
                return true;
            }
            return false;
        }

        public void updateSoul(Matrix view, Matrix projection)
        {
            if (soul != null)
                this.soul.drawSoul(view, projection);
        }

    }
}
