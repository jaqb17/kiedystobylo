﻿using Microsoft.Xna.Framework;
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

        public Fireplace(Vector2 _position, string texturePath, ContentManager content)
        {
            logsUnderCauldron = new WoodenLog[maxLogsUnderCauldron];
            for (int i = 0; i < maxLogsUnderCauldron; i++)
                logsUnderCauldron[i] = null;//logsUnderCauldron[i] = new WoodenLog();
                //logsUnderCauldron[i] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");
            logsUnderCauldron[0] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");
            logsUnderCauldron[1] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");

        }

        public void addLogBeneathCauldron()
        {
            for (int i = 0; i < maxLogsUnderCauldron; i++)
            {
                if (logsUnderCauldron[i] == null)
                {
                    //logsUnderCauldron[i] = new WoodenLog(content, "Assets\\Drewno\\DrewnoRozpad\\drewnoRoz");
                    logsUnderCauldron[i] = new WoodenLog();
                    break;
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
                if (logsUnderCauldron[i] != null && logsUnderCauldron[i].decayValue < 0)
                    logsUnderCauldron[i] = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            fuelValueChange();
            decay(0.001);
            removeLog();
        }

    }
}