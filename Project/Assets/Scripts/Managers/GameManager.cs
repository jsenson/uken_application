using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public static event System.Action onGameComplete;

    [SerializeField] private GridController _gridController = null;

    protected override bool Awake() {
        if(base.Awake()) return false;

        GameSettings.level = 1;
        GameSettings.score = 0;
        return true;
    }

    void Start() {
        // Start level one after the grid has initialized in Awake
        TimerBar.Instance.Reset();
        StartGame();
    }

    public void LoadNextLevel() {
        if(GameSettings.level == GameSettings.maxLevel) {
            if(onGameComplete != null) onGameComplete();
        } else {
            GameSettings.level++;
            StartGame();
        }
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
}
