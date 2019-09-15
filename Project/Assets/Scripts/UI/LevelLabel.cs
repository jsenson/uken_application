using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelLabel : MonoBehaviour {
    private TextMeshProUGUI _label;

    void Start() {
        _label = GetComponent<TextMeshProUGUI>();
        UpdateLabel(GameSettings.level);
    }

    void OnEnable() {
        GameSettings.onLevelChanged += UpdateLabel;
    }

    void OnDisable() {
        GameSettings.onLevelChanged -= UpdateLabel;
    }

    void UpdateLabel(int level) {
        _label.text = string.Format("Level: {0:N0}", level);
    }
}
