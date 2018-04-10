using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;

namespace DevilSoup
{
    public class Pad
    {
        DirectInput Input = new DirectInput();
        Joystick stick;
        Joystick[] Sticks;
        List<int> connectedPadsId;
        bool[] lastButtons;

        //Thumstick variables.
        int yValue = 0;
        int xValue = 0;
        int zValue = 0;
        int rotationZValue = 0;

        public Pad()
        {
            //GetSticks();
            Sticks = GetSticks();
            connectedPadsId = new List<int>();
        }

        private Joystick[] GetSticks()
        {

            // Creates the list of joysticks connected to the computer via USB.
            List<Joystick> sticks = new List<Joystick>();

            foreach (DeviceInstance device in Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // Creates a joystick for each game device in USB Ports
                try
                {
                    stick = new Joystick(Input, device.InstanceGuid);
                    stick.Acquire();

                    // Gets the joysticks properties and sets the range for them.
                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
                    }

                    // Adds how ever many joysticks are connected to the computer into the sticks list.
                    sticks.Add(stick);
                }
                catch (DirectInputException)
                {
                    Console.WriteLine("Blad przy rozpoznawaniu joysticka");
                }
            }
            return sticks.ToArray();
        }

        public int getKeyState()
        {
            int result = -1;
            for (int i = 0; i < Sticks.Length; i++)
            {
                result = StickHandlingLogic(Sticks[i], i);
            }

            return result;
        }

        private int StickHandlingLogic(Joystick stick, int id)
        {
            // Creates an object from the class JoystickState.
            JoystickState state = new JoystickState();

            state = stick.GetCurrentState(); //Gets the state of the joystick

            //These are for the thumbstick readings
            yValue = -state.Y;
            xValue = state.X;
            zValue = state.Z;
            rotationZValue = -state.RotationZ;

            // Stores the number of each button on the gamepad into the bool[] butons.
            bool[] buttons = state.GetButtons();

            if (lastButtons == null) lastButtons = buttons;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Ponizej zamiescilem przyklad obslugi gamepada. Za pomoca id mozna zdefiniowac, z ktorego pada korzystamy.//
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (id == 0)
            {
                // This is when button 0 of the gamepad is pressed, the label will change. Button 0 should be the square button.
                for (int i = 0; i < buttons.Length; i++)
                {
                    if ((buttons[i] && !lastButtons[i]) || (buttons[i] && (i == 8 || i == 9)))
                    {
                        Console.WriteLine("Wcisnieto przycisk: " + i);
                        lastButtons = buttons;
                        return i;
                    }
                }
            }

            lastButtons = buttons;
            return -1;
        }
    }
}
