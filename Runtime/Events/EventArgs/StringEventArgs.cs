using System;

namespace Pado.Framework.Core.Events.EventArgs
{
    [Serializable]
    public class StringEventArgs : EventArgs
    {
        public string Value;

        public StringEventArgs(string value)
        {
            Value = value;
        }
    }
}
