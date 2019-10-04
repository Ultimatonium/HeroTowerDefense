using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class HUD : MonoBehaviour
{

    public static HUD Instance { get; private set; }

    [SerializeField] private Text meleeMinionAmountText;
    [SerializeField] private Text rangeMinionAmountText;
    [SerializeField] private Text meleeTowerAmountText;
    [SerializeField] private Text rangeTowerAmountText;
    [SerializeField] private Text goldAmountText;
    [SerializeField] private Text gameTimeText;
    [SerializeField] private GameObject warningNotificationBox;
    [SerializeField] private Text heroPowerText;
    [SerializeField] private Text heroArmorText;
    [SerializeField] private Text heroAttackSpeedText;
    [SerializeField] public Slider heroLifebarSlider;
    [SerializeField] private GameObject towerInformationBox;
    [SerializeField] private Text towerAttackPowerText;
    [SerializeField] private Text towerLifepointsText;
    [SerializeField] private Text towerAttackSpeedText;
    [SerializeField] private GameObject closeGameBox;
    [SerializeField] private GameObject winLostBox;
    [SerializeField] private Text winLostText;

    //public static Action OnHUDIsReady { get; set; }

    public Action OnPlaceMeleeTower { get; set; }
    public Action OnPlaceRangeTower { get; set; }
    public Action OnSpawnMeleeMinion { get; set; }
    public Action OnSpawnRangeMinion { get; set; }

    public Action OnUpgradeMeleeMinionAttackPower { get; set; }
    public Action OnUpgradeMeleeMinionHealth { get; set; }

    public Action OnUpgradeRangeMinionAttackPower { get; set; }
    public Action OnUpgradeRangeMinionHealth { get; set; }

    public Action<int> OnUpradeTowerAttackPower { get; set; }
    public Action<int> OnUpradeTowerAttackSpeed { get; set; }
    public Action<int> OnUpradeTowerHealth { get; set; }

    public Action OnUpgradeHeroAttackPower { get; set; }
    public Action OnUpgradeHeroArmor { get; set; }
    public Action OnUpgradeHeroAttackSpeed { get; set; }
    public Action OnUpgradeHeroHealth { get; set; }

    private bool isTimeRunning = true;
    private float timeIngame = 0;

    public const string defaultWarningMessage = "Du hast nicht genug Gold";

    private int towerID = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        ManageTime();
        TowerRaycast();
    }

    private void Init()
    {
        //if (OnHUDIsReady != null) OnHUDIsReady();
        if (meleeMinionAmountText != null) meleeMinionAmountText.text = 0.ToString();
        if (rangeMinionAmountText != null) rangeMinionAmountText.text = 0.ToString();
        if (meleeTowerAmountText != null) meleeTowerAmountText.text = 0.ToString();
        if (rangeTowerAmountText != null) rangeTowerAmountText.text = 0.ToString();
        if (goldAmountText != null) goldAmountText.text = 0.ToString();
        if (gameTimeText != null) gameTimeText.text = "00:00";
        if (warningNotificationBox != null)
        {
            warningNotificationBox.SetActive(false);
            Text t = warningNotificationBox.GetComponentInChildren<Text>();
            if (t != null) t.text = defaultWarningMessage;
            CanvasGroup cg = warningNotificationBox.GetComponentInChildren<CanvasGroup>();
            if (cg != null) cg.alpha = 0;
        }
        if (heroPowerText != null) heroPowerText.text = 0.ToString();
        if (heroArmorText != null) heroArmorText.text = 0.ToString();
        if (heroAttackSpeedText != null) heroAttackSpeedText.text = 0.ToString();
        if (heroLifebarSlider != null) heroLifebarSlider.value = heroLifebarSlider.maxValue;
        if (towerInformationBox != null)
        {
            towerInformationBox.SetActive(false);
            CanvasGroup cg = towerInformationBox.GetComponentInChildren<CanvasGroup>();
            if (cg != null) cg.alpha = 0;
        }
        if (towerAttackPowerText != null) towerAttackPowerText.text = 0.ToString();
        if (towerLifepointsText != null) towerLifepointsText.text = 0.ToString();
        if (towerAttackSpeedText != null) towerAttackSpeedText.text = 0.ToString();
        if (closeGameBox != null) closeGameBox.SetActive(false);
        if (winLostBox != null) winLostBox.SetActive(false);
    }

    private void TowerRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Clickable")))
            {
                if (hit.collider.gameObject.GetComponent<Tower>() != null)
                {
                    if (hit.collider.gameObject.GetComponent<Tower>().Team == PlayerController.GetOwnPlayerControler().team)
                    {
                        ShowInfoTowerBox(hit.collider.gameObject);
                    }
                    else
                    {
                        HideInfoTowerBox();
                    }
                }
                else
                {
                    HideInfoTowerBox();
                }
            }
        }
    }

    private void ShowInfoTowerBox(GameObject hit)
    {
        if (towerInformationBox == null) return;
        if (towerAttackPowerText == null) return;
        if (towerLifepointsText == null) return;
        if (towerAttackSpeedText == null) return;

        CanvasGroup cg = towerInformationBox.GetComponent<CanvasGroup>();
        if (cg == null) return;

        Tower tower = hit.GetComponentInChildren<Tower>();
        if (tower == null) return;

        UpdateInfoTowerBox(tower);

        if (towerID == tower.Id) return;
        towerID = tower.Id;
        StartCoroutine(IEFade(cg, FadeEnum.FadeIn, 0.2f));
    }

    private void HideInfoTowerBox()
    {
        if (!towerInformationBox.activeSelf) return;

        CanvasGroup cg = towerInformationBox.GetComponent<CanvasGroup>();
        if (cg == null) return;

        towerID = -1;
        StartCoroutine(IEFade(cg, FadeEnum.FadeOut, 0.2f));
    }

    public void UpdateInfoTowerBox(Tower tower)
    {
        towerAttackPowerText.text = tower.AttackPower.ToString();
        towerAttackSpeedText.text = tower.AttackSpeed.ToString();
        towerLifepointsText.text = tower.MaxHealth.ToString();
    }

    private void ManageTime()
    {
        if (!isTimeRunning) return;
        if (gameTimeText == null) return;

        timeIngame += Time.deltaTime;
        if (timeIngame > 3600)
        {
            gameTimeText.text = "too long";
            return;
        }

        TimeSpan t = TimeSpan.FromSeconds(timeIngame);
        gameTimeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }

    public void PlaceMeleeTower()
    {
        if (OnPlaceMeleeTower == null) return;
        OnPlaceMeleeTower();
    }

    public void PlaceRangeTower()
    {
        if (OnPlaceRangeTower == null) return;
        OnPlaceRangeTower();
    }

    public void SpawnMeleeMinion()
    {
        if (OnSpawnMeleeMinion == null) return;
        OnSpawnMeleeMinion();
    }

    public void SpawnRangeMinion()
    {
        if (OnSpawnRangeMinion == null) return;
        OnSpawnRangeMinion();
    }

    public void LevelUpMeleeMinionAttackPower()
    {
        LevelUpPhysicalObject(PhysicalObjectType.MeleeMinion, Attribute.AttackPower);
    }

    public void LevelUpMeleeMinionLifePoints()
    {
        LevelUpPhysicalObject(PhysicalObjectType.MeleeMinion, Attribute.LifePoints);
    }

    public void LevelUpRangeMinionAttackPower()
    {
        LevelUpPhysicalObject(PhysicalObjectType.RangeMinion, Attribute.AttackPower);
    }

    public void LevelUpRangeMinionLifePoints()
    {
        LevelUpPhysicalObject(PhysicalObjectType.MeleeMinion, Attribute.LifePoints);
    }

    public void LevelUpHeroAttackPower()
    {
        LevelUpPhysicalObject(PhysicalObjectType.Hero, Attribute.AttackPower);
    }

    public void LevelUpHeroArmor()
    {
        LevelUpPhysicalObject(PhysicalObjectType.Hero, Attribute.Armor);
    }

    public void LevelUpHeroAttackSpeed()
    {
        LevelUpPhysicalObject(PhysicalObjectType.Hero, Attribute.AttackSpeed);
    }

    public void LevelUpHeroLifePoints()
    {
        LevelUpPhysicalObject(PhysicalObjectType.Hero, Attribute.LifePoints);
    }

    public void LevelUpTowerAttackPower()
    {
        if (towerID <= 0) return;
        LevelUpPhysicalObject(PhysicalObjectType.Tower, Attribute.AttackPower, towerID);
    }

    public void LevelUpTowerLifePoints()
    {
        if (towerID <= 0) return;
        LevelUpPhysicalObject(PhysicalObjectType.Tower, Attribute.LifePoints, towerID);
    }

    public void LevelUpTowerAttackSpeed()
    {
        if (towerID <= 0) return;
        LevelUpPhysicalObject(PhysicalObjectType.Tower, Attribute.AttackSpeed, towerID);
    }

    private enum Attribute
    {
        LifePoints,
        AttackPower,
        AttackSpeed,
        Armor
    }

    private void LevelUpPhysicalObject(PhysicalObjectType type, Attribute attribute, int id = 0)
    {
        switch (type)
        {
            case PhysicalObjectType.Hero:
                switch (attribute)
                {
                    case Attribute.LifePoints:
                        if (OnUpgradeHeroHealth != null) OnUpgradeHeroHealth();
                        break;
                    case Attribute.AttackPower:
                        if (OnUpgradeHeroAttackPower != null) OnUpgradeHeroAttackPower();
                        break;
                    case Attribute.Armor:
                        if (OnUpgradeHeroArmor != null) OnUpgradeHeroArmor();
                        break;
                    case Attribute.AttackSpeed:
                        if (OnUpgradeHeroAttackSpeed != null) OnUpgradeHeroAttackSpeed();
                        break;
                }
                break;
            case PhysicalObjectType.MeleeMinion:
                switch (attribute)
                {
                    case Attribute.LifePoints:
                        if (OnUpgradeMeleeMinionHealth != null) OnUpgradeMeleeMinionHealth();
                        break;
                    case Attribute.AttackPower:
                        if (OnUpgradeMeleeMinionAttackPower != null) OnUpgradeMeleeMinionAttackPower();
                        break;
                }
                break;
            case PhysicalObjectType.RangeMinion:
                switch (attribute)
                {
                    case Attribute.LifePoints:
                        if (OnUpgradeRangeMinionHealth != null) OnUpgradeRangeMinionHealth();
                        break;
                    case Attribute.AttackPower:
                        if (OnUpgradeRangeMinionAttackPower != null) OnUpgradeRangeMinionAttackPower();
                        break;
                }
                break;
            case PhysicalObjectType.Tower:
                switch (attribute)
                {
                    case Attribute.LifePoints:
                        if (OnUpradeTowerHealth != null) OnUpradeTowerHealth(id);
                        break;
                    case Attribute.AttackPower:
                        if (OnUpradeTowerAttackPower != null) OnUpradeTowerAttackPower(id);
                        break;
                    case Attribute.AttackSpeed:
                        if (OnUpradeTowerAttackSpeed != null) OnUpradeTowerAttackSpeed(id);
                        break;
                }
                break;

        }
    }

    public void SetGold(int gold)
    {
        if (goldAmountText == null) return;
        goldAmountText.text = Mathf.Max(0, gold).ToString();
    }

    public void SetHeroLifePoints(int life, int maxLife = 0)
    {
        if (heroLifebarSlider == null) return;
        if (maxLife > 0) heroLifebarSlider.maxValue = maxLife;

        heroLifebarSlider.value = life;
    }

    public IEnumerator SetHeroStats(float attackPower, float armor, float attackSpeed, TeamTag team)
    {
        yield return new WaitUntil(() => PlayerController.GetOwnPlayerControler(team) != null);
        if (!PlayerController.GetOwnPlayerControler(team).isLocalPlayer) yield break;
        if (heroPowerText != null) heroPowerText.text = attackPower.ToString();
        if (heroArmorText != null) heroArmorText.text = armor.ToString();
        if (heroAttackSpeedText != null) heroAttackSpeedText.text = attackSpeed.ToString();
    }

    public void IncrementPhysicalObjectAmountByOne(PhysicalObjectType type)
    {
        Text t = GetPhysicalObjectTextComponentByType(type);
        if (t == null) return;
        int amount = 0;
        int.TryParse(t.text, out amount);
        SetPhyiscalObjectAmount(t, amount + 1);
    }

    public void DecrementPhysicalObjectAmountByOne(PhysicalObjectType type)
    {
        Text t = GetPhysicalObjectTextComponentByType(type);
        if (t == null) return;
        int amount = 0;
        int.TryParse(t.text, out amount);
        SetPhyiscalObjectAmount(t, amount - 1);
    }

    public void SetPhyiscalObjectAmount(PhysicalObjectType type, int amount)
    {
        Text t = GetPhysicalObjectTextComponentByType(type);
        SetPhyiscalObjectAmount(t, amount);
    }

    public void ShowWinLost(bool hasWin)
    {
        if (winLostBox == null) return;
        if (winLostText == null) return;

        if (hasWin) winLostText.text = "gewonnen.";
        else winLostText.text = "verloren.";

        winLostBox.SetActive(true);
    }

    public void HideWinlost()
    {
        if (winLostBox == null) return;
        winLostBox.SetActive(false);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        PlayerController.GetOwnPlayerControler().StopGame();
        /*
        AutoNetworkManager lobby = NetworkManager.singleton as AutoNetworkManager;
        if (lobby == null) return;
        lobby.CmdCloseHost();
        */
    }

    private void SetPhyiscalObjectAmount(Text text, int amount)
    {
        if (text == null) return;
        text.text = Mathf.Max(0, amount).ToString();
    }

    private Text GetPhysicalObjectTextComponentByType(PhysicalObjectType type)
    {
        Text t = null;
        switch (type)
        {
            case PhysicalObjectType.MeleeMinion: t = meleeMinionAmountText; break;
            case PhysicalObjectType.RangeMinion: t = rangeMinionAmountText; break;
            case PhysicalObjectType.MeleeTower: t = meleeTowerAmountText; break;
            case PhysicalObjectType.RangeTower: t = rangeTowerAmountText; break;
        }
        return t;
    }

    public void ShowWarningNotification(string message = "")
    {
        if (warningNotificationBox == null) return;

        Text warningNotificationBoxText = warningNotificationBox.GetComponentInChildren<Text>();
        if (warningNotificationBoxText == null) return;

        CanvasGroup warningNotificationBoxCanvasgroup = warningNotificationBox.GetComponentInChildren<CanvasGroup>();
        if (warningNotificationBoxCanvasgroup == null) return;

        if (!string.IsNullOrEmpty(message))
        {
            warningNotificationBoxText.text = message;
        }
        else
        {
            warningNotificationBoxText.text = defaultWarningMessage;
        }

        StartCoroutine(IEFade(warningNotificationBoxCanvasgroup, FadeEnum.FadeIn, 1f, () =>
        {
            StartCoroutine(IEFade(warningNotificationBoxCanvasgroup, FadeEnum.FadeOut, 1f));
        }));
    }

    private enum FadeEnum
    {
        FadeIn,
        FadeOut
    }

    private IEnumerator IEFade(CanvasGroup canvasGroup, FadeEnum fadeType, float time, Action callback = null)
    {
        if (canvasGroup != null)
        {
            canvasGroup.gameObject.SetActive(true);

            float elapsedTime = 0;
            float startAlpha = canvasGroup.alpha;
            while (elapsedTime < time)
            {
                float perc = (1 / time * elapsedTime);
                if (fadeType == FadeEnum.FadeOut) perc = startAlpha - (1 / time * elapsedTime);

                canvasGroup.alpha = perc;
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = 1;
            if (fadeType == FadeEnum.FadeOut)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }

            if (callback != null) callback();
        }
    }
}
