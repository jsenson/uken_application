using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public static event System.Action<int> onLevelComplete;
    public static event System.Action onGameComplete;

    [SerializeField] private GridController _gridController = null;

    protected override bool Awake() {
        if(base.Awake()) return false;

        GameSettings.level = 1;
        GameSettings.score = 0;
        return true;
    }

    void OnEnable() {
        GridController.onAllTilesCleared += OnGridCleared;
    }

    void OnDisable() {
        GridController.onAllTilesCleared -= OnGridCleared;
    }

    public void LoadNextLevel() {
        GameSettings.level++;
        StartGame();
    }

    public void ResetGame() {
        GameSettings.level = 1;
        GameSettings.score = 0;
        StartGame();
    }

    public void QuitGame() {
        Application.Quit();
    }

    void StartGame() {
        _gridController.InitializeGrid();
        TimerBar.Instance.Play();
    }

    void OnGridCleared() {
        if(onLevelComplete != null) onLevelComplete(GameSettings.level);
        if(GameSettings.level == GameSettings.maxLevel && onGameComplete != null) onGameComplete();
    }
}
