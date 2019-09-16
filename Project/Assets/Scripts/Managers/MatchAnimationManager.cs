using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchAnimationManager : MonoBehaviour {
    private List<MatchAnimation> _activeAnimations;

    void Awake() {
        _activeAnimations = new List<MatchAnimation>();
    }

    void OnEnable() {
        MatchAnimation.onAnimationStart += OnAnimationStart;
        MatchAnimation.onAnimationEnd += OnAnimationEnd;
        GameManager.onGameReset += OnReset;
    }

    void OnDisable() {
        MatchAnimation.onAnimationStart -= OnAnimationStart;
        MatchAnimation.onAnimationEnd -= OnAnimationEnd;
        GameManager.onGameReset -= OnReset;
    }

    void OnAnimationStart(MatchAnimation sender) {
        _activeAnimations.Add(sender);
    }

    void OnAnimationEnd(MatchAnimation sender) {
        _activeAnimations.Remove(sender);
    }

    void OnReset() {
        for(int i = 0, count = _activeAnimations.Count; i < count; i++) {
            _activeAnimations[i].StopAndDestroy();
        }
        _activeAnimations.Clear();
    }
}
