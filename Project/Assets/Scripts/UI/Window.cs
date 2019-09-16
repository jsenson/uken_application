using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Window class that opens and closes based on animations in an Animator component.
// Animations must have an "Open" and "Close" state on the base layer and an 'open' boolean parameter
[RequireComponent(typeof(Animator))]
public class Window : MonoBehaviour {
    public static event System.Action<Window> onWindowOpening;
    public static event System.Action<Window> onWindowClosed;

    [SerializeField] private bool _startOpen = false;

    public bool isOpen { get; private set; }

    // The name of the boolean parameter that controls transitions in the Animator
    private const string kOpenFlag = "open";

    private Animator _animator = null;
    private int _openHash;
    private int _closeHash;
    private Coroutine _closeRoutine = null;

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
        
        // Store the hashes for the two main animation state names
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

    // Wait for the end of the given animation to complete before running the onComplete callback.
    // May not work with looping animations. Not tested.
    private IEnumerator WaitForAnimation(int layer, int pathHash, System.Action onComplete) {
        bool done = false;

        while(!done) {
            yield return null;
            AnimatorStateInfo currState = _animator.GetCurrentAnimatorStateInfo(layer);
            AnimatorStateInfo nextState = _animator.GetNextAnimatorStateInfo(layer);

            // Need to check both current and next animation state since we could be in a transition to the one we're waiting for.
            // If the next state isn't what we're looking for and we're at the end of the current one we must be finished waiting.
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
