using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : Window {
    public void StartGame() {
        GameManager.Instance.ResetGame();
        Close();
    }
}
