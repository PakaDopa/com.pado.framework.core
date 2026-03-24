namespace Pado.Framework.Core.Bootstrap
{
    public enum BootState
    {
        None,
        CreatingCoreServices,
        InitializingEventManager,
        InitializingSaveManager,
        Completed,
        Failed,
    }
}
