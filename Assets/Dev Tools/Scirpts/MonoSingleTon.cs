using UnityEngine;

namespace FSF.Collection{
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
                DestroyImmediate(this.gameObject);
                return;
            }
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
