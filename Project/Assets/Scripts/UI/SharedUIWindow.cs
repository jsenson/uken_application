using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// A shared window to handle Pause, Level Complete, Game Over, and Game Complete since they all share almost the same exact layout with very little variation.
public class SharedUIWindow : Window {
    [SerializeField] private bool _pauseWhileOpen = true;
    [SerializeField] private TextMeshProUGUI _message = null;
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Button _nextButton = null;
    [SerializeField] private Button _restartButton = null;

    #pragma warning disable 649
    // Disable the 'unused variable' warning since we know it's used in standalone builds
    [SerializeField] private Button _quitButton = null;
    #pragma warning restore 649

    private const string kPauseMessage = "PAUSED";
    private const string kLevelCompleteMessage = "LEVEL COMPLETE\nSCORE = {0:N0}";
    private const string kGameCompleteMessage = "GAME COMPLETED!\nSCORE = {0:N0}";
    private const string kGameOverMessage = "GAME OVER\nSCORE = {0:N0}";

#if !UNITY_STANDALONE
    protected override void Awake() {
        base.Awake();

        // Destroy the quit button outside of standalone builds since it makes no sense on mobile/web
        Destroy(_quitButton.gameObject);
        _quitButton = null;
    }
#endif

    void OnEnable() {
        TimerBar.onTimeExpired += OpenGameOver;
        GameManager.onLevelComplete += OpenLevelComplete;
        GameManager.onGameComplete += OpenGameComplete;
    }

    void OnDisable() {
        TimerBar.onTimeExpired -= OpenGameOver;
        GameManager.onLevelComplete -= OpenLevelComplete;
        GameManager.onGameComplete -= OpenGameComplete;
    }

    // Pause the game when open
    protected override void OnOpening() {
        base.OnOpening();
        if(_pauseWhileOpen) Time.timeScale = 0;
    }

    // Unpause when closed
    protected override void OnClosed() {
        if(_pauseWhileOpen) Time.timeScale = 1;
        base.OnClosed();
    }

    // Opens the Pause menu
    public void OpenPause() {
        SetButtonsActive(false, true, true, true);
        _message.text = kPauseMessage;
        Open();
    }

    // Opens the Level Complete menu
    public void OpenLevelComplete(int level = 1) {
        if(level == GameSettings.maxLevel) return;  // Ignore this message on the last level since onGameComplete will also fire.
        SetButtonsActive(true, false, false, false);
        _message.text = string.Format(kLevelCompleteMessage, GameSettings.score);
        Open();
    }

    // Opens the Game Over menu
    public void OpenGameOver() {
        SetButtonsActive(false, true, false, true);
        _message.text = string.Format(kGameOverMessage, GameSettings.score);
        Open();
    }

    // Opens the Game Compelte menu
    public void OpenGameComplete() {
        SetButtonsActive(false, true, false, true);
        _message.text = string.Format(kGameCompleteMessage, GameSettings.score);
        Open();
    }

    // Helper to set all buttons on or off as needed
    void SetButtonsActive(bool next, bool reset, bool close, bool quit) {
        _nextButton.gameObject.SetActive(next);
        _restartButton.gameObject.SetActive(reset);
        _closeButton.gameObject.SetActive(close);
#if UNITY_STANDALONE
        _quitButton.gameObject.SetActive(quit);
#endif
    }

    public void NextLevel() {
        GameManager.Instance.LoadNextLevel();
        Close();
    }

    public void QuitGame() {
        GameManager.Instance.QuitGame();
    }

    public void RestartGame() {
        GameManager.Instance.ResetGame();
        Close();
    }
}
