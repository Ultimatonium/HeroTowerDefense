using UnityEngine;
using UnityEngine.Networking;

public class MeleeMinion : Minion
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip hitClip;

    private float timeSinceHit;

    protected override void Start()
    {
        base.Start();
        transform.Find("Type").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        HudDIncrementPhysicalObjectAmountByOne(PhysicalObjectType.MeleeMinion);
        if (isServer) InvokeRepeating("HitAllEnemies", 0, TimeTillTick());
    }

    protected override void Update()
    {
        base.Update();

        if (IsDead()) return;

        timeSinceHit += Time.deltaTime;
        if (timeSinceHit >= TimeTillTick())
        {
            HitAllEnemies();
            timeSinceHit = 0;
        }
    }

    [ServerCallback]
    private void HitAllEnemies()
    {
        Enemies.RemoveAll(PhysicalObjects => PhysicalObjects == null);
        foreach (PhysicalObject enemy in Enemies)
        {
            InitValues.PlayLowPrioAudioSource(gameObject, hitClip, 0, 0);
            anim.SetTrigger(isAttackingId);
            //Debug.DrawLine(this.transform.position, enemy.transform.position, this.transform.Find("Base").gameObject.GetComponent<Renderer>().material.GetColor("_Color"), 0.2f);
            enemy.ApplyDmg(AttackPower, gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HudDecrementPhysicalObjectAmountByOne(PhysicalObjectType.MeleeMinion);
    }
}
