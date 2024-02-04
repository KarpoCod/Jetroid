using UnityEngine;
using System.Threading.Tasks;

public class AutoSave : MonoBehaviour
{

    [SerializeField] private float ReloadTime = 5;
    [SerializeField] private LoadManager LoadManager;

    void Update()
    {
        ReloadTime -= Time.deltaTime;

        if (ReloadTime <= 0)
        {
            Debug.LogWarning("update");
            ReloadTime = 5;
            Task.Factory.StartNew(() =>
            {
                LoadManager.Save_World();
            });
        }
    }
}
