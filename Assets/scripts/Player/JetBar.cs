using UnityEngine;
using UnityEngine.UI;

public class JetBar : MonoBehaviour
{
    public Image jetBar;
    public Image HPBar;

    void Update()
    {
        jetBar.fillAmount = PlayerController.fuel;
        HPBar.fillAmount = Character.Health;
    }
}
