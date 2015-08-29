using UnityEngine;
using System;

public class SingletonGameObject<T> : MonoBehaviour where T : MonoBehaviour {
    private static WeakReference<T> _instance;
    private static bool quit = false;

    void OnApplicationQuit() {
        quit = true;
    }

    public static bool HasInstance() {
        if (_instance == null || _instance.Target == null) {
            return false;
        }

        return true;
    }

    public static T instance {
        get {
            if (quit) {
                return null;
            }

            if (_instance != null && _instance.Target != null) {
                return _instance.Target;
            }

            GameObject container = new GameObject();
            container.name = "_" + typeof(T).Name;
            T instance = container.AddComponent(typeof(T)) as T;
            _instance = new WeakReference<T>(instance);
            return instance;
        }
    }
}


public class Singleton<T> where T : new() {
    private static T _instance;

    public static T instance {
        get {
            if( _instance == null ) {
                _instance = new T();
            }
            
            return _instance;
        }
    }
}
