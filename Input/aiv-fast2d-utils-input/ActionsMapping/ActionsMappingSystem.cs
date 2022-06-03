using System;
using System.Collections.Generic;

namespace Aiv.Fast2D.Utils.Input.ActionsMapping
{
    public static class ActionsMappingSystem
    {
        private class KeyCallbackWrapper
        {
            public AivKeyCode Key;
            public InputAction InputAction;

            public void OnValueChanged(bool value)
            {
                OnActionKeyValueChanged(Key, InputAction, value);
            }
        }

        // these are ordered by priority
        private static readonly List<InputContext> s_activeContexts = new List<InputContext>();

        private static readonly Dictionary<InputAction, Action<InputActionValue>> s_registeredActions =
            new Dictionary<InputAction, Action<InputActionValue>>();

        // cache for bound actions so we are able to unregister them easily
        private static readonly Dictionary<AivKeyCode, Dictionary<InputAction, Action<bool>>> s_currentlyBoundKeyActions = new Dictionary<AivKeyCode, Dictionary<InputAction, Action<bool>>>();
        private static readonly Dictionary<AivAxisCode, Dictionary<InputAction, Action<float>>> s_currentlyBoundAxisActions = new Dictionary<AivAxisCode, Dictionary<InputAction, Action<float>>>();

        // context APIs
        private static void ValidateContext(InputContext ctx)
        {
            HashSet<InputAction> set = new HashSet<InputAction>();
            foreach (var map in ctx.Mappings)
            {
                if(!set.Add(map.Action))
                {
                    throw new System.Exception("The same actions can only appear one time for a given context. To bind the same action to multiple keys use the lists provided in the class InputMapping.");
                }
            }
        }

        private static void OnActionKeyValueChanged(AivKeyCode key, InputAction action, bool value)
        {
            for (int i = s_activeContexts.Count - 1; i >= 0; i--)
            {
                // from back to front check what highest priority context has this key bound to it
                var ctx = s_activeContexts[i];
                if (ctx.HasKeyBoundToAnyAction(key, out InputAction relatedAction))
                {
                    if(relatedAction != action)
                    {
                        // if the action is different, we have found our key, we don't keep searching lower priorities.
                        // just get out of the loop and ignore this input
                        break;
                    }

                    // we check if the ctx has the correct action bound to the key. if not means this ctx has
                    // the same key bound to a different action and this ctx is higher priority so we ignore this input.
                    // we weill receive another callback with the correct key and input action.
                    if (s_registeredActions.TryGetValue(action, out var userCallback))
                    {
                        // create payload and send back to the user
                        InputActionValue arg = new InputActionValue();
                        arg.BoolValue = value;
                        userCallback?.Invoke(arg);
                    }
                }
            }
        }

        private static void OnActionAxisValueChanged(AivAxisCode axis, InputAction action, float value)
        {
            for (int i = s_activeContexts.Count - 1; i >= 0; i--)
            {
                // from back to front check what highest priority context has this key bound to it
                var ctx = s_activeContexts[i];
                if (ctx.HasAxisBoundToAnyAction(axis, out InputAction relatedAction))
                {
                    if (relatedAction != action)
                    {
                        // if the action is different, we have found our key, we don't keep searching lower priorities.
                        // just get out of the loop and ignore this input
                        break;
                    }

                    // we check if the ctx has the correct action bound to the axis. if not means this ctx has
                    // the same key bound to a different action and this ctx is higher priority so we ignore this input.
                    // we weill receive another callback with the correct axis and input action.
                    if (s_registeredActions.TryGetValue(action, out var userCallback))
                    {
                        // create payload and send back to the user
                        InputActionValue arg = new InputActionValue();
                        arg.FloatValue = value;
                        userCallback?.Invoke(arg);
                    }
                }
            }
        }

        private static void CacheAction<TActionArg, TAivCode>(
            Dictionary<TAivCode, Dictionary<InputAction, Action<TActionArg>>> container,
            Action<TActionArg> action,
            TAivCode key,
            InputAction inputAction)
        {
            if (!container.ContainsKey(key))
            {
                container.Add(key, new Dictionary<InputAction, Action<TActionArg>>());
            }
            container[key].Add(inputAction, action);
        }

