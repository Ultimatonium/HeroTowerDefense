using UnityEngine;
using UnityEngine.Networking;

public class GladeOfTheRebirth : NetworkBehaviour
{
    [SerializeField]
    private float GladeRadius;

    private void Start()
    {
        if (isServer)
        {
            gameObject.tag = "Alpha";
        }
        else
        {
            gameObject.tag = "Omega";
        }
    }

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(this.transform.position, GladeRadius);
        foreach (Collider hit in hits)
        {
            if (hit.isTrigger) continue;
            if (hit.tag != this.tag) continue;
            if (hit.gameObject.name != "Hero") continue;
            Hero hero = hit.gameObject.GetComponent<Hero>();
            if (hero != null)
            {
                hero.ApplyHeal(10 * Time.deltaTime);
            }
            else
            {
                Debug.LogWarning("Unauthorized object in Glade");
            }
        }
    }
}
