using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.Singletons;
using ToN.ObjectPooling;

// Singleton accessor for an object pool of Line Renderers to use when drawing animated paths.
public class LinePoolManager : MonoBehaviourSingleton<LinePoolManager> {
    [SerializeField] private LineRenderer _linePrefab = null;
    
    private GameObjectPool<LineRenderer> _linePool;

    protected override bool Awake() {
        if(!base.Awake()) return false;
        _linePool = new GameObjectPool<LineRenderer>(_linePrefab, transform, 3, 2);
        return true;
    }

    public LineRenderer PopLine() {
        return _linePool.Pop();
    }

    public void PushLine(LineRenderer line) {
        _linePool.Push(line);
    }
}
