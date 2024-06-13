using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float scale = 4f;

    private Transform t;

    private void Awake()
    {
        var cam = GetComponent<Camera>();
        cam.orthographicSize = scale;
    }

    void Start()
    {
        t = target.transform;
    }

    void Update()
    {
        t = target.transform;
        if (target != null)
        {
            transform.position = new Vector3(t.position.x, t.position.y, transform.position.z);
        }
    }
}
