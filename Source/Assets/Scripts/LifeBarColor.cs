using UnityEngine;
using UnityEngine.UI;

public class LifeBarColor : MonoBehaviour
{
    private Image fillImage;
    private Slider healthBar;

    private void Start()
    {
        fillImage = this.transform.Find("Fill Area").Find("Fill").gameObject.GetComponent<Image>();
        healthBar = this.GetComponent<Slider>();
    }

    private void Update()
    {
        float healthInPercent = healthBar.value / healthBar.maxValue;
        if (healthInPercent > 0.8)
        {
            fillImage.color = Color.green;
        }
        else if (healthInPercent > 0.3)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}
