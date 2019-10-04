using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class PhysicalObject : NetworkBehaviour
{
    [SyncVar(hook = "OnChangePhysicalAttributes")]
    private PhysicalAttributes physicalAttributes = new PhysicalAttributes(666, 0, 1, 1, 0, 2, 20, 10, 5, 10, 1);
    [SyncVar(hook = "OnChangeCurrentHealth")]
    private float currentHealth = 100;
    [SyncVar(hook = "OnChangeTeam")]
    private TeamTag team;

    private List<PhysicalObject> enemies = new List<PhysicalObject>();

    public Slider lifeBar;

    protected const string moveId = "Move";
    protected const string isDeadId = "IsDead";
    protected const string isAttackingId = "IsAttacking";
    protected const string attackingSpeedId = "AttackingSpeed";

    public PhysicalAttributes PhysicalAttributes
    {
        private get { return physicalAttributes; }
        set
        {
            if (!isServer) return;
            physicalAttributes = value;
            currentHealth = physicalAttributes.maxHealth;
        }
    }

    [Client]
    private void OnChangePhysicalAttributes(PhysicalAttributes value)
    {
        physicalAttributes = value;
        if (GetComponent<SphereCollider>() != null)
        {
            GetComponent<SphereCollider>().radius = value.maxRange;

        }
        if (lifeBar != null)
        {
            lifeBar.maxValue = value.maxHealth;
            lifeBar.value = CurrentHealth;
        }
        if (HUD.Instance != null)
        {
            if (this is Hero)
            {
                StartCoroutine(HUD.Instance.SetHeroStats(AttackPower, Armor, AttackSpeed, Team));
            }
        }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (!isServer) return;
            currentHealth = value;
        }
    }

    [Client]
    private void OnChangeCurrentHealth(float value)
    {
        currentHealth = value;
        if (lifeBar == null) return;
        lifeBar.value = value;
    }

    [Client]
    protected void OnChangeTeam(TeamTag value)
    {
        team = value;
        Transform basePlate = transform.Find("Base");
        if (basePlate == null) return;
        switch (value)
        {
            case TeamTag.Alpha:
                basePlate.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case TeamTag.Omega:
                basePlate.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            default:
                Debug.LogWarning("Wrong Team: " + value);
                break;
        }
    }

    public float MaxHealth
    {
        get { return physicalAttributes.maxHealth; }
    }

    public float Armor
    {
        get { return physicalAttributes.armor; }
    }

    public float AttackPower
    {
        get { return physicalAttributes.attackPower; }
    }
    public float AttackSpeed
    {
        get { return physicalAttributes.attackSpeed; }
    }

    public float MinRange
    {
        get { return physicalAttributes.minRange; }
    }

    public float MaxRange
    {
        get { return physicalAttributes.maxRange; }
    }

    public int DestroyGoldValue
    {
        get { return physicalAttributes.destroyGoldValue; }
    }

    public int SpawnGoldCosts
    {
        get { return physicalAttributes.spawnGoldCosts; }
    }

    public int UpgradeGoldCosts
    {
        get { return physicalAttributes.upgradeGoldCosts; }
    }

    public float MovemenetSpeed
    {
        get { return physicalAttributes.movemenetSpeed; }
    }

    public float DespawnTime
    {
        get { return physicalAttributes.despawnTime; }
    }

    public List<PhysicalObject> Enemies
    {
        get { return enemies; }
    }

    public TeamTag Team
    {
        get { return team; }
        set
        {
            if (isServer) team = value;
        }
    }

    [ServerCallback]
    public void AddEnemy(GameObject enemy)
    {
        if (enemy == null) return;
        if (enemy.GetComponent<PhysicalObject>() == null) return;
        if (enemy.GetComponent<PhysicalObject>().IsDead()) return;
        enemies.Add(enemy.GetComponent<PhysicalObject>());
    }

    [ServerCallback]
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemy == null) return;
        if (enemy.GetComponent<PhysicalObject>() == null) return;
        enemies.Remove(enemy.GetComponent<PhysicalObject>());
    }

    [ClientRpc]
    public void RpcAddEnemy(GameObject enemy)
    {
        if (enemy == null) return;
        if (enemy.GetComponent<PhysicalObject>() == null) return;
        enemies.Add(enemy.GetComponent<PhysicalObject>());
    }

    [ClientRpc]
    public void RpcRemoveEnemy(GameObject enemy)
    {
        if (enemy == null) return;
        if (enemy.GetComponent<PhysicalObject>() == null) return;
        enemies.Add(enemy.GetComponent<PhysicalObject>());
    }

    private void Awake()
    {
        currentHealth = physicalAttributes.maxHealth;
    }

    protected virtual void Start()
    {
        if (isLocalPlayer)
        {
            if (HUD.Instance != null)
            {
                lifeBar = HUD.Instance.heroLifebarSlider;
            }
        }
        else
        {
            if (this.transform.Find("Life") != null)
            {
                lifeBar = this.transform.Find("Life").gameObject.GetComponent<Slider>();
            }
        }
        if (lifeBar != null)
        {
            lifeBar.maxValue = physicalAttributes.maxHealth;
            lifeBar.value = currentHealth;
        }
    }

    [ServerCallback]
    protected virtual void Update()
    {
        IsDead();
    }

    [ServerCallback]
    public virtual void ApplyDmg(float dmg, GameObject attacker)
    {
        if (!hasAuthority) return;
        if (Armor > 100)
        {
            dmg = dmg - (1 / 100 * Armor);
        }

        CurrentHealth -= Mathf.Abs(dmg);
    }

    [ServerCallback]
    public void ApplyHeal(float heal)
    {
        CurrentHealth += Mathf.Abs(heal);
    }

    protected float TimeTillTick()
    {
        return TimeTillTick(physicalAttributes.attackSpeed);
    }

    public static float TimeTillTick(float attackPerSecond)
    {
        return 1 / attackPerSecond;
    }

    [ServerCallback]
    public virtual bool IsDead()
    {
        if (CurrentHealth <= 0)
        {
            if (physicalAttributes.despawnTime >= 0)
            {
                Destroy(gameObject, physicalAttributes.despawnTime);
                physicalAttributes.despawnTime = -1;
            }
            return true;
        }
        return false;
    }

    [ServerCallback]
    public virtual void ApplyDmgFromAnimation()
    {
    }

    protected virtual void OnDestroy()
    {
        if (PlayerController.GetEnemyPlayerControler(team) == null) return;
        if (!isServer) return;
        PlayerController.GetEnemyPlayerControler(team).gold += DestroyGoldValue;
    }
}