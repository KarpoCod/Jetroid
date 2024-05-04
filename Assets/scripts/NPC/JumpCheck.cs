using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    public bool posible = true;

    private void OnTriggerEnter2D()
    {
        posible = false;
    }

    private void OnTriggerStay2D()
    {
        posible = false;
    }

    private void OnTriggerExit2D()
    {
        posible = true;
    }
}
