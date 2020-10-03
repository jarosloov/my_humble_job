using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SceneManager. LoadScene("имя сцены")

public class Touching_the_ground : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.CompareTag("ground"))
        {
            SceneManager.LoadScene(0);
        }
    }



}
