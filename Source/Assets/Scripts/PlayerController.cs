using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject heroPrefab;
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject meleeMinionPrefab;
    [SerializeField]
    private GameObject rangeMinionPrefab;
    [SerializeField]
    private GameObject meleeTowerPrefab;
    [SerializeField]
    private GameObject rangeTowerPrefab;
    [SerializeField]
    private GameObject arrowPrefab;

    public TeamTag team;
    private Base enemyBase;
    private Base playerBase;
    private Hero playerHero;

    private LayerMask floorLayer;

    [SyncVar]
    private PhysicalAttributes baseAttributes;
    [SyncVar]
    private PhysicalAttributes meleeMinionAttributes;
    [SyncVar]
    private PhysicalAttributes rangeMinionAttributes;
    [SyncVar]
    private PhysicalAttributes meleeTowerAttributes;
    [SyncVar]
    private PhysicalAttributes rangeTowerAttributes;
    [SyncVar]
    private PhysicalAttributes heroAttributes;
    [SyncVar(hook = "OnChangeGold")]
    public int gold;

    public Base EnemyBase
    {
        get
        {
            if (enemyBase == null)
            {
                foreach (Base someoneBase in FindObjectsOfType<Base>())
                {
                    if (someoneBase.Team != team) enemyBase = someoneBase;
                }
            }
            return enemyBase;
        }
    }

    public Base PlayerBase
    {
        get
        {
            if (playerBase == null)
            {
                foreach (Base someoneBase in FindObjectsOfType<Base>())
                {
                    if (someoneBase.Team == team) playerBase = someoneBase;
                }
            }
            return playerBase;
        }
    }

    public Hero PlayerHero
    {
        get
        {
            if (playerHero == null)
            {
                foreach (Hero someoneHero in FindObjectsOfType<Hero>())
                {
                    if (someoneHero.Team == team) playerHero = someoneHero;
                }
            }
            return playerHero;
        }
    }

    [Client]
    private void OnChangeGold(int value)
    {
        gold = value;
        if (!isLocalPlayer) return;
        if (HUD.Instance != null) HUD.Instance.SetGold(value);
    }
    
    public override void OnStartAuthority()
    {
        CmdCheckPlayer();
    }

    [Command]
    private void CmdCheckPlayer()
    {
        if (NetworkManager.singleton.numPlayers == NetworkManager.singleton.maxConnections)
        {
            foreach (PlayerController playerController in FindObjectsOfType<PlayerController>())
            {
                playerController.PlayerReady();
            }
        }
    }

    [ServerCallback]
    public void PlayerReady()
    {
        InvokeRepeating("SpawnWave", 0, PhysicalObject.TimeTillTick(0.05f));
    }

    private void Start()
    {
        floorLayer = LayerMask.GetMask("Clickable");
        SetTeam();
        InitValues.InitBase(ref baseAttributes);
        InitValues.InitHero(ref heroAttributes);
        InitValues.InitMeleeMinion(ref meleeMinionAttributes);
        InitValues.InitRangeMinion(ref rangeMinionAttributes);
        InitValues.InitMeleeTower(ref meleeTowerAttributes);
        InitValues.InitRangeTower(ref rangeTowerAttributes);
        SpawnPhysicalObject(transform.position, PhysicalObjectType.Base, team, baseAttributes);
        SpawnPhysicalObject(transform.position, PhysicalObjectType.Hero, team, heroAttributes);
        StartCoroutine(StartHUD());
        gold = InitValues.startGold;
    }

    private void Update()
    {
        ControllHero();
    }

    [Client]
    private void ControllHero()
    {
        if (!isLocalPlayer) return;
        if (PlayerHero == null) return;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && !PlayerHero.IsDead())
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, floorLayer))
            {
                CmdHeroMove(hit.point);
                CmdGetTarget(hit.transform.gameObject);
            }
        }

    }

    [Command]
    private void CmdHeroMove(Vector3 destination)
    {
        PlayerHero.HeroMove(destination);
    }

    [Command]
    private void CmdGetTarget(GameObject newTarget)
    {
        PlayerHero.GetTarget(newTarget);
    }

    [Command]
    private void CmdSpawnPhysicalObject(Vector3 position, PhysicalObjectType type, TeamTag tag, PhysicalAttributes attributes)
    {
        SpawnPhysicalObject(position, type, tag, attributes);
    }

    [Command]
    private void CmdSpawnMinion(Vector3 position, PhysicalObjectType type, TeamTag tag, PhysicalAttributes attributes, GameObject target)
    {
        if (EnemyBase == null) return;
        Minion newMinion = SpawnPhysicalObject(position, type, tag, attributes).GetComponent<Minion>();
        newMinion.finalTarget = target;
    }

    [ServerCallback]
    public GameObject SpawnPhysicalObject(Vector3 position, PhysicalObjectType type, TeamTag tag, PhysicalAttributes attributes)
    {
        GameObject gameObject = null;
        switch (type)
        {
            case PhysicalObjectType.Base:
                switch (tag)
                {
                    case TeamTag.Zero:
                        gameObject = Instantiate(basePrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                        break;
                    case TeamTag.Alpha:
                        gameObject = Instantiate(basePrefab, position + new Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 45));
                        break;
                    case TeamTag.Omega:
                        gameObject = Instantiate(basePrefab, position + new Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 225));
                        break;
                    default:
                        break;
                }
                break;
            case PhysicalObjectType.Hero:
                gameObject = Instantiate(heroPrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                break;
            case PhysicalObjectType.MeleeMinion:
                gameObject = Instantiate(meleeMinionPrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                break;
            case PhysicalObjectType.RangeMinion:

                gameObject = Instantiate(rangeMinionPrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                break;
            case PhysicalObjectType.MeleeTower:

                gameObject = Instantiate(meleeTowerPrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                break;
            case PhysicalObjectType.RangeTower:

                gameObject = Instantiate(rangeTowerPrefab, position + new Vector3(0, 1, 0), Quaternion.identity);
                break;
            default:
                return null;
        }
        NetworkServer.Spawn(gameObject);
        gameObject.GetComponent<PhysicalObject>().Team = tag;
        gameObject.GetComponent<PhysicalObject>().PhysicalAttributes = attributes;
        return gameObject;
    }

    private void SetTeam()
    {
        GameObject sp1 = GameObject.Find("Spawn1");
        GameObject sp2 = GameObject.Find("Spawn2");

        if (transform.position == sp1.transform.position)
        {
            team = TeamTag.Alpha;
        }
        else if (transform.position == sp2.transform.position)
        {
            team = TeamTag.Omega;
        }
        else
        {
            team = TeamTag.Zero;
        }
    }

    [Client]
    public void HeroRespawn()
    {
        if (!isLocalPlayer) return;
        StartCoroutine("HeroRespawnCount");
    }

    [Client]
    private IEnumerator HeroRespawnCount()
    {
        if (!isLocalPlayer) yield break;
        const int RESPAWNTIME = 25;
        for (int i = 0; i < RESPAWNTIME; i++)
        {
            if (HUD.Instance != null)
            {
                HUD.Instance.ShowWarningNotification("Respawn in " + (RESPAWNTIME - i) + " ...");
            }
            yield return new WaitForSeconds(1);
        }
        CmdSpawnPhysicalObject(transform.position, PhysicalObjectType.Hero, team, heroAttributes);
    }


    [ServerCallback]
    private void SpawnWave()
    {
        if (EnemyBase == null) return;
        const int WAVECOUNT = 1;
        for (int i = 0; i < WAVECOUNT; i++)
        {
            CmdSpawnMinion(PlayerBase.transform.position, PhysicalObjectType.MeleeMinion, team, meleeMinionAttributes, EnemyBase.gameObject);
        }
        for (int i = 0; i < WAVECOUNT; i++)
        {
            CmdSpawnMinion(PlayerBase.transform.position, PhysicalObjectType.RangeMinion, team, rangeMinionAttributes, EnemyBase.gameObject);
        }
    }


    [Client]
    private IEnumerator StartHUD()
    {
        yield return new WaitUntil(() => HUD.Instance != null && PlayerBase != null);
        if (!isLocalPlayer) yield break;
        HUD.Instance.OnPlaceMeleeTower += PlaceMeleeTower;
        HUD.Instance.OnPlaceRangeTower += PlaceRangeTower;
        HUD.Instance.OnSpawnMeleeMinion += SpawnMeleeMinion;
        HUD.Instance.OnSpawnRangeMinion += SpawnRangeMinion;
        HUD.Instance.OnUpgradeMeleeMinionAttackPower += UpgradeMeleeMinionAttackPower;
        HUD.Instance.OnUpgradeMeleeMinionHealth += UpgradeMeleeMinionHealth;
        HUD.Instance.OnUpgradeRangeMinionAttackPower += UpgradeRangeMinionAttackPower;
        HUD.Instance.OnUpgradeRangeMinionHealth += UpgradeRangeMinionHealth;
        HUD.Instance.OnUpgradeHeroAttackPower += UpgradeHeroAttackPower;
        HUD.Instance.OnUpgradeHeroArmor += UpgradeHeroArmor;
        HUD.Instance.OnUpgradeHeroAttackSpeed += UpgradeHeroAttackSpeed;
        HUD.Instance.OnUpgradeHeroHealth += UpgradeHeroHealth;

        HUD.Instance.SetGold(gold);
        PlayerBase.lifeBar = HUD.Instance.heroLifebarSlider;
        if (PlayerBase.lifeBar != null)
        {
            PlayerBase.lifeBar.maxValue = PlayerBase.MaxHealth;
            PlayerBase.lifeBar.value = PlayerBase.CurrentHealth;
        }
        yield return StartCoroutine(HUD.Instance.SetHeroStats(PlayerHero.GetComponent<Hero>().AttackPower, PlayerHero.GetComponent<Hero>().Armor, PlayerHero.GetComponent<Hero>().AttackSpeed, team));
    }

    [Client]
    private void SpawnMeleeMinion()
    {
        if (PlayerHero == null) return;
        if (!EnemyIsReady()) return;
        if (!IsOnYourSide(PlayerHero.gameObject)) return;
        if (EnemyBase == null) return;
        if (!TryToPay(meleeMinionAttributes.spawnGoldCosts)) return;
        CmdSpawnMinion(PlayerHero.transform.position, PhysicalObjectType.MeleeMinion, team, meleeMinionAttributes, EnemyBase.gameObject);
    }

    [Client]
    private void SpawnRangeMinion()
    {
        if (PlayerHero == null) return;
        if (!EnemyIsReady()) return;
        if (!IsOnYourSide(PlayerHero.gameObject)) return;
        if (!TryToPay(meleeMinionAttributes.spawnGoldCosts)) return;
        CmdSpawnMinion(PlayerHero.transform.position, PhysicalObjectType.RangeMinion, team, rangeMinionAttributes, EnemyBase.gameObject);
    }

    [Client]
    private void PlaceMeleeTower()
    {
        if (PlayerHero == null) return;
        if (!EnemyIsReady()) return;
        if (!IsOnYourSide(PlayerHero.gameObject)) return;
        if (TowerInRange(PlayerHero.transform.position, GetTowerRadius(meleeTowerPrefab) * 2))
        {
            if (HUD.Instance != null)
            {
                HUD.Instance.ShowWarningNotification("Zu nah an einem anderen Tower");
            }
            return;
        }
        if (!TryToPay(meleeTowerAttributes.spawnGoldCosts)) return;
        CmdSpawnPhysicalObject(PlayerHero.transform.position, PhysicalObjectType.MeleeTower, team, meleeTowerAttributes);
    }

    [Client]
    private void PlaceRangeTower()
    {
        if (PlayerHero == null) return;
        if (!EnemyIsReady()) return;
        if (!IsOnYourSide(PlayerHero.gameObject)) return;
        if (TowerInRange(PlayerHero.transform.position, GetTowerRadius(rangeTowerPrefab) * 2))
        {
            if (HUD.Instance != null)
            {
                HUD.Instance.ShowWarningNotification("Zu nah an einem anderen Towen");
            }
            return;
        }
        if (!TryToPay(rangeTowerAttributes.spawnGoldCosts)) return;
        CmdSpawnPhysicalObject(PlayerHero.transform.position, PhysicalObjectType.RangeTower, team, rangeTowerAttributes);
    }

    [Client]
    private void UpgradeMeleeMinionAttackPower()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(meleeMinionAttributes.upgradeGoldCosts)) return;
        meleeMinionAttributes.attackPower += InitValues.upgradeMeleeMinionAttackPower;
        CmdSetMeleeMinionPhysicalAttributes(meleeMinionAttributes);
    }

    [Client]
    private void UpgradeMeleeMinionHealth()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(meleeMinionAttributes.upgradeGoldCosts)) return;
        meleeMinionAttributes.maxHealth += InitValues.upgradeMeleeMinionHealth;
        CmdSetMeleeMinionPhysicalAttributes(meleeMinionAttributes);
    }

    [Command]
    private void CmdSetMeleeMinionPhysicalAttributes(PhysicalAttributes physicalAttributes)
    {
        meleeMinionAttributes = physicalAttributes;
        foreach (MeleeMinion meleeMinon in FindObjectsOfType<MeleeMinion>())
        {
            if (meleeMinon.Team != team) continue;
            meleeMinon.PhysicalAttributes = physicalAttributes;
        }
    }

    [Client]
    private void UpgradeRangeMinionAttackPower()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(rangeMinionAttributes.upgradeGoldCosts)) return;
        rangeMinionAttributes.attackPower += InitValues.upgradeRangeMinionAttackPower;
        CmdSetRangeMinionPhysicalAttributes(rangeMinionAttributes);
    }

    [Client]
    private void UpgradeRangeMinionHealth()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(rangeMinionAttributes.upgradeGoldCosts)) return;
        rangeMinionAttributes.maxHealth += InitValues.upgradeRangeMinionHealth;
        CmdSetRangeMinionPhysicalAttributes(rangeMinionAttributes);

    }

    [Command]
    private void CmdSetRangeMinionPhysicalAttributes(PhysicalAttributes physicalAttributes)
    {
        rangeMinionAttributes = physicalAttributes;
        foreach (RangeMinion rangeMinon in FindObjectsOfType<RangeMinion>())
        {
            if (rangeMinon.Team != team) continue;
            rangeMinon.PhysicalAttributes = physicalAttributes;
        }
    }

    [Client]
    private void UpgradeHeroAttackPower()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(heroAttributes.upgradeGoldCosts)) return;
        heroAttributes.attackPower += InitValues.upgradeHeroAttackPower;
        CmdSetHeroPhysicalAttributes(heroAttributes);
    }


    [Client]
    private void UpgradeHeroArmor()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(heroAttributes.upgradeGoldCosts)) return;
        heroAttributes.armor += InitValues.upgradeHeroArmor;
        CmdSetHeroPhysicalAttributes(heroAttributes);

    }

    [Client]
    private void UpgradeHeroAttackSpeed()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(heroAttributes.upgradeGoldCosts)) return;
        heroAttributes.attackSpeed += InitValues.upgradeHeroAttackSpeed;
        CmdSetHeroPhysicalAttributes(heroAttributes);
    }

    [Client]
    private void UpgradeHeroHealth()
    {
        if (!EnemyIsReady()) return;
        if (!TryToPay(heroAttributes.upgradeGoldCosts)) return;
        heroAttributes.maxHealth += InitValues.upgradeHeroHealth;
        CmdSetHeroPhysicalAttributes(heroAttributes);
    }

    [Command]
    private void CmdSetHeroPhysicalAttributes(PhysicalAttributes physicalAttributes)
    {
        heroAttributes = physicalAttributes;
        PlayerHero.PhysicalAttributes = physicalAttributes;
    }

    [Command]
    public void CmdSetTowerPhysicalAttributes(PhysicalAttributes physicalAttributes, int id)
    {
        foreach (Tower tower in FindObjectsOfType<Tower>())
        {
            if (tower.Team != team) continue;
            if (tower.Id != id) continue;
            tower.PhysicalAttributes = physicalAttributes;
        }
    }

    private bool EnemyIsReady()
    {
        if (EnemyBase == null)
        {
            if (HUD.Instance != null)
            {
                HUD.Instance.ShowWarningNotification("Enemy is not ready");
            }
            return false;
        }
        return true;
    }

    [Client]
    public bool TryToPay(int cost)
    {
        if (gold >= cost)
        {
            CmdPay(cost);
            return true;
        }
        else
        {
            if (HUD.Instance != null)
            {
                HUD.Instance.ShowWarningNotification(HUD.defaultWarningMessage);
            }
            return false;
        }
    }

    [Command]
    private void CmdPay(int cost)
    {
        if (gold >= cost)
        {
            gold -= cost;
        }
    }

    private bool IsOnYourSide(GameObject gameObject)
    {
        foreach (Collider item in Physics.OverlapBox(transform.position, new Vector3(100, 1, 35), Quaternion.Euler(0, 45, 0)))
        {
            if (item.GetComponent<PhysicalObject>() == null) continue;
            if (item.GetComponent<PhysicalObject>().Team != team) continue;
            if (item.name == gameObject.name)
            {
                return true;
            }
        }
        if (HUD.Instance != null)
        {
            HUD.Instance.ShowWarningNotification("Nicht in deiner Spielhälfte");
        }
        return false;
    }

    private bool TowerInRange(Vector3 position, float radius)
    {
        foreach (Collider item in Physics.OverlapSphere(position, radius))
        {
            if (item.GetComponent<PhysicalObject>() == null) continue;
            if (item.GetComponent<PhysicalObject>().Team != team) continue;
            if (item.name == "MeleeTower(Clone)") return true;
            if (item.name == "RangeTower(Clone)") return true;
        }
        return false;
    }

    private float GetTowerRadius(GameObject gameObject)
    {
        float returnvalue = 0;
        foreach (CapsuleCollider item in gameObject.GetComponentsInChildren<CapsuleCollider>(true))
        {
            if (item.isTrigger) continue;
            returnvalue = item.radius;
        }
        return returnvalue;
    }

    public static PlayerController GetOwnPlayerControler()
    {
        foreach (PlayerController playerController in FindObjectsOfType<PlayerController>())
        {
            if (playerController.isLocalPlayer)
            {
                return playerController;
            }
        }
        return null;
    }

    public static PlayerController GetOwnPlayerControler(TeamTag team)
    {
        if (team == TeamTag.Zero) return null;
        foreach (PlayerController playerController in FindObjectsOfType<PlayerController>())
        {
            if (playerController.team == team)
            {
                return playerController;
            }
        }
        return null;
    }

    public static PlayerController GetEnemyPlayerControler(TeamTag team)
    {
        foreach (PlayerController playerController in FindObjectsOfType<PlayerController>())
        {
            if (playerController.team != team)
            {
                return playerController;
            }
        }
        return null;
    }


    [ServerCallback]
    public void ShootArrow(GameObject source, Vector3 sourceDelta, GameObject target, float attackPower, TeamTag team)
    {
        GameObject newArrow = Instantiate(arrowPrefab, source.transform.position + sourceDelta, Quaternion.identity);
        newArrow.GetComponent<Arrow>().FireArrow(target, source, attackPower, team);
        NetworkServer.Spawn(newArrow);
    }

    public void StopGame()
    {
        CmdCloseHost();
    }

    [Command]
    public void CmdCloseHost()
    {
        RpcCloseHost();
    }

    [ClientRpc]
    public void RpcCloseHost()
    {
        AutoNetworkManager lobby = NetworkManager.singleton as AutoNetworkManager;
        Debug.Log(lobby);
        if (lobby == null) return;
        lobby.StopHost();
    }
}
