namespace Pado.Framework.Core.Singleton
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : UnityEngine.MonoBehaviour
    {
        protected override bool ShouldDontDestroyOnLoad => true;
    }
}
