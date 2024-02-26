using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetController : MonoBehaviour
{
    private bool running = false;
   [SerializeField] private GameObject effects;

    public void Fly(bool fire)
    {
        if (running != fire)
        {
            running = fire;
            effects.SetActive(running);
        }
    }
}
