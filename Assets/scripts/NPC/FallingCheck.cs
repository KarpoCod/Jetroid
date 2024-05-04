using UnityEngine;

public class FallingCheck : MonoBehaviour
{
    [SerializeField]private NPCMove NPC;
    private bool col;
    private void OnTriggerExit2D()
    {
        if (!col) NPC.Rotate();
    }

    private void OnTriggerStay2D()
    {
        col = true;
    }

    void Update()
    {
        if (col == true) { col = false;}
    }
}
