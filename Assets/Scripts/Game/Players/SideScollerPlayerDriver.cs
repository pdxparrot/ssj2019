﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players
{
    public abstract class SideScollerPlayerDriver<T> : PlayerDriver where T: class, IInputActionCollection, new()
    {
        protected T Actions { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Actions = new T();
        }
        protected override void OnDestroy()
        {
            Actions = null;

            base.OnDestroy();
        }
#endregion

        protected override void EnableControls(bool enable)
        {
            if(enable) {
                Actions.Enable();
            } else {
                Actions.Disable();
            }
        }

        public void OnMove(Vector2 axes)
        {
            // translate movement from x / y to x / z
            LastControllerMove = new Vector3(axes.x, 0.0f, axes.y);
        }
    }
}
