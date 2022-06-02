using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Fast2D.Utils.Input
{
    internal static class JoystickButtons
    {
        internal static AivKeyCode A(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 1);
        internal static AivKeyCode B(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 2);
        internal static AivKeyCode X(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 3);
        internal static AivKeyCode Y(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 4);
        internal static AivKeyCode Up(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 5);
        internal static AivKeyCode Down(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 6);
        internal static AivKeyCode Left(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 7);
        internal static AivKeyCode Right(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 8);
        internal static AivKeyCode Start(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 9);
        internal static AivKeyCode Back(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 10);
        internal static AivKeyCode BigButton(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 11);
        internal static AivKeyCode BumperLeft(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 12);
        internal static AivKeyCode BumperRight(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 13);
        internal static AivKeyCode TriggerLeft(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 14);
        internal static AivKeyCode TriggerRight(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 15);
        internal static AivKeyCode StickLeft(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 16);
        internal static AivKeyCode StickRight(int idx) => (AivKeyCode)(Input.JOYPAD_OFFSET + (idx * 100) + 17);
    }

    internal static class JoystickAxes
    {
        internal static AivAxisCode LeftHorizontal(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 1);
        internal static AivAxisCode LeftVertical(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 2);
        internal static AivAxisCode RightHorizontal(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 3);
        internal static AivAxisCode RightVertical(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 4);
        internal static AivAxisCode LeftTrigger(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 5);
        internal static AivAxisCode RightTrigger(int idx) => (AivAxisCode)(Input.JOYPAD_OFFSET + (idx * 100) + 6);
    }
}
