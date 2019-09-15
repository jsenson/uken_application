using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : Window {
    #pragma warning disable 649
    // Disable the 'unused variable' warning since we know it's used in standalone builds
    [SerializeField] private Button _quitButton;
    #pragma warning restore 649

#if !UNITY_STANDALONE
    protected override void Awake() {
        base.Awake();

        // Destroy the quit button outside of standalone builds since it makes no sense on mobile/web
        Destroy(_quitButton.gameObject);
    }
#endif

    protected override void OnOpening() {
        base.OnOpening();
        Time.timeScale = 0;
    }

    protected override void OnClosed() {
        Time.timeScale = 1;
        base.OnClosed();
    }

    public void QuitGame() {
        GameManager.Instance.QuitGame();
    }
}
