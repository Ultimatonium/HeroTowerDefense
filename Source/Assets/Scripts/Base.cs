using UnityEngine;
using UnityEngine.Networking;

public class Base : AiObject
{

    protected override void Start()
    {
        base.Start();
        InitBaseDefence();
    }

    [ServerCallback]
    private void InitBaseDefence()
    {
        InvokeRepeating("HitAllEnemies", 1, TimeTillTick());
    }

    [ServerCallback]
    private void HitAllEnemies()
    {
        Enemies.RemoveAll(PhysicalObjects => PhysicalObjects == null);
        foreach (PhysicalObject enemy in Enemies)
        {
            PlayerController.GetOwnPlayerControler(Team).ShootArrow(gameObject, new Vector3(0, 1, 0), enemy.gameObject, AttackPower, Team);
        }
    }

    [Client]
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (HUD.Instance == null) return;
        if (!PlayerController.GetOwnPlayerControler(Team).isLocalPlayer)
        {
            HUD.Instance.ShowWinLost(true);
        }
        if (PlayerController.GetOwnPlayerControler(Team).isLocalPlayer)
        {
            HUD.Instance.ShowWinLost(false);
        }
    }
}
