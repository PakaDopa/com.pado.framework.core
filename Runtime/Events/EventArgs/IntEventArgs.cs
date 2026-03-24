using System;

namespace Pado.Framework.Core.Events.Args
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
