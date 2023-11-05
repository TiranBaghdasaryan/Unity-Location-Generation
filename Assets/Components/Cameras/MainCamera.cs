
using UnityEngine;

namespace Components.Cameras
{
    public class MainCamera : Movable3DCamera
    {
        public static MainCamera Current { get; private set; }

        private void OnEnable()
        {
            Current = this;
        }

        private void OnDisable()
        {
            Current = null;
        }
    }
}