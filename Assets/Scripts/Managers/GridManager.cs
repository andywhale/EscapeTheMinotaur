using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile floorTile, waterTile;

    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject goalInstance;

    [SerializeField] private GameObject reward;
    [SerializeField] private GameObject rewardInstance;

    [SerializeField] private Transform camera;

    [SerializeField] private Transform grid;

    [SerializeField] private Pathfinding path;

    private Dictionary<Vector2, Tile> tiles;

    private Tile goalTile;

    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<Vector2, Tile> GetTiles()
    {
        return tiles;
    }

    public void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tilePrefab = floorTile;
                if (Random.Range(0, 10) < 3)
                {
                    tilePrefab = waterTile;
                }
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, grid);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        foreach (KeyValuePair<Vector2, Tile> kvp in tiles)
        {
            Tile tile = kvp.Value;
            tile.SetNeighbours(tiles);
        }

        path.SetGrid(tiles);

        goalTile = tiles.Where(t => t.Key.y > (height / 4 * 3) && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
        goalTile.isGoal = true;
        goalInstance = Instantiate(goal, goalTile.transform.position, Quaternion.identity);

        camera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);

        UnitManager.Instance.SpawnHero();
        var hero = UnitManager.Instance.GetHero();

        List<Tile> route = path.FindPath(hero.OccupiedTile.transform, goalTile.transform);
        if (route == null)
        {
            GameManager.Instance.UpdateGameState(GameState.ResetLevel);
            return;
        }

        GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);
    }

    public void DestroyGrid()
    {
        Destroy(goalInstance.gameObject);
        Destroy(rewardInstance.gameObject);
        foreach (KeyValuePair<Vector2, Tile> kvp in tiles)
        {
            Destroy(kvp.Value.gameObject);
        }
    }

    public Tile GetTileAtPosition(Vector2 position)
    {
        if (tiles.TryGetValue(position, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetHeroSpawnTile()
    {
        return tiles.Where(t => t.Key.y < height / 3 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return tiles.Where(t => t.Key.y > height / 3 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public void AddReward()
    {
        Tile rewardTile = null;
        List<Tile> rewardRoute = null;

        while (rewardRoute == null && rewardTile == null) {
            rewardTile = tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
            rewardRoute = path.FindPath(UnitManager.Instance.GetHero().OccupiedTile.transform, rewardTile.transform);
        }

        rewardTile.isReward = true;
        rewardInstance = Instantiate(reward, rewardTile.transform.position, Quaternion.identity);
    }

    public Tile FindNextMove(Tile originTile, Tile targetTile)
    {
        List<Tile> route = path.FindPath(originTile.transform, targetTile.transform);
        if (route != null)
        {
            return route[0];
        }
        return null;
    }

    public void HighlightRoute(Tile originTile, Tile targetTile)
    {
        List<Tile> route = path.FindPath(originTile.transform, targetTile.transform);
        route.ToList().ForEach(tile => tile.Highlight());
    }

    public void OpenChest()
    {
        rewardInstance.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(renderer => renderer.enabled = true);
    }

    public void CloseChest()
    {
        rewardInstance.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(renderer => renderer.enabled = false);
    }
}
