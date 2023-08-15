using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolow : MonoBehaviour
{
    public GameObject target;
    public float scale = 4f;

    public const int offsetX = 0;
    public const int offsetY = 0;

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
        if (target != null)
        {
            transform.position = new Vector3(t.position.x + offsetX, t.position.y + offsetY, transform.position.z);
        }
        t = target.transform;
    }
}
