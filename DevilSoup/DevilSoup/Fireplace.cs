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
        public Rectangle barRectangle;
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        private WoodenLog[] LogsUnderCauldron;
        public double fuelValue;
        private const int maxLogsInFire = 4;

        private Vector3[] logsPositions;

        public Fireplace(Vector2 _position, string texturePath, ContentManager content)
        {
            logsPositions = new Vector3[maxLogsInFire];
            position = _position;
            texture = content.Load<Texture2D>(texturePath);
            barRectangle = new Rectangle((int)_position.X, (int)_position.Y, 400, 400);
            fuelValue = 3;
            LogsUnderCauldron = new WoodenLog[maxLogsInFire];
            for (int i = 0; i < maxLogsInFire; i++)
                logsPositions[i] = new Vector3(i * 20f - 30f, 0, 5f);
            for (int i = 0; i < maxLogsInFire; i++)
                LogsUnderCauldron[i] = null;
            LogsUnderCauldron[0] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[0]);
            LogsUnderCauldron[1] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[1]);
        }
        public void fuelValueChange (int changeValue)
        {
            barRectangle.Width += changeValue;
            if (barRectangle.Width > texture.Width)
                barRectangle.Width = texture.Width;
        }
        public bool isBarEmpty()
        {
            if (barRectangle.Width <= 0)
                return true;
            return false;
        }
        public void addLogUnderCauldron(ContentManager content)
        {
            if(LogsUnderCauldron[0]==null)
            {
                LogsUnderCauldron[0] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[0]);
                return;
            }
            if (LogsUnderCauldron[1] == null)
            {
                LogsUnderCauldron[1] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[1]);
                return;
            }
            if (LogsUnderCauldron[2] == null)
            {
                LogsUnderCauldron[2] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[2]);
                return;
            }
            if (LogsUnderCauldron[3] == null)
            {
                LogsUnderCauldron[3] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz", logsPositions[3]);
                return;
            }
        }
        public void calculateFuelValue()
        {
            double fireValue=0;
            for(int i = 0;i<maxLogsInFire;i++)
            {
                if (LogsUnderCauldron[i] != null)
                    fireValue += 2;
            }
            fuelValue = fireValue;
        }
        public void updateLogDecayRatio(double _decayTick)
        {
            for(int i=0;i<maxLogsInFire;i++)
            {
                if (LogsUnderCauldron[i] != null)
                {
                    LogsUnderCauldron[i].decayValue -= _decayTick;
                    if (LogsUnderCauldron[i].decayValue <= 0)
                        LogsUnderCauldron[i] = null;
                }
            }
         
        }

        public void drawWoodenLogs(GameTime gameTime, Matrix view, Matrix projection)
        {
          for (int i=0;i<maxLogsInFire;i++)
            {
                if (LogsUnderCauldron[i] != null)
                    LogsUnderCauldron[i].drawWoodenLog(gameTime, view, projection);
            }
        }
        
    }   
}
