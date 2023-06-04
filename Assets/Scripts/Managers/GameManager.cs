using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    public int maxLives;
    public int lives;

    public int level;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.GridGeneration);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.ResetLevel:
                UnitManager.Instance.DestroyUnits();
                GridManager.Instance.DestroyGrid();
                UpdateGameState(GameState.GridGeneration);
                break;
            case GameState.GridGeneration:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnEnemies:
                int numberOfEnemies = UnityEngine.Random.Range(1 + level / 3, level / 2);
                UnitManager.Instance.SpawnEnemies(numberOfEnemies);
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                UnitManager.Instance.EnemyActions();
                break;
            case GameState.SpawnReward:
                GridManager.Instance.AddReward();
                break;
            case GameState.RewardSelect:
                UIManager.Instance.RewardSelect();
                break;
            case GameState.Victory:
                IncreaseLevel();
                GenerateLevel();
                break;
            case GameState.Lose:
                UnitManager.Instance.DestroyUnits();
                GridManager.Instance.DestroyGrid();
                UIManager.Instance.GameOver();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    protected void IncreaseLevel()
    {
        level++;
    }

    protected void IncreaseLife()
    {
        if (lives < maxLives)
        {
            lives++;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GenerateLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.ResetLevel);
    }

    public void RegenerateLevel()
    {
        lives--;
        GenerateLevel();
    }

    public void RewardLife()
    {
        if (lives == maxLives)
        {
            maxLives++;
        }
        else
        {
            lives++;
        }
        UIManager.Instance.HideRewardSelect();
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }

    public void RewardWeapon()
    {

        UIManager.Instance.HideRewardSelect();
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }
}

public enum GameState
{
    ResetLevel,
    GridGeneration,
    SpawnHeroes,
    SpawnEnemies,
    PlayerTurn,
    EnemyTurn,
    SpawnReward,
    RewardSelect,
    Victory,
    Lose
}