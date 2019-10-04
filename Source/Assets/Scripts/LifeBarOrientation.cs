using UnityEngine;
using UnityEngine.UI;

public class LifeBarOrientation : MonoBehaviour
{
    private void Update()
    {
        if (GetComponent<Slider>() == null) { return; }
        this.transform.rotation = Quaternion.Euler(0, -Camera.main.transform.rotation.y, 0);
    }
}