        private static Action<TActionArg> GetAndClearAction<TActionArg, TAivCode>(
            Dictionary<TAivCode, Dictionary<InputAction, Action<TActionArg>>> container,
            TAivCode key,
            InputAction inputAction)
        {
            var result = container[key][inputAction];
            container[key].Remove(inputAction);
            return result;
        }

        private static void CacheKeyAction(Action<bool> action, AivKeyCode key, InputAction inputAction)
        {
            CacheAction(s_currentlyBoundKeyActions, action, key, inputAction);
            //if(!s_currentlyBoundKeyActions.ContainsKey(key))
            //{
            //    s_currentlyBoundKeyActions.Add(key, new Dictionary<InputAction, Action<bool>>());
            //}
            //s_currentlyBoundKeyActions[key].Add(inputAction, action);
        }

        private static Action<bool> GetAndClearKeyAction(AivKeyCode key, InputAction inputAction)
        {
            return GetAndClearAction(s_currentlyBoundKeyActions, key, inputAction);
            //var result = s_currentlyBoundKeyActions[key][inputAction];
            //s_currentlyBoundKeyActions[key].Remove(inputAction);
            //return result;
        }

        private static void CacheAxisAction(Action<float> action, AivAxisCode axis, InputAction inputAction)
        {
            CacheAction(s_currentlyBoundAxisActions, action, axis, inputAction);
            //if (!s_currentlyBoundAxisActions.ContainsKey(axis))
            //{
            //    s_currentlyBoundAxisActions.Add(axis, new Dictionary<InputAction, Action<float>>());
            //}
            //s_currentlyBoundAxisActions[axis].Add(inputAction, action);
        }

        private static Action<float> GetAndClearAxisAction(AivAxisCode axis, InputAction inputAction)
        {
            return GetAndClearAction(s_currentlyBoundAxisActions, axis, inputAction);

            //var result = s_currentlyBoundKeyActions[key][inputAction];
            //s_currentlyBoundKeyActions[key].Remove(inputAction);
            //return result;
        }

        public static void AddContext(InputContext ctx)
        {
            ValidateContext(ctx);

            s_activeContexts.Add(ctx);
            s_activeContexts.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            // when a ctx is added, we need to register its actions into the actual input manager. 
            // when we receive notifications from them we can handle them properly and if the priority is correct 
            // we send them back to the user

            foreach (var actionMap in ctx.Mappings)
            {
                foreach (var key in actionMap.KeysToBind)
                {
                    // this is dodgy but we need more info in the callback than just the value. in order to be able to unregister it we need to cache the action behind some keys
                    Action<bool> ac = new Action<bool>((bool value) => OnActionKeyValueChanged(key, actionMap.Action, value));
                    Input.RegisterListener(key, ac);

                    // cache this action in order to unregister it later when needed.
                    CacheKeyAction(ac, key, actionMap.Action);
                }

                foreach (var axis in actionMap.AxesToBind)
                {
                    Action<float> ac = new Action<float>((float value) => OnActionAxisValueChanged(axis, actionMap.Action, value));
                    Input.RegisterListener(axis, ac);
                    CacheAxisAction(ac, axis, actionMap.Action);
                }
            }
        }

        public static void RemoveContext(InputContext ctx)
        {
            s_activeContexts.Remove(ctx);

            foreach (var actionMap in ctx.Mappings)
            {
                foreach (var key in actionMap.KeysToBind)
                {
                    Input.UnregisterListener(key, GetAndClearKeyAction(key, actionMap.Action));
                }

                foreach (var axis in actionMap.AxesToBind)
                {
                    Input.UnregisterListener(axis, GetAndClearAxisAction(axis, actionMap.Action));
                }
            }
        }
        // end context APIs

        // input actions APIs
        public static void BindInputAction(InputAction action, Action<InputActionValue> callback)
        {
            // we are an interface between actual input and the user. this will bind to the actual input system and
            // will go back to the user only if all the requirements are matched
            s_registeredActions.Add(action, callback);
        }

        public static void UnbindInputAction(InputAction action)
        {
            s_registeredActions.Remove(action);
        }

        // end input actions APIs
    }
}
