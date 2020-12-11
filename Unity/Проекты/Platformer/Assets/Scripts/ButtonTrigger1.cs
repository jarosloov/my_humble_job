using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTrigger1 : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    [SerializeField]
    int LvlToChanger;
    public bool isReady = false;
    public GameObject Player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator1.SetTrigger("isTriggered");
            animator2.SetTrigger("isTriggered");
            isReady = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator1.SetTrigger("isTriggered");
            animator2.SetTrigger("isTriggered");
            isReady = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isReady)
        {
            SceneManager.LoadScene(LvlToChanger);
        }
    }
}
