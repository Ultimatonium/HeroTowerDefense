using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AiObject : PhysicalObject
{
    protected override void Start()
    {
        base.Start();
        this.GetComponent<SphereCollider>().radius = MaxRange;
    }

    public bool IsRelevant(Collider other)
    {
        if (other.isTrigger) return false;
        if (other.gameObject.GetComponent<PhysicalObject>() == null) return false;
        if (other.gameObject.GetComponent<PhysicalObject>().Team == Team) return false;
        return true;
    }

    [ServerCallback]
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsRelevant(other))
        {
            AddEnemy(other.gameObject);
        }
    }

    [ServerCallback]
    protected void OnTriggerExit(Collider other)
    {
        if (IsRelevant(other))
        {
            RemoveEnemy(other.gameObject);
        }
    }

    protected void HudDIncrementPhysicalObjectAmountByOne(PhysicalObjectType type)
    {
        StartCoroutine(CoroutineHudDIncrementPhysicalObjectAmountByOne(type));
    }

    private IEnumerator CoroutineHudDIncrementPhysicalObjectAmountByOne(PhysicalObjectType type)
    {
        yield return new WaitUntil(() => PlayerController.GetOwnPlayerControler(Team) != null);
        if (!PlayerController.GetOwnPlayerControler(Team).isLocalPlayer) yield break;
        if (HUD.Instance == null) yield break;
        HUD.Instance.IncrementPhysicalObjectAmountByOne(type);
    }

    protected void HudDecrementPhysicalObjectAmountByOne(PhysicalObjectType type)
    {
        if (PlayerController.GetOwnPlayerControler(Team) == null) return;
        if (!PlayerController.GetOwnPlayerControler(Team).isLocalPlayer) return;
        if (HUD.Instance == null) return;
        HUD.Instance.DecrementPhysicalObjectAmountByOne(type);
    }
}
