using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> units;

    private List<BaseEnemy> enemies;

    private BaseHero hero;

    private void Awake()
    {
        Instance = this;

        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();

    }

    public BaseHero GetHero()
    {
        return hero;
    }

    public void SpawnHero()
    {
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        hero = Instantiate(randomPrefab);
        var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();
        randomSpawnTile.SetUnit(hero);
    }

    public void SpawnEnemies(int enemyCount = 1)
    {
        enemies = new List<BaseEnemy>();
        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var enemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            randomSpawnTile.SetUnit(enemy);
            enemies.Add(enemy);
        }

        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
    }

    public void KillEnemy(BaseEnemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.gameObject);
        if (enemies.Count <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.SpawnReward);
        }
    }

    public void EnemyActions()
    {
        foreach (var enemy in enemies)
        {
            if (enemy)
            {
                enemy.CarryOutAction();
            }
        }

        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);

    }

    public void DestroyUnits()
    {
        Destroy(hero.gameObject);
        if (enemies == null) return;
        foreach (var enemy in enemies)
        {
            if (enemy)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public bool PlayerCarryOutAction(Tile tile)
    {
        return hero.CarryOutAction(tile);
    }

    public void HighlightPlayerRoute(Tile tile)
    {
        GridManager.Instance.HighlightRoute(tile, hero.OccupiedTile);
    }

}
