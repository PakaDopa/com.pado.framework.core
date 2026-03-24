using System;

namespace Pado.Framework.Core.Events.Args
{
    [Serializable]
    public class BoolEventArgs : EventArgs
    {
        public bool Value;

        public BoolEventArgs(bool value)
        {
            Value = value;
        }
    }
}
