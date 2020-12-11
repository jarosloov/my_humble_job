using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundInstanceControllerComponent : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string createdTag;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(this.createdTag);
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
