using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private bool isWalkable;
    [SerializeField] public bool isGoal;
    [SerializeField] public bool isReward;

    public BaseUnit OccupiedUnit;

    public bool Routable => isWalkable;
    public bool Walkable => isWalkable && !isGoal && OccupiedUnit == null;

    public bool Goal => isGoal;
    public bool Reward => isReward;

    [SerializeField] private GameObject highlight;

    [SerializeField] public int GCost; // Movement cost from the start tile
    [SerializeField] public int HCost; // Heuristic cost to the end tile
    [SerializeField] public Tile parent; // Parent tile in the pathfinding
    [SerializeField] private List<Tile> neighbours = new List<Tile>();

    public int FCost
    {
        get { return GCost + HCost; }
    }

    public void SetNeighbours(Dictionary<Vector2, Tile> grid)
    {
        // Add the adjacent tiles
        Vector2 left = new Vector2(transform.position.x - 1, transform.position.y);
        Vector2 right = new Vector2(transform.position.x + 1, transform.position.y);
        Vector2 up = new Vector2(transform.position.x, transform.position.y + 1);
        Vector2 down = new Vector2(transform.position.x, transform.position.y - 1);

        if (grid.ContainsKey(left))
        {
            neighbours.Add(grid[left]);
        }
        if (grid.ContainsKey(right))
        {
            neighbours.Add(grid[right]);
        }
        if (grid.ContainsKey(up))
        {
            neighbours.Add(grid[up]);
        }
        if (grid.ContainsKey(down))
        {
            neighbours.Add(grid[down]);
        }
    }

    public void Reset()
    {
        GCost = 0;
        HCost = 0;
        parent = null;
    }

    public List<Tile> GetNeighbours()
    {
        return neighbours;
    }

    public void Init(bool isOffset)
    {
        renderer.color = baseColor;
        if (isOffset)
        {
            renderer.color = offsetColor;
        }
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        highlight.SetActive(true);
    }

    public void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        highlight.SetActive(false);
    }

    public void Highlight()
    {
        highlight.SetActive(true);
    }

    public void RemoveHighlight()
    {
        highlight.SetActive(false);
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public BaseUnit GetUnit()
    {
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.faction == Faction.Enemy)
            {
                return (BaseEnemy)OccupiedUnit;
            } else if (OccupiedUnit.faction == Faction.Hero)
            {
                return (BaseHero)OccupiedUnit;
            }
        }
        return null;
    }

    public bool OnMouseDown()
    {
        if (GameManager.Instance.state != GameState.PlayerTurn) return false;
        return UnitManager.Instance.PlayerCarryOutAction(this);
    }

    public void OnMouseUp()
    {
        foreach (KeyValuePair<Vector2, Tile> tile in GridManager.Instance.GetTiles())
        {
            tile.Value.RemoveHighlight();
        }
    }
}
