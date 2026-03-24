namespace Pado.Framework.Core.Events
{
    public enum MEventType
    {
        None,

        AppBootStarted,
        EventSystemReady,
        SaveSystemReady,
        AppBootCompleted,
        AppBootFailed,

        SaveCompleted,
        SaveFailed,
        SaveDeleted,
        SaveDeletedAll,
    }
}
