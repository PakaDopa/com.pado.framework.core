using System;

namespace Pado.Framework.Core.Events.EventArgs
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
