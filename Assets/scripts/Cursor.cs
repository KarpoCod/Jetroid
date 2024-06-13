using UnityEngine;

public class Cursor : MonoBehaviour
{
    void Update()
    {
        Vector3 cam = Camera.main.transform.position;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(cam, mouse-cam, 10, LayerMask.GetMask("FG"));
        if (hitInfo)
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = Vector2.MoveTowards(cam, mouse, 10);
        }
    }
}
