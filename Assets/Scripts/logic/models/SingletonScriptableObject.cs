using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                T[] objects = Resources.FindObjectsOfTypeAll<T>();
                if (objects.Length == 0)
                {
                    Debug.LogError("SingletonScriptableObject -> instance -> objects.Length == 0");
                    return null;
                }

                /*if (objects.Length > 1)
                {
                    Debug.LogError("SingletonScriptableObject -> instance -> objects.Length > 1");
                    Debug.LogError("objects.Length: " + objects.Length);
                    return null;
                }*/
                
                _instance = objects[0];
            }

            return _instance;
        }
    }
        
}