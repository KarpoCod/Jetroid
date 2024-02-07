using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FuelOutput : MonoBehaviour
{
    public Text FuelText;

    void Update()
    {
        FuelText.text = "Your Fuel is " + (PlayerController.fuel * 100).ToString("0") + " / 100";
    }
}

