using UnityEngine.Networking;

public abstract class Tower : AiObject
{
    public int Id { get; private set; }

    protected override void Start()
    {
        base.Start();
        Id = unchecked((int)netId.Value); //unchecked just to be sure no OverflowException appears
        RegisterEvents();
    }

    protected void UpdateTowerHUD()
    {
        if (HUD.Instance == null) return;
        HUD.Instance.UpdateInfoTowerBox(this);
    }

    private void RegisterEvents()
    {
        if (HUD.Instance == null) return;
        HUD.Instance.OnUpradeTowerAttackPower += UpradeTowerAttackPower;
        HUD.Instance.OnUpradeTowerAttackSpeed += UpradeTowerAttackSpeed;
        HUD.Instance.OnUpradeTowerHealth += UpradeTowerHealth;
    }

    protected abstract float GetAttackPowerUpgradeValue();
    protected abstract float GetAttackSpeedUpgradeValue();
    protected abstract float GetHealthUpgradeValue();

    [Client]
    private void UpradeTowerAttackPower(int id)
    {
        if (id == Id)
        {
            if (!PlayerController.GetOwnPlayerControler(Team).TryToPay(UpgradeGoldCosts)) return;
            PlayerController.GetOwnPlayerControler(Team).CmdSetTowerPhysicalAttributes(new PhysicalAttributes(MaxHealth
                                                                                                            , Armor
                                                                                                            , AttackPower + GetAttackPowerUpgradeValue()
                                                                                                            , AttackSpeed
                                                                                                            , MinRange
                                                                                                            , MaxRange
                                                                                                            , DestroyGoldValue
                                                                                                            , SpawnGoldCosts
                                                                                                            , UpgradeGoldCosts
                                                                                                            , MovemenetSpeed
                                                                                                            , DespawnTime)
                                                                                     , id);
            UpdateTowerHUD();
        }
    }

    [Client]
    protected void UpradeTowerAttackSpeed(int id)
    {
        if (id == Id)
        {
            if (!PlayerController.GetOwnPlayerControler(Team).TryToPay(UpgradeGoldCosts)) return;
            PlayerController.GetOwnPlayerControler(Team).CmdSetTowerPhysicalAttributes(new PhysicalAttributes(MaxHealth
                                                                                                            , Armor
                                                                                                            , AttackPower
                                                                                                            , AttackSpeed + GetAttackSpeedUpgradeValue()
                                                                                                            , MinRange
                                                                                                            , MaxRange
                                                                                                            , DestroyGoldValue
                                                                                                            , SpawnGoldCosts
                                                                                                            , UpgradeGoldCosts
                                                                                                            , MovemenetSpeed
                                                                                                            , DespawnTime)
                                                                                     , id);
            UpdateTowerHUD();
        }
    }

    [Client]
    protected void UpradeTowerHealth(int id)
    {
        if (id == Id)
        {
            if (!PlayerController.GetOwnPlayerControler(Team).TryToPay(UpgradeGoldCosts)) return;
            PlayerController.GetOwnPlayerControler(Team).CmdSetTowerPhysicalAttributes(new PhysicalAttributes(MaxHealth + GetHealthUpgradeValue()
                                                                                                            , Armor
                                                                                                            , AttackPower
                                                                                                            , AttackSpeed
                                                                                                            , MinRange
                                                                                                            , MaxRange
                                                                                                            , DestroyGoldValue
                                                                                                            , SpawnGoldCosts
                                                                                                            , UpgradeGoldCosts
                                                                                                            , MovemenetSpeed
                                                                                                            , DespawnTime)
                                                                                     , id);
            UpdateTowerHUD();
        }
    }
}
