using UnityEngine;
using UnityEngine.Networking;

public class MeleeTower : Tower
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip hitClip;

    private GameObject blade;

    protected override void Start()
    {
        base.Start();
        blade = this.transform.Find("Blade").gameObject;
        HudDIncrementPhysicalObjectAmountByOne(PhysicalObjectType.MeleeTower);
        if (isServer) InvokeRepeating("HitAllEnemies", 0, TimeTillTick());
    }

    protected override void Update()
    {
        base.Update();
        blade.transform.Rotate(new Vector3(0, AttackSpeed * 200, 0) * Time.deltaTime);
    }

    [ServerCallback]
    private void HitAllEnemies()
    {
        Enemies.RemoveAll(PhysicalObjects => PhysicalObjects == null);
        foreach (PhysicalObject enemy in Enemies)
        {
            RpcQuadrupelSound();
            //Debug.DrawLine(this.transform.position, enemy.transform.position);
            enemy.ApplyDmg(this.AttackPower, this.gameObject);
        }
    }

    [ClientRpc]
    private void RpcQuadrupelSound()
    {
        for (int i = 0; i < 4; i++)
        {
            InitValues.PlayLowPrioAudioSource(gameObject, hitClip, i * 0.2f, 0);
        }
    }

    protected override float GetAttackPowerUpgradeValue()
    {
        return InitValues.upradeMeleeTowerAttackPower;
    }

    protected override float GetAttackSpeedUpgradeValue()
    {
        return InitValues.upradeMeleeTowerAttackSpeed;
    }

    protected override float GetHealthUpgradeValue()
    {
        return InitValues.upradeMeleeTowerHealth;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HudDecrementPhysicalObjectAmountByOne(PhysicalObjectType.MeleeTower);
    }
}
