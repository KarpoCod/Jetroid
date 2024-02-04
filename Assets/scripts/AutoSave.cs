using UnityEngine;
using System.Threading.Tasks;

public class AutoSave : MonoBehaviour
{ 
    [SerializeField] private float ReloadTime = 5;
    private LoadManager LoadManager;

    private void Start()
    {
        LoadManager = this.GetComponent<LoadManager>();
    }

    private void Update()
    {
        ReloadTime -= Time.deltaTime;

        if (ReloadTime <= 0)
        {
            ReloadTime = 5;
            Task.Factory.StartNew(() =>
            {
                LoadManager.Save_World();
            });
        }
    }
}
