using System.Diagnostics;
using UnityEngine;

public class FallingCheck : MonoBehaviour
{
    [SerializeField]private NPCMove NPC;
    private int counter;
    private void OnTriggerExit2D()
    {
        counter--;
        if (counter == 0) NPC.Rotate();
    }

    private void OnTriggerEnter2D()
    {
        counter ++;
    }
}
