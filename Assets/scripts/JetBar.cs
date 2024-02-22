using UnityEngine;
using UnityEngine.UI;

public class JetBar : MonoBehaviour
{
    public Image jetBar;

    void Update()
    {
        jetBar.fillAmount = PlayerController.fuel;
    }
}
