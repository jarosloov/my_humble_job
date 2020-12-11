using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundInstanceControllerComponent1 : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string createdTag;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(this.createdTag);
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Main Menu")
            Destroy(this.gameObject);
        if (obj != null)
            Destroy(this.gameObject);
        //if (SceneManager.GetActiveScene().name == "Street")
        //Destroy(this.gameObject)
        else
        {
            this.gameObject.tag = this.createdTag;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
