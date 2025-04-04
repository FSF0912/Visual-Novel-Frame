using UnityEngine;

namespace FSF.Collection
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake() 
        {
            if(instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            OnAwake();
        }

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }

        protected virtual void OnAwake() { }
    }
}
