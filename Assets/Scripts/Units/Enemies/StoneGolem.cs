using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGolem : BaseEnemy
{

    public override bool CarryOutAction(Tile tile = null)
    {
        bool action = false;
        var hero = UnitManager.Instance.GetHero();

        if (recharge)
        {
            recharge = false;
            return false;
        }

        if (TargetInRange(hero))
        {
            Attack();
            action = true;
        }
        else
        {
            // Randomly pause sometimes
            if (Random.Range(0, 10) <= 1)
            {
                return false;
            }

            // If not then move towards the hero
            var targetTile = FindClosestTileInTargetDirection(hero.OccupiedTile);
            if (targetTile)
            {
                Move(targetTile);
                action = true;
            }
        }

        return action;

    }

    protected override bool InRange(Vector3 a, Vector3 b)
    {
        if (a.x >= b.x - 1 && a.x <= b.x + 1 && a.y == b.y) return true;
        if (a.y >= b.y - 1 && a.y <= b.y + 1 && a.x == b.x) return true;
        return false;
    }

}
