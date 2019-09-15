using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Window : MonoBehaviour {
    public static event System.Action<Window> onWindowOpening;
    public static event System.Action<Window> onWindowClosed;

    [SerializeField] private bool _startOpen = false;

    public bool isOpen { get; private set; }

    private const string kOpenFlag = "open";

    private Animator _animator = null;
    private int _openHash;
    private int _closeHash;
    private Coroutine _closeRoutine = null;

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
        
        _openHash = Animator.StringToHash("Base Layer.Open");
        _closeHash = Animator.StringToHash("Base Layer.Close");

        // Start at the end of the animation so we're already fully closed/open
        _animator.SetBool(kOpenFlag, _startOpen);
        _animator.Play(_startOpen ? _openHash : _closeHash, 0, 1);
        isOpen = _startOpen;
    }

    [ContextMenu("Open")]
    public virtual void Open() {
        if(_closeRoutine != null) {
            StopCoroutine(_closeRoutine);
            _closeRoutine = null;
            OnClosed();
        }

        _animator.SetBool(kOpenFlag, true);
        OnOpening();
    }

    [ContextMenu("Close")]
    public virtual void Close() {
        _animator.SetBool(kOpenFlag, false);
        _closeRoutine = StartCoroutine(WaitForAnimation(0, _closeHash, OnClosed));
    }

    private IEnumerator WaitForAnimation(int layer, int pathHash, System.Action onComplete) {
        bool done = false;

        while(!done) {
            yield return null;
            AnimatorStateInfo currState = _animator.GetCurrentAnimatorStateInfo(layer);
            AnimatorStateInfo nextState = _animator.GetNextAnimatorStateInfo(layer);
            done = nextState.fullPathHash != pathHash && currState.normalizedTime >= 1;
        }

        _closeRoutine = null;
        if(onComplete != null) onComplete();
    }

    protected virtual void OnOpening() {
        isOpen = true;
        if(onWindowOpening != null) onWindowOpening(this);
    }

    protected virtual void OnClosed() {
        isOpen = false;
        if(onWindowClosed != null) onWindowClosed(this);
    }
}
