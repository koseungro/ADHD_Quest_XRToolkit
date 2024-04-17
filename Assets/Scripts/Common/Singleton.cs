using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;

    protected static bool dontDestroy = false;

    public static T Inst
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    T[] objects = FindObjectsOfType(typeof(T)) as T[];
                    if (objects.Length > 1)
                    {
                        Debug.Log("Find : " + _instance.name);
                        Debug.LogError("Singleton: " + objects.Length.ToString() + "가 존재함. " + _instance.name);

                        return _instance;
                    }

                    //if (_instance == null)
                    //{
                    //    GameObject singleton = new GameObject();
                    //    _instance = singleton.AddComponent<T>();
                    //    singleton.name = "_" + typeof(T).ToString() + "_";
                    //    //DontDestroyOnLoad(singleton);
                    //}
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {                
                _instance = this as T;

                if(dontDestroy)
                    DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this.GetComponent<T>())
            {
                Destroy(this.gameObject);
            }
        }
    }

    public virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;

        if (dontDestroy)
            _instance = null;
    }
}
