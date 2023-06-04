using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    protected bool recharge;

    public override bool CarryOutAction(Tile tile = null)
    {
        var hero = UnitManager.Instance.GetHero();

        // After an attack an enemy must recharge
        if (recharge)
        {
            recharge = false;
            return false;
        }

        // Is the hero in range?
        if (TargetInRange(hero))
        {
            Attack();
            return true;
        }
        else
        {
            // Randomly pause sometimes
            if (Random.Range(0, 10) <= 1)
            {
                return false;
            }

            // If not then move towards the hero (basic for now, then add pathfinding)
            var targetTile = FindClosestTileInTargetDirection(hero.OccupiedTile);
            if (targetTile)
            {
                Move(targetTile);
                return true;
            }
        }

        return false;
    }

    public virtual void Attack()
    {
        var hero = UnitManager.Instance.GetHero();

        hero.TakesDamage(AttackDamage());
        recharge = true;
    }

    public override void TakesDamage(int damageAmount = 1)
    {
        var newHealth = Health - damageAmount;
        SetHealth(newHealth);

        if (newHealth <= 0)
        {
            this.OccupiedTile.OccupiedUnit = null;
            UnitManager.Instance.KillEnemy(this);
        }
    }

    public Tile FindClosestTileInTargetDirection(Tile targetTile)
    {
        return GridManager.Instance.FindNextMove(OccupiedTile, targetTile);
    }
}
