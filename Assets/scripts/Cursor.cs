using UnityEngine;

public class Cursor : MonoBehaviour
{
    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouse2 = new Vector2(mouse.x, mouse.y);
        transform.position = mouse2;
    }
}
