using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Not much to say here.
public class TitleScreen : Window {
    public void StartGame() {
        GameManager.Instance.ResetGame();
        Close();
    }
}
