using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton plr;
    public void Awake()
    {
        if (!plr)
        {
            DontDestroyOnLoad(gameObject);
            plr = this;
        }
        else
        {
            if (plr != this)
            {
                plr.transform.position = transform.position;
                Destroy(gameObject);
            }
        }
    }
}
