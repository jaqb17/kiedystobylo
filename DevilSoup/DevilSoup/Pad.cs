using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;
using WiimoteLib;
using Microsoft.Xna.Framework;

namespace DevilSoup
{
    public class Pad
    {
        DirectInput Input = new DirectInput();
        //Joystick stick;
        public Joystick USBMatt { get; }
        //Joystick[] Sticks;
        Wiimote wiimote;
        List<int> connectedPadsId;
        bool[] lastButtons;

        public Pad()
        {
            //GetSticks();
            //Sticks = GetSticks();
            USBMatt = USBDanceMattInitialize();
            connectedPadsId = new List<int>();
            wiimote = new Wiimote();
            connectWiiremote();
        }
        private void connectWiiremote()
        {
            try
            {
                wiimote.Connect();
                wiimote.SetReportType(InputReport.IRAccel, true);
                wiimote.SetLEDs(true, false, false, false);
            }
            catch { Console.WriteLine("Can't find a Wiimote"); }

        }

        private Joystick USBDanceMattInitialize()
        {
            foreach (DeviceInstance device in Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                Joystick _currentlyRead;
                // Creates a joystick for each game device in USB Ports
                try
                {
                    _currentlyRead = new Joystick(Input, device.InstanceGuid);
                    _currentlyRead.Acquire();

                    // Gets the joysticks properties and sets the range for them.
                    foreach (DeviceObjectInstance deviceObject in _currentlyRead.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            _currentlyRead.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
                    }

                    // Adds how ever many joysticks are connected to the computer into the sticks list.
                    if (_currentlyRead.Information.ProductName == "USB Gamepad ")
                        return _currentlyRead;

                }
                catch (DirectInputException)
                {
                    Console.WriteLine("Blad przy rozpoznawaniu joysticka");
                }
            }
            return null;
        }

        public int getKeyState()
        {
            int result = -1;
            //for (int i = 0; i < Sticks.Length; i++)
            //{
            //    result = StickHandlingLogic(i);
            //}
            result = StickHandlingLogic(USBMatt);
            return result;
        }

        private int StickHandlingLogic(Joystick _var)
        {
            // Creates an object from the class JoystickState.
            JoystickState state = new JoystickState();
            state = _var.GetCurrentState(); //Gets the state of the joystick

            // Stores the number of each button on the gamepad into the bool[] butons.
            bool[] buttons = state.GetButtons();

            if (lastButtons == null) lastButtons = buttons;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Ponizej zamiescilem przyklad obslugi gamepada. Za pomoca id mozna zdefiniowac, z ktorego pada korzystamy.//
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //if (_id == 0) //stick.Information.ProductName == "USB Gamepad " id==0
            //{
            // This is when button 0 of the gamepad is pressed, the label will change. Button 0 should be the square button.
            for (int i = 0; i < buttons.Length; i++)
            {
                if ((buttons[i] && !lastButtons[i]) || (buttons[i] && i == 9))
                {
                    Console.WriteLine("Wcisnieto przycisk: " + i);
                    lastButtons = buttons;
                    return i;
                }
            }
            // }

            lastButtons = buttons;
            return -1;
        }

        public double swung()
        {
            WiimoteState state = wiimote.WiimoteState;
            Point3F accelVector = state.AccelState.Values;
            return Math.Sqrt(accelVector.X * accelVector.X + accelVector.Z * accelVector.Z + accelVector.Y * accelVector.Y);
        }

        public Vector3 accelerometerStatus()
        {
            WiimoteState state = wiimote.WiimoteState;

            Point3F accelVector = state.AccelState.Values;

            Vector3 result = new Vector3(accelVector.X, accelVector.Y, accelVector.Z);
            if (Math.Abs(result.X) <= 1.1f) result.X = 0f;
            if (Math.Abs(result.Y) <= 1.1f) result.Y = 0f;
            if (Math.Abs(result.Z) <= 1.1f) result.Z = 0f;
            Console.WriteLine(result.ToString());
            return result;
        }

    }
}
