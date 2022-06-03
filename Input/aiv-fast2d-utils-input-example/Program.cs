using System;
using Aiv.Fast2D.Utils.Input.ActionsMapping;

namespace Aiv.Fast2D.Utils.Input.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            InputContextExample();
            return;
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

        // import from a csv?
        // format like: move left, A, Left,. dunno how to diff axes from keys. maybe if not key is an axis? if not both you fucked up
        static void InputContextExample()
        {
            Window window = new Window(800, 600, "TEST");

            // setup the actions and contexts

            // while on foot we can move left and right
            InputAction MoveLeft = new InputAction() { Name = "Move left" };
            InputAction MoveRight = new InputAction() { Name = "Move right" };

            // when we jump on mecha godzilla we can strafe left and right.
            // they are conceptually different actions to perform even tho bound to the same keys
            InputAction StrafeLeft = new InputAction() { Name = "Strafe left" };
            InputAction StrafeRight = new InputAction() { Name = "Strafe right" };

            // setup the walk context with the actions that we can perform and with which button.
            InputContext WalkCtx = new InputContext();
            InputMapping MoveLeftMapping = new InputMapping() { Action = MoveLeft };
            MoveLeftMapping.KeysToBind.Add(AivKeyCode.A);
            MoveLeftMapping.KeysToBind.Add(AivKeyCode.Left);
            InputMapping MoveRightMapping = new InputMapping() { Action = MoveRight };
            MoveRightMapping.KeysToBind.Add(AivKeyCode.D);
            WalkCtx.Mappings.Add(MoveLeftMapping);
            WalkCtx.Mappings.Add(MoveRightMapping);

            // now setup the extra mecha godzilla mapping that we'll enable when the time is right.
            InputContext MechaCtx = new InputContext() { Priority = 1 }; // more important than walk context
            InputMapping StrafeLeftMapping = new InputMapping() { Action = StrafeLeft };
            StrafeLeftMapping.KeysToBind.Add(AivKeyCode.A);
            InputMapping StrafeRightMapping = new InputMapping() { Action = StrafeRight };
            StrafeRightMapping.KeysToBind.Add(AivKeyCode.D);
            MechaCtx.Mappings.Add(StrafeLeftMapping);
            MechaCtx.Mappings.Add(StrafeRightMapping);

            // we can add as many extra actions as we want, we are not limited to the movement, for example we can have mecha godzilla shout!

            // by default enable the walk context as it's the default mode of our fake character
            ActionsMappingSystem.AddContext(WalkCtx);

            // at this point we need to define what our actions do.
            // to do so we bind input actions to some behaviour.
            // it doesn't really matter when you bind these, they won't get called if an appropriate context is not active.
            // some cases you might prefer to bind these actions when you activate the context to localize the 
            // functions containing the behaviour to a specific object.
            ActionsMappingSystem.BindInputAction(MoveLeft, OnWalkLeft);
            ActionsMappingSystem.BindInputAction(MoveRight, OnWalkRight);

            ActionsMappingSystem.BindInputAction(StrafeLeft, OnStrafeLeft);
            ActionsMappingSystem.BindInputAction(StrafeRight, OnStrafeRight);

            bool IAmOnMechaGodzilla = false;
            Console.WriteLine("Press space to toggle a mecha godzilla mode!");
            while (window.opened)
            {
                Input.Update(window);

                if(Input.WasPressed(AivKeyCode.Space))
                {
                    // THIS SIMULATES US GOING INTO THE MECHA GODZILLA.
                    IAmOnMechaGodzilla = !IAmOnMechaGodzilla;
                    if(IAmOnMechaGodzilla)
                    {
                        // by simply adding this context the system will automatically filters out the lower priority actions
                        // for the simple walk left and right and will use the mecha godzilla calls.
                        ActionsMappingSystem.AddContext(MechaCtx);
                        Console.WriteLine("now you are on a mecha godzilla!");
                    }
                    else
                    {
                        // removing this context will allow the system to go back to the walk state.
                        ActionsMappingSystem.RemoveContext(MechaCtx);
                        Console.WriteLine("mecha godzilla has ran away, you are on foot now!");
                    }
                }

                window.Update();
            }
        }

        // input action value contains every possible value (either for axis or for button), is up to you to read the correct one.
        static void OnWalkLeft(InputActionValue value)
        {
            if(value.BoolValue)
            {
                Console.WriteLine("START WALK LEFT");
            }
            else
            {
                Console.WriteLine("STOP WALK LEFT");
            }
        }
        static void OnWalkRight(InputActionValue value)
        {
            if (value.BoolValue)
            {
                Console.WriteLine("START WALK RIGHT");
            }
            else
            {
                Console.WriteLine("STOP WALK RIGHT");
            }
        }
        static void OnStrafeLeft(InputActionValue value)
        {
            if (value.BoolValue)
            {
                Console.WriteLine("MECHA GODZILLA START STRAFE LEFT");
            }
            else
            {
                Console.WriteLine("MECHA GODZILLA STOP STRAFE LEFT");
            }
        }
        static void OnStrafeRight(InputActionValue value)
        {
            if (value.BoolValue)
            {
                Console.WriteLine("MECHA GODZILLA START STRAFE RIGHT");
            }
            else
            {
                Console.WriteLine("MECHA GODZILLA STOP STRAFE RIGHT");
            }
        }
    }
}