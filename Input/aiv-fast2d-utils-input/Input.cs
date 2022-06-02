using System;
using System.Collections.Generic;
using OpenTK;

namespace Aiv.Fast2D.Utils.Input
{
    public static class Input
    {
        private static readonly Dictionary<AivKeyCode, ButtonState> s_keys = new Dictionary<AivKeyCode, ButtonState>();
        private static readonly Dictionary<AivAxisCode, AxisState> s_axes = new Dictionary<AivAxisCode, AxisState>();

        // this is a list of callbacks listening for key state changes.
        private static readonly Dictionary<AivKeyCode, List<Action<bool>>> s_keysListeners = new Dictionary<AivKeyCode, List<Action<bool>>>();
        private static readonly Dictionary<AivAxisCode, List<Action<float>>> s_axesListeners = new Dictionary<AivAxisCode, List<Action<float>>>();

        internal const int KEYBOARD_OFFSET = 0;
        internal const int MOUSE_OFFSET = 200;
        internal const int JOYPAD_OFFSET = 300;

        // quick utils to not get confused
        internal static bool IsKeyboard(AivKeyCode key) => (int)key < MOUSE_OFFSET;
        internal static bool IsMouse(AivKeyCode key) => (int)key > MOUSE_OFFSET && (int)key < JOYPAD_OFFSET;
        internal static bool IsJoypad(AivKeyCode key) => (int)key > JOYPAD_OFFSET;

        static Input()
        {
            // fill dict with Key States
            s_keys.Clear();
            foreach (var item in Enum.GetValues(typeof(AivKeyCode)))
            {
                AivKeyCode key = (AivKeyCode)item;
                s_keys.Add(key, new ButtonState());
            }

            // fill dictionary with axes states
            s_axes.Clear();
            foreach (var item in Enum.GetValues(typeof(AivAxisCode)))
            {
                AivAxisCode key = (AivAxisCode)item;
                s_axes.Add(key, new AxisState());
            }
        }

        private static void RaiseStateChangedEventIfNeeded(AivKeyCode key)
        {
            var keyState = s_keys[key];
            if ((keyState.Down || keyState.Up) &&
                    s_keysListeners.TryGetValue(key, out var listeners))
            {
                // value has changed and we need to raise events
                foreach (var action in listeners)
                {
                    action?.Invoke(keyState.Pressed); // pass in the current state of the key
                }
            }
        }

        private static bool AlmostEqual(float a, float b, float tolerance = 0.0001f)
        {
            return Math.Abs(a - b) <= tolerance;
        }

