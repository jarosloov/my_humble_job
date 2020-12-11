using UnityEngine;
using System.Collections;

public class dontdestroy : MonoBehaviour
{
    public static Transform playerTransform;
    void Awake()
    {
        if (playerTransform = null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(transform.gameObject);
        playerTransform = transform;
    }
}