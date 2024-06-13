using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections.Specialized;
//using static System.Net.Mime.MediaTypeNames;
using System.IO;
//using System.Diagnostics;

public class MenuManager : MonoBehaviour
{
    private bool bind;
    private bool isFullScreen;
    [SerializeField] private GameObject setCanv;
    [SerializeField] private GameObject mainCanv;
    [SerializeField] private WorldGen world;
    private Camera cam;
    public int speed;

    private Resolution[] rsl;
    private List<string> resolutions;
    [SerializeField] private Toggle FullScreenTog;
    [SerializeField] private Dropdown dropdown;

    public GameObject MainLayer;
    public GameObject ChooseWorld;
    public GameObject CreateWorld;

    private string field_world_name;

    public void Start()
    {
        cam = Camera.main;
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
        ChooseWorld.SetActive(false);
        CreateWorld.SetActive(false);
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

    public List<string> ReadSaveDirectory()
    {
        string savesPath = Application.persistentDataPath + "/saves";
        List<string> worldNames = new List<string>();
        if (Directory.Exists(savesPath))
        {
            DirectoryInfo dir = new DirectoryInfo(savesPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                worldNames.Add(file.Name.Substring(0, file.Name.IndexOf('.')));
            }
        }
        /*foreach (string s in  worldNames)
        {
            Debug.Log(s);
        }*/
        return worldNames;
    }

    public void Load()
    {
        DataHold.WorldOperation = 1;
        DataHold.SaveName = field_world_name;
        SceneManager.LoadScene("Main World");
    }

    public void ReadInputField(string s)
    {
        field_world_name = s;
    }

    public void NewGame()
    {
        DataHold.WorldOperation = 2;
        DataHold.SaveName = field_world_name;
        Debug.Log(field_world_name);
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

    ///Layers

    public void ShowUIMainLayer()
    {
        MainLayer.SetActive(true);
        CreateWorld.SetActive(false);
        ChooseWorld.SetActive(false);
    }

    public void ShowUIChooseWorld()
    {
        ChooseWorld.SetActive(true);
        CreateWorld.SetActive(false);
        MainLayer.SetActive(false);
    }

    public void ShowUICreateWorld()
    {
        ChooseWorld.SetActive(false);
        MainLayer.SetActive(false);
        CreateWorld.SetActive(true);
    }
}
