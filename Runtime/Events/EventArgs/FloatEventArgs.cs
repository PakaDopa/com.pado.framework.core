using System;

namespace Pado.Framework.Core.Events.EventArgs
{
    [Serializable]
    public class FloatEventArgs : EventArgs
    {
        public float Value;

        public FloatEventArgs(float value)
        {
            Value = value;
        }
    }
}
