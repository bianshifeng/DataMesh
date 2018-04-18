using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    public class Singleton<T>: MonoBehaviour where T: Singleton<T>
    {
        private static T instance;

        private static bool searchForInstance = true;

        public static bool isInitialized
        {
            get
            {
                return instance != null;
            }
        }

        public static T Instance
        {
            get
            {
                if(!isInitialized && searchForInstance)
                {
                    searchForInstance = false;
                    T[] objects = FindObjectsOfType<T>();   //找到全局所有的这个类型的实例
                    if(objects.Length == 1)
                    {
                        instance = objects[0];
                        instance.gameObject.GetParentRoot().DontDestroyOnLoad();
                    }
                    else if ( objects.Length > 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}", typeof(T).Name, objects.Length);
                    }
                }

                return instance;
            }
        }

        public static void AssertIsInitialized()
        {
            Debug.Assert(isInitialized, string.Format("The {0} singleton has not been initialized.", typeof(T).Name));
        }

        protected virtual void Awake()
        {
            if (isInitialized && instance != this)
            {
                if(Application.isEditor)
                {
                    DestroyImmediate(this);
                }
                else
                {
                    Destroy(this);
                }

                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}. Additional Instance was destroyed", GetType().Name);

            }
            else if (!isInitialized)
            {
                instance = (T)this;
                searchForInstance = false;
                gameObject.GetParentRoot().DontDestroyOnLoad();
            }
        }


        protected virtual void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }
    }
}
