using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    private static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public static Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }
}
