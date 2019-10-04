using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Hero : PhysicalObject
{
    private Animator anim;
    private NavMeshAgent agent;
    private PhysicalObject target;
    private const float stand = 0;
    private bool enemyInRange;

    [Header("Audio")]
    [SerializeField] private AudioClip swordSlashClip;
    protected AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        OnChangeTeam(Team); //BasePlate on Client Fix
    }

    [ServerCallback]
    protected override void Update()
    {
        if (!hasAuthority) return;


        if (enemyInRange && target != null && !target.IsDead())
        {
            Attack();
        }

        if (Vector3.Distance(agent.transform.position, agent.destination) <= 0.1f)
        {
            anim.SetFloat(moveId, stand);
        }
    }

    [ServerCallback]
    public override bool IsDead()
    {
        bool dead = base.IsDead();
        if (dead)
        {
            anim.SetTrigger("IsDead");
            agent.destination = transform.position;
        }
        return dead;
    }

    [ServerCallback]
    public void HeroMove(Vector3 destination)
    {
        agent.destination = destination;
        anim.SetFloat(moveId, MovemenetSpeed);
        agent.speed = MovemenetSpeed;
    }

    [ServerCallback]
    public void GetTarget(GameObject newTarget)
    {
        PhysicalObject physicalTarget = newTarget.GetComponent<PhysicalObject>();
        if (physicalTarget != null)
        {
            if (physicalTarget.Team != Team)
            {
                target = physicalTarget;
            }
            else
            {
                target = null;
            }
        }
        else
        {
            target = null;
        }
    }

    [ServerCallback]
    private void Attack()
    {
        InitValues.PlayLowPrioAudioSource(this.gameObject, swordSlashClip, 0, 0);
        anim.SetTrigger(isAttackingId);
        anim.SetFloat(attackingSpeedId, AttackSpeed);
    }

    [ServerCallback]
    public override void ApplyDmgFromAnimation()
    {
        if (target == null) return;
        target.ApplyDmg(AttackPower, gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider collider)
    {
        if (target == null) return;
        if (collider.gameObject == target.gameObject)
        {
            enemyInRange = true;
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider collider)
    {
        enemyInRange = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayerController.GetOwnPlayerControler(Team) == null) return;
        PlayerController.GetOwnPlayerControler(Team).HeroRespawn();
    }
}