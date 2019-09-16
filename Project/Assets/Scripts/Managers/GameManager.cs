using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.Singletons;

// Simple controller to handle a few game-level events and provide Singleton access to resetting and progressing the game.
public class GameManager : MonoBehaviourSingleton<GameManager> {
    public static event System.Action<int> onLevelComplete;
    public static event System.Action onGameComplete;
    public static event System.Action onGameReset;

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

    // Increments the game level and reloads the grid, starting a new game.  If called on the final level this will simple restart the level instead since it is clamped.
    public void LoadNextLevel() {
        GameSettings.level++;
        StartGame();
    }

    // Fully resets the game to level 1 with a score of 0
    public void ResetGame() {
        GameSettings.level = 1;
        GameSettings.score = 0;
        if(onGameReset != null) onGameReset();
        StartGame();
    }

    // Quit the applicaiton.  Only applicable in Standalone builds and does nothing otherwise.
    public void QuitGame() {
        Application.Quit();
    }

    void StartGame() {
        _gridController.ResetGrid();
        TimerBar.Instance.Play();
    }

    // Fire level complete events whenever the grid is cleared.
    void OnGridCleared() {
        if(onLevelComplete != null) onLevelComplete(GameSettings.level);
        if(GameSettings.level == GameSettings.maxLevel && onGameComplete != null) onGameComplete();
    }
}
