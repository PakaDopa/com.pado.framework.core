using System;

namespace Pado.Framework.Core.Events.EventArgs
{
    [Serializable]
    public class IntEventArgs : EventArgs
    {
        public int Value;

        public IntEventArgs(int value)
        {
            Value = value;
        }
    }
}
