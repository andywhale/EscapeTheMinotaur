using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpear : BaseEnemy
{
    [SerializeField] public int range = 2;

    public override void Attack()
    {
        var hero = UnitManager.Instance.GetHero();

        hero.TakesDamage(AttackDamage());
        GetComponentInChildren<Fire>().Shoot(hero.OccupiedTile.transform);
        recharge = true;
    }

    protected override bool InRange(Vector3 a, Vector3 b)
    {
        if (a.x >= b.x - (range) && a.x <= b.x + (range) && a.y == b.y) return true;
        if (a.y >= b.y - (range) && a.y <= b.y + (range) && a.x == b.x) return true;

        return false;
    }


}
