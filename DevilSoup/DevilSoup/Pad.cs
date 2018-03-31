using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public static class Pad
    {
        private static KeyboardState keyboardState;
        private static GamePadState gamepadState;
        private static PlayerIndex conntectedPadId;

        public static void getKeyState()
        {
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(conntectedPadId);             // Ważne! Trzeba wybrać ID pada!

           // findConnectedPad();

            Console.WriteLine(conntectedPadId + " " +gamepadState.IsButtonDown(Buttons.A));

            /*
            //if (keyboardState.IsKeyDown(Keys.J))

            keys = keyboardState.GetPressedKeys();
            string _stringValue = string.Empty;

            foreach(Keys key in keys)
            {
                var keyValue = key.ToString();
                _stringValue += keyValue;
            }
            Console.WriteLine(_stringValue);
            */
        }

        public static void findConnectedPad()
        {

            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                GamePadState state = GamePad.GetState(i);
                if (state.IsConnected)
                {
                    conntectedPadId = i;
                    Console.WriteLine("Znaleziono!!! " + i);
                    break;
                }
            }
        }
    }
}
