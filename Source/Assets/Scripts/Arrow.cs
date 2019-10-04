using UnityEngine;
using UnityEngine.Networking;

public class Arrow : NetworkBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip arrowShotClip;
    [SerializeField]
    private AudioClip arrowHitClip;

    private TeamTag team;
    private GameObject source;
    private float dmg;

    [ServerCallback]
    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    [ServerCallback]
    private void Update()
    {
        this.transform.Translate((Vector3.forward + new Vector3(0, -0.05f)) * Time.deltaTime * 20);
    }

    [ServerCallback]
    public void FireArrow(GameObject target, GameObject source, float dmg, TeamTag team)
    {
        InitValues.PlayLowPrioAudioSource(gameObject, arrowShotClip, 0, 0);
        this.source = source;
        this.dmg = dmg;
        this.team = team;
        this.transform.LookAt(target.transform.position + new Vector3(0, 1, 0));
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.gameObject.GetComponent<Arrow>() == this) return;

        PhysicalObject physicalObject = other.gameObject.GetComponent<PhysicalObject>();
        if (physicalObject != null)
        {
            if (physicalObject.Team == team) return;
            if (physicalObject.IsDead()) return;
            physicalObject.ApplyDmg(dmg, source);
        }

        transform.Translate(Vector3.forward * 0.75f);
        transform.parent = other.gameObject.transform;

        InitValues.PlayLowPrioAudioSource(gameObject, arrowHitClip, 0, 0.4f);

        enabled = false;
    }

}
