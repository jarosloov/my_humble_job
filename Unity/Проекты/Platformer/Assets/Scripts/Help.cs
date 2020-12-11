using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Help : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    public bool isReady = false;

    [SerializeField]
    int LvlToChanger;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator2.SetTrigger("isTriggered");
            isReady = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator2.SetTrigger("isTriggered");
            isReady = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator1.Play("Player_Idle");
        if (Input.GetKeyDown(KeyCode.E) && isReady)
            SceneManager.LoadScene(LvlToChanger);
    }
}
