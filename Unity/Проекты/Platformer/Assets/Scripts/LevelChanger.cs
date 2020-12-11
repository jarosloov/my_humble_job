using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    public int levelToload;
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public VectorValue playerStorage;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeToLevel()
    {
        animator.SetTrigger("fade");
    }

    public void OnFadeComplete()
    {
        playerStorage.initalValue = position;
        SceneManager.LoadScene(levelToload);
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
