using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    [SerializeField] private GridController _gridController = null;

    protected override bool Awake() {
        if(base.Awake()) return false;

        GameSettings.level = 1;
        GameSettings.score = 0;
        return true;
    }

    void Start() {
        // Start level one after the grid has initialized in Awake
        _gridController.InitializeGrid();
        TimerBar.Instance.Reset();
        TimerBar.Instance.Play();
    }

    public void LoadNextLevel() {
        if(GameSettings.level == GameSettings.maxLevel) {
            Debug.Log("Level 3 complete!");
        } else {
            GameSettings.level++;
            _gridController.InitializeGrid();
            TimerBar.Instance.Play();
        }
    }
}
