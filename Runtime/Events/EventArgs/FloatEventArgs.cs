using System;

namespace Pado.Framework.Core.Events.Args
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
