using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreLabel : MonoBehaviour {
    private TextMeshProUGUI _label;

    void Start() {
        _label = GetComponent<TextMeshProUGUI>();
        UpdateLabel(GameSettings.score);
    }

    void OnEnable() {
        GameSettings.onScoreChanged += UpdateLabel;
    }

    void OnDisable() {
        GameSettings.onScoreChanged -= UpdateLabel;
    }

    void UpdateLabel(int score) {
        _label.text = string.Format("Score: {0:N0}", score);
    }
}
