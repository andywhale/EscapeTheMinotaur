using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseUnit
{
    public new int Health()
    {
        return GameManager.Instance.lives;
    }

    public override void SetHealth(int newHealth)
    {
        GameManager.Instance.lives = newHealth;
    }

    public override bool CarryOutAction(Tile tile)
    {
        var unit = tile.GetUnit();
        if (unit != null)
        {
            if (unit.faction == Faction.Enemy)
            {
                if (TargetInRange(unit))
                {
                    unit.TakesDamage(AttackDamage());
                    GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
                    return true;
                } else
                {
                    unit.ShowRange();
                    return false;
                }
            }
        }
        else
        {
            Move(tile);
            return true;
        }
        return false;
    }

    public override void TakesDamage(int damageAmount = 1)
    {
        var currentHealth = Health();
        var newHealth = currentHealth - damageAmount;

        if (newHealth <= 0)
        {
            this.OccupiedTile.OccupiedUnit = null;
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }

        SetHealth(newHealth);
    }

    protected override void Move(Tile targetile)
    {
        if (!InRange(this.OccupiedTile.transform.position, targetile.transform.position))
        {
            targetile = GridManager.Instance.FindNextMove(this.OccupiedTile, targetile);
            if (targetile == null) return;
        }
        if (targetile.Goal)
        {
            targetile.SetUnit(this);
            GameManager.Instance.UpdateGameState(GameState.Victory);
        }
        if (targetile.Reward)
        {
            GridManager.Instance.OpenChest();
            GameManager.Instance.UpdateGameState(GameState.RewardSelect);
        }
        if (!targetile.Walkable) return;
        targetile.SetUnit(this);
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }

    protected override bool InRange(Vector3 a, Vector3 b)
    {
        if (a.x >= b.x - 1 && a.x <= b.x + 1 && a.y == b.y) return true;
        if (a.y >= b.y - 1 && a.y <= b.y + 1 && a.x == b.x) return true;
        return false;
    }
}
