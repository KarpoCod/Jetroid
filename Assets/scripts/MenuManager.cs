using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private bool bind;
    private bool isFullScreen;
    public GameObject setCanv;
    public GameObject mainCanv;

    Resolution[] rsl;
    List<string> resolutions;
    public Toggle FullScreenTog;
    public Dropdown dropdown;
    public Slider brightness;
    public Text fire1;

    public void Awake()
    {
        isFullScreen = Screen.fullScreen;
        resolutions = new List<string>();
        rsl = Screen.resolutions;
        foreach (var i in rsl)
        {
            resolutions.Add(i.width + "x" + i.height);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(resolutions);
        string resol = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();

        dropdown.value = resolutions.IndexOf(resol);
    }

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

    public void Settings()
    {
        mainCanv.SetActive(false);
        setCanv.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        mainCanv.SetActive(true);
        setCanv.SetActive(false);
    }

    ///settings

    /*public string BindButton()
    {
        while (true) { if(Input.anyKeyDown) break; }
        return Input.inputString;
    }*/

    public void Brightness()
    {
        Screen.brightness = brightness.value;
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
        FullScreenTog.isOn = isFullScreen;

    }

    /*public void Fire1()
    {
        fire1.text = "press button to bind";
        string inp = BindButton();
        fire1.text = inp;
        
    }*/

    public void Resolution(int r)
    {
        Screen.SetResolution(rsl[r].width, rsl[r].height, isFullScreen);
    }
}
