using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Load()
    {
        DataHold.WorldOperation = 1;
        SceneManager.LoadScene("Main World");
    }

    public void NewGame()
    {
        DataHold.WorldOperation = 2;
        SceneManager.LoadScene("Main World");
    }
}
