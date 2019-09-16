using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Listens to animation events and tracks active animations for the event when the player resets the game while animations are still active.
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

    // Clear any active animations any time the game is reset
    void OnReset() {
        for(int i = 0, count = _activeAnimations.Count; i < count; i++) {
            _activeAnimations[i].StopAndDestroy();
        }
        _activeAnimations.Clear();
    }
}
