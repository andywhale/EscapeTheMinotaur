using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction faction;
    [SerializeField] private int health;
    [SerializeField] private int damage;

    public int Health => health;

    public virtual void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public virtual bool CarryOutAction(Tile tile = null)
    {
        return false;
    }

    public virtual void TakesDamage(int damageAmount = 1)
    {
        var newHealth = Health - damageAmount;

        if (newHealth <= 0)
        {
            this.OccupiedTile.OccupiedUnit = null;
        }

        SetHealth(newHealth);
    }

    protected virtual void Move(Tile tile)
    {
        if (!tile.Walkable) return;
        if (!InRange(this.OccupiedTile.transform.position, tile.transform.position)) return;
        tile.SetUnit(this);
    }

    public void ShowRange()
    {
        foreach (KeyValuePair<Vector2, Tile> tile in GridManager.Instance.GetTiles())
        {
            if (InRange(this.OccupiedTile.transform.position, tile.Value.transform.position))
            {
                tile.Value.Highlight();
            }
        }
    }

    public int AttackDamage()
    {
        return damage;
    }

    protected virtual bool TargetInRange(BaseUnit target)
    {
        return InRange(this.OccupiedTile.transform.position, target.OccupiedTile.transform.position);
    }

    protected virtual bool InRange(Vector3 a, Vector3 b)
    {
        return true;
    }
}
