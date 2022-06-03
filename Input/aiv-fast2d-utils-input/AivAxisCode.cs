using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Fast2D.Utils.Input
{
    public enum AivAxisCode
    {
        Joystick1LeftHorizontal = Input.JOYPAD_OFFSET + 1,
        Joystick1LeftVertical,
        Joystick1RightHorizontal,
        Joystick1RightVertical,
        Joystick1LeftTrigger,
        Joystick1RightTrigger,

        Joystick2LeftHorizontal = Input.JOYPAD_OFFSET + 101,
        Joystick2LeftVertical,
        Joystick2RightHorizontal,
        Joystick2RightVertical,
        Joystick2LeftTrigger,
        Joystick2RightTrigger,
    }
}
