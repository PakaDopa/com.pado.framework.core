using System;

namespace Pado.Framework.Core.Events.Args
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
