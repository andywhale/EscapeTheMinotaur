using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMage : BaseEnemy
{
    [SerializeField] public int range = 1;

    public override void Attack()
    {
        var hero = UnitManager.Instance.GetHero();

        hero.TakesDamage(AttackDamage());
        GetComponentInChildren<Fire>().Shoot(hero.OccupiedTile.transform);
        recharge = true;
    }

    protected override bool InRange(Vector3 a, Vector3 b)
    {
        if (a.x >= b.x - (range * 2) && a.x <= b.x + (range * 2) && a.y == b.y) return true;
        if (a.y >= b.y - (range * 2) && a.y <= b.y + (range * 2) && a.x == b.x) return true;

        if (a.x - range > b.x || a.x + range < b.x) return false;
        if (a.y - range > b.y || a.y + range < b.y) return false;

        return true;
    }


}
