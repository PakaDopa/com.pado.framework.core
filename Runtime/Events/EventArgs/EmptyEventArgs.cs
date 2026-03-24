using System;

namespace Pado.Framework.Core.Events.EventArgs
{
    [Serializable]
    public sealed class EmptyEventArgs : EventArgs
    {
        public static readonly EmptyEventArgs Instance = new EmptyEventArgs();

        private EmptyEventArgs()
        {
        }
    }
}
