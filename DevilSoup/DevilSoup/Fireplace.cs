using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Fireplace
    {

        public WoodenLog[] logsUnderCauldron { get; set; }
        public double fuelValue { get; set; }
        public const int maxLogsUnderCauldron = 5;
        private Vector3[] positionVectors;
        private Vector3 position1, position2, position3, position4, position5;
        GraphicsDevice graphicsDevice;

        public Fireplace(ContentManager content, GraphicsDevice graphicsDevice)
        {
            #region Positions for logs under Cauldron
            positionVectors = new Vector3[maxLogsUnderCauldron];
            positionVectors[0] = new Vector3(-30f, 0f, 35f);
            positionVectors[1] = new Vector3(-20f, 0f, 44f);
            positionVectors[2] = new Vector3(0, 0, 50f);
            positionVectors[3] = new Vector3(20f, 0f, 44f);
            positionVectors[4] = new Vector3(30f, 0f, 35f);
            #endregion
            logsUnderCauldron = new WoodenLog[maxLogsUnderCauldron];
            for (int i = 0; i < maxLogsUnderCauldron; i++)
                logsUnderCauldron[i] = null;//logsUnderCauldron[i] = new WoodenLog();
                                            //logsUnderCauldron[i] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");

            string modelPath = "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz";
            string colorTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Albedo";
            string normalTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Normal";
            string specularTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Metallic";

            this.graphicsDevice = graphicsDevice;
            logsUnderCauldron[0] = new WoodenLog(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, specularTexturePath, positionVectors[0]);
            logsUnderCauldron[1] = new WoodenLog(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, specularTexturePath, positionVectors[1]);
            logsUnderCauldron[0].isWoodActive = true;
            logsUnderCauldron[1].isWoodActive = true;
        }

        public void addLogBeneathCauldron(ContentManager content, Camera camera)
        {

            string modelPath = "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz";
            string colorTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Albedo";
            string normalTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Normal";
            string specularTexturePath = "Assets\\Drewno\\DrewnoRozpad\\drewnoR_Metallic";

            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] == null)
                {
                    logsUnderCauldron[i] = new WoodenLog(content, graphicsDevice, modelPath, colorTexturePath, normalTexturePath, specularTexturePath, positionVectors[i]);
                    logsUnderCauldron[i].isWoodActive = true;
                    logsUnderCauldron[i].Initialization(camera);
                    break;
                }
            }
        }

        public void Initialization(Camera camera)
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null)
                {
                    //logsUnderCauldron[i] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");
                    logsUnderCauldron[i].Initialization(camera);
                }
            }
        }

        private void fuelValueChange()
        {
            double _value = 0;
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null)
                    _value += logsUnderCauldron[i].fireBoostValue;
            }
            fuelValue = _value;
        }

        private void decay(double _value)
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null)
                    logsUnderCauldron[i].decayValue -= _value;
            }
        }

        private void removeLog()
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null && logsUnderCauldron[i].decayValue < 2)
                    logsUnderCauldron[i] = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null)
                {
                    logsUnderCauldron[i].StaticUpdate(gameTime);
                }
            }
            fuelValueChange();
            decay(0.001);
            removeLog();
        }

        private Vector3 computePosition(int id)
        {
            Vector3 result = new Vector3(0f, 0f, 0f);
            //Vector3 result = new Vector3(1.484828f, 25.99457f, 4.579611f);
            float radius = 1f;
            float angle = (float)(id * 360.0f / (maxLogsUnderCauldron * 2) * Math.PI / 180.0f);
            result.X += (float)(1.0f * radius * Math.Cos(angle));
            result.Y += (float)(1.0f * radius * Math.Sin(angle));

            return result;
        }

        public void Draw(GameTime gameTime, Camera _camera)
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] != null)
                {
                    logsUnderCauldron[i].Initialization(_camera);
                    Vector3 factor = computePosition(i);
                    System.Console.WriteLine(factor);
                    logsUnderCauldron[i].log.rotateAsset((int)(factor.X * 90), (int)(factor.Y * 90), 0);
                    logsUnderCauldron[i].log.scaleAset(2.8f, 3.8f, 1.8f);
                    logsUnderCauldron[i].Draw(gameTime);
                }
            }

        }

    }
}
