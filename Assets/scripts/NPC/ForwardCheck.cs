using UnityEngine;

public class ForwardCheck : MonoBehaviour
{
    public bool posible = true;

    private void OnTriggerEnter2D()
    {
        posible = false;
    }

    private void OnTriggerExit2D()
    {
        posible = true;
    }
}
