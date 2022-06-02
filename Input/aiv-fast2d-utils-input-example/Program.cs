using System;

namespace Aiv.Fast2D.Utils.Input.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Window window = new Window(800, 600, "TEST");

            // a bit more structured pattern, where you register listener and you can get a callback when that value changes.
            Input.RegisterListener(AivKeyCode.L, OnLButton);
            Input.RegisterListener(AivAxisCode.Joystick1LeftHorizontal, OnAxis);
            
            // Input.UnregisterListener(AivKeyCode.L, OnLButton); // when you wanna stop listening unregister it like this

            while (window.opened)
            {
                Input.Update(window);

                // classic input polling for the space bar
                if (Input.WasPressed(AivKeyCode.Space))
                {
                    Console.WriteLine("space bar was pressed");
                }

                if (Input.WasReleased(AivKeyCode.Space))
                {
                    Console.WriteLine("space bar was released");
                }

                // support joysticks as well
                if (Input.WasPressed(AivKeyCode.Joystick1ButtonA))
                {
                    Console.WriteLine("JOYSTICK KEY UP");
                }

                // classic polling pattern but for axis (this is the left stick of a joypad moving vertically). goes from -1 to 1 for bottom and top
                float hor = Input.GetAxis(AivAxisCode.Joystick1LeftVertical);

                window.Update();
            }
        }

        static void OnLButton(bool state)
        {
            Console.WriteLine("from event state " + state);
        }

        static void OnAxis(float val)
        {
            Console.WriteLine(val);
        }
    }
}