        public static void Update(Window window)
        {
            // keys update
            foreach (var keyState in s_keys)
            {
                var key = keyState.Key;
                if (IsJoypad(key))
                {
                    // this is handled separatedly 
                    continue;
                }

                bool newVal = false;
                if (IsKeyboard(key))
                {
                    newVal = window.GetKey((KeyCode)key); // this maps 1:1 with the window keycode enum
                }
                else if (IsMouse(key))
                {
                    int idx = ((int)key) - MOUSE_OFFSET - 1; // open tk maps the first to 0, we map it to 1 so to compansate we just remove 1;
                    newVal = window.context.Mouse.GetState().IsButtonDown((OpenTK.Input.MouseButton)idx);
                }

                keyState.Value.Update(newVal);
                RaiseStateChangedEventIfNeeded(key);
            }

            void UpdateJoystickButton(AivKeyCode key, bool value)
            {
                var keyState = s_keys[key];
                keyState.Update(value);
                RaiseStateChangedEventIfNeeded(key);
            }

            void UpdateJoystickAxis(AivAxisCode axis, float value)
            {
                var axisState = s_axes[axis];
                float prevVal = axisState.Value;
                axisState.Update(value);
                if (!AlmostEqual(prevVal, value))
                {
                    // raise events
                    if (s_axesListeners.TryGetValue(axis, out var listeners))
                    {
                        // value has changed and we need to raise events
                        foreach (var action in listeners)
                        {
                            action?.Invoke(value);
                        }
                    }
                }
            }

            // we can't use this technique with the joypads so we update them separatedly
            const int joystickSupported = 2;
            for (int j = 0; j < joystickSupported; j++)
            {
                var state = OpenTK.Input.GamePad.GetState(j);
                UpdateJoystickButton(JoystickButtons.A(j), state.Buttons.A == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.B(j), state.Buttons.B == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.X(j), state.Buttons.X == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.Y(j), state.Buttons.Y == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.Up(j), state.DPad.IsUp);
                UpdateJoystickButton(JoystickButtons.Down(j), state.DPad.IsDown);
                UpdateJoystickButton(JoystickButtons.Left(j), state.DPad.IsLeft);
                UpdateJoystickButton(JoystickButtons.Right(j), state.DPad.IsRight);
                UpdateJoystickButton(JoystickButtons.Start(j), state.Buttons.Start == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.Back(j), state.Buttons.Back == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.BigButton(j), state.Buttons.BigButton == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.BumperLeft(j), state.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.BumperRight(j), state.Buttons.RightShoulder == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.TriggerLeft(j), state.Triggers.Left > 0.7f); // todo should be going into a config file
                UpdateJoystickButton(JoystickButtons.TriggerRight(j), state.Triggers.Right > 0.7f); // todo should be going into a config file
                UpdateJoystickButton(JoystickButtons.StickLeft(j), state.Buttons.LeftStick == OpenTK.Input.ButtonState.Pressed);
                UpdateJoystickButton(JoystickButtons.StickRight(j), state.Buttons.RightStick == OpenTK.Input.ButtonState.Pressed);

                // in the same for loop we can update joystick axes too
                UpdateJoystickAxis(JoystickAxes.LeftHorizontal(j), state.ThumbSticks.Left.X);
                UpdateJoystickAxis(JoystickAxes.LeftVertical(j), state.ThumbSticks.Left.Y);
                UpdateJoystickAxis(JoystickAxes.RightHorizontal(j), state.ThumbSticks.Right.X);
                UpdateJoystickAxis(JoystickAxes.RightVertical(j), state.ThumbSticks.Right.Y);
                UpdateJoystickAxis(JoystickAxes.LeftTrigger(j), state.Triggers.Left);
                UpdateJoystickAxis(JoystickAxes.RightTrigger(j), state.Triggers.Right);
            }

            MouseX = window.mouseX;
            MouseY = window.mouseY;
            MousePosition = window.mousePosition;
        }

        // enhanced input events
        public static bool IsPressed(AivKeyCode key)
        {
            if (s_keys.TryGetValue(key, out var state))
            {
                return state.Pressed;
            }
            return false;
        }
        public static bool WasPressed(AivKeyCode key)
        {
            if (s_keys.TryGetValue(key, out var state))
            {
                return state.Down;
            }
            return false;
        }
        public static bool WasReleased(AivKeyCode key)
        {
            if (s_keys.TryGetValue(key, out var state))
            {
                return state.Up;
            }
            return false;
        }
        public static float GetAxis(AivAxisCode axis)
        {
            if (s_axes.TryGetValue(axis, out var state))
            {
                return state.Value;
            }
            return 0f;
        }
        // end enhanced input events

        //Mouse Events
        public static float MouseX { get; private set; }
        public static float MouseY { get; private set; }
        public static Vector2 MousePosition { get; private set; }


        // todo add events logic
        public static void RegisterListener(AivKeyCode key, Action<bool> callback)
        {
            if (!s_keysListeners.ContainsKey(key))
            {
                s_keysListeners.Add(key, new List<Action<bool>>());
            }

            s_keysListeners[key].Add(callback);
        }

        public static void UnregisterListener(AivKeyCode key, Action<bool> callbackToRemove)
        {
            if (s_keysListeners.TryGetValue(key, out var listeners))
            {
                listeners.Remove(callbackToRemove);
            }
        }

        public static void RegisterListener(AivAxisCode key, Action<float> callback)
        {
            if (!s_axesListeners.ContainsKey(key))
            {
                s_axesListeners.Add(key, new List<Action<float>>());
            }

            s_axesListeners[key].Add(callback);
        }

        public static void UnregisterListener(AivAxisCode key, Action<float> callbackToRemove)
        {
            if (s_axesListeners.TryGetValue(key, out var listeners))
            {
                listeners.Remove(callbackToRemove);
            }
        }
    }
}