using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Fast2D.Utils.Input.ActionsMapping
{
    /// <summary>
    /// Wrapper for an input value
    /// </summary>
    public struct InputActionValue
    {
        public bool BoolValue;
        public float FloatValue;
    }

    /// <summary>
    /// Maps an action to an actual input
    /// </summary>
    public class InputMapping
    {
        public InputAction Action;
        public readonly List<AivKeyCode> KeysToBind = new List<AivKeyCode>();
        public readonly List<AivAxisCode> AxesToBind = new List<AivAxisCode>();
    }

    public class InputContext
    {
        public string Name;

        /// <summary>
        /// Used to determine if this context takes priority over others. Higher numbers mean higher priority
        /// </summary>
        public int Priority;

        public readonly List<InputMapping> Mappings = new List<InputMapping>();

        public bool HasKeyBoundToAnyAction(AivKeyCode key, out InputAction outRelatedAction)
        {
            foreach (var map in Mappings)
            {
                if(map.KeysToBind.Contains(key))
                {
                    outRelatedAction = map.Action;
                    return true;
                }
            }

            outRelatedAction = null;
            return false;
        }

        public bool HasAxisBoundToAnyAction(AivAxisCode axis, out InputAction outRelatedAction)
        {
            foreach (var map in Mappings)
            {
                if (map.AxesToBind.Contains(axis))
                {
                    outRelatedAction = map.Action;
                    return true;
                }
            }

            outRelatedAction = null;
            return false;
        }

    }
}
