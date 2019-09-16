using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToN.Singletons;

// Handles display and tracking of the level timer
public class TimerBar : MonoBehaviourSingleton<TimerBar> {
    public static event System.Action onTimeExpired;

    [SerializeField] private Image _timerBar = null;
    [SerializeField, Range(0f, 1f)] private float _warningThreshold = 0.1f;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _warningColor = Color.red;

    public float time { get; private set; }
    public float percent { get { return time / _maxTime; } }

    private float _maxTime = 200;
    private bool _active = false;
    private float _lastPercent = float.MaxValue;
    private Coroutine _warningRoutine = null;

    protected override bool Awake() {
        if(!base.Awake()) return false;
        UpdateMaxTime(GameSettings.level);
        return true;
    }

    void OnEnable() {
        GameSettings.onLevelChanged += UpdateMaxTime;
        GridController.onAllTilesCleared += Pause;
        SpriteTile.onTilesMatched += OnMatch;
    }

    void OnDisable() {
        GameSettings.onLevelChanged -= UpdateMaxTime;
        GridController.onAllTilesCleared -= Pause;
        SpriteTile.onTilesMatched -= OnMatch;
    }

    public void Play() {
        _active = true;
    }

    public void Pause() {
        _active = false;
    }

    void Update() {
        if(_active) {
            time -= Time.deltaTime;

            _timerBar.fillAmount = percent;

            // Check for activating or deactivating the warning animation
            if(_lastPercent > _warningThreshold && _timerBar.fillAmount <= _warningThreshold) SetWarningActive(true);
            else if(_lastPercent <= _warningThreshold && _timerBar.fillAmount > _warningThreshold) SetWarningActive(false);

            // Trigger the expired event when we're out of time
            if(time <= 0) {
                Pause();
                if(onTimeExpired != null) onTimeExpired();
            }

            // Track the previous time percentage for checking the warning threshold
            _lastPercent = _timerBar.fillAmount;
        }
    }

    // Enable of disable the warning animation coroutine
    void SetWarningActive(bool active) {
        if(active && _warningRoutine == null) {
            _warningRoutine = StartCoroutine(AnimateWarning(0.5f));
        } else if(!active && _warningRoutine != null) {
            StopCoroutine(_warningRoutine);
            _warningRoutine = null;
            _timerBar.color = _defaultColor;
        }
    }

    // Set the maximum time for the timer.  This will reset the timer to full.
    void UpdateMaxTime(int level) {
        _maxTime = GameSettings.GetLevelSettings(level).timeLimit;
        Reset();
    }

    // Resets the timer state
    public void Reset() {
        _active = false;
        time = _maxTime;
        _lastPercent = 1;
        SetWarningActive(false);
    }

    // Add bonus time whenever there's a match
    void OnMatch(SpriteTile t1, SpriteTile t2) {
        time = Mathf.Clamp(time + GameSettings.timeBonusPerMatch, 0, _maxTime);
    }

    // Animates a colour pulse on the time at the given frequency
    IEnumerator AnimateWarning(float frequency) {
        float timer;
        float inverse = 1f / frequency;;
        bool forward = false;
        
        while(true) {
            timer = 0;
            forward = !forward;
        
            while(timer <= frequency) {
                timer += Time.deltaTime;
                // Lerp the color back and forth between the two
                _timerBar.color = Color.Lerp(_defaultColor, _warningColor, forward ? timer * inverse : 1 - timer * inverse);
                yield return null;
            }
        }
    }
}
