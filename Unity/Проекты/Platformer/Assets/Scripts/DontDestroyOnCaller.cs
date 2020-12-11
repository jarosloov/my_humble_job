using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnCaller : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
