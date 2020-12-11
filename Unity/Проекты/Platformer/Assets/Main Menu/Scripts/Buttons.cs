using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void PlayGame()        // выход в игру
    {
        SceneManager.LoadScene("Street");
    }
    
    public void Authors()        // выход в авторы
    {
        SceneManager.LoadScene("Authors");
    }

    public void Back()        //выход в меню
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()        //Выход из игры
    {
        Debug.Log("Я вышла закрой рот");
        Application.Quit();
    }

    public void Storyline()    // сюжет Матвея
    {
        SceneManager.LoadScene("storyline");
    }

    public void Customization()
    {
        SceneManager.LoadScene("Customization");
    }
}
