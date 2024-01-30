using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    private bool bind;
    private bool isFullScreen;
    public GameObject setCanv;
    public GameObject mainCanv;
    public WorldGen world;
    public Camera cam;
    public int speed;

    Resolution[] rsl;
    List<string> resolutions;
    public Toggle FullScreenTog;
    public Dropdown dropdown;


    public void Start()
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
        string resol = Screen.width.ToString() + "x" + Screen.height.ToString();

        dropdown.value = resolutions.IndexOf(resol);
        world.create_world();
        world.ready = true;
    }

    public void Update()
    {
        try
        {
            float s = Time.deltaTime * speed;
            int ty = world.Teraingen.GetFirstAir((int)Math.Floor(cam.transform.position.x), world.seed);
            float cy = cam.transform.position.y;
            float ys = (MathF.Abs(cy - ty) < 0.01) ? ty : cy + (ty - cy) * 0.05f;
            cam.transform.position = new Vector3(cam.transform.position.x + s, ys, -5);
        }
        catch{ }
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

   

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
        FullScreenTog.isOn = isFullScreen;

    }

    public void Resolution(int r)
    {
        Screen.SetResolution(rsl[r].width, rsl[r].height, isFullScreen);
    }
}
