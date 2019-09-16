using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchAnimation : MonoBehaviour {
    public static event System.Action<MatchAnimation> onAnimationStart;
    public static event System.Action<MatchAnimation> onAnimationEnd;

    public SpriteTile startTile { get; private set; }
    public SpriteTile endTile { get; private set; }
    public bool playing { get; private set; }

    private LineRenderer _line;
    private Coroutine _animationRoutine = null;
    
    void OnDestroy() {
        if(_line != null && LinePoolManager.Instance != null) {
            LinePoolManager.Instance.PushLine(_line);
            _line = null;
        }
    }

    public void Play(SpriteTile start, SpriteTile end, IAStarNode[] path, float moveDuration = 0.4f) {
        if(!playing) {
            playing = true;

            startTile = start;
            endTile = end;
            Vector2[] points = GetKeyWorldPoints(path);
            _animationRoutine = StartCoroutine(AnimateTiles(points, moveDuration));
        }
    }

    // Stop and remove the animation destroying the active orphaned tile since we have no access to the pool that spawned it.
    // May cause the original tile pool to spawn more tiles in the future if needed.
    public void StopAndDestroy() {
        if(playing) {
            if(_animationRoutine != null) StopCoroutine(_animationRoutine);
            Destroy(startTile.gameObject);
            if(_line != null) LinePoolManager.Instance.PushLine(_line);
            Destroy(this);
        }
    }

    private IEnumerator AnimateTiles(Vector2[] points, float duration) {
        if(onAnimationStart != null) onAnimationStart(this);

        endTile.SetColliderEnabled(false);
        startTile.SetColliderEnabled(false);
        startTile.transform.position += new Vector3(0, 0, -0.2f);
        if(startTile.gridNode != null) startTile.gridNode.Clear();

        _line = LinePoolManager.Instance.PopLine();
        SetLinePositions(_line, points, startTile.transform.position.z + 0.1f);

        for(int i = 0; i < points.Length - 1; i++) {
            float timer = 0f;
            float durationInverse = 1f / duration;

            while(timer <= 1f) {
                yield return null;

                timer += Time.deltaTime * durationInverse;
                Vector3 newPos = (Vector3)Vector2.Lerp(points[i], points[i + 1], EaseSineInOut(timer));
                newPos.z = startTile.transform.position.z;
                startTile.transform.position = newPos;
            }
        }

        _animationRoutine = null;
        LinePoolManager.Instance.PushLine(_line);
        _line = null;

        if(onAnimationEnd != null) onAnimationEnd(this);

        Destroy(this);
        SpriteTile.MatchTiles(startTile, endTile);
    }

    private void SetLinePositions(LineRenderer line, Vector2[] points, float zPosition) {
        line.positionCount = points.Length;
        for(int i = 0; i < points.Length; i++) {
            line.SetPosition(i, new Vector3(points[i].x, points[i].y, zPosition));
        }
    }

    private Vector2[] GetKeyWorldPoints(IAStarNode[] path) {
        if(path.Length < 2) throw new System.ArgumentException("MatchAnimation called with an invalid path. IAStarNode[] must contain at least two elements.");

        List<Vector2> keys = new List<Vector2>();
        Vector2 previousDirection = path[1].coordinates - path[0].coordinates;

        keys.Add(((GridNode)path[0]).worldPosition);
        
        for(int i = 2; i < path.Length; i++) {
            Vector2 direction = path[i].coordinates - path[i - 1].coordinates;
            if(direction != previousDirection) keys.Add(((GridNode)path[i - 1]).worldPosition);
            previousDirection = direction;
        }

        keys.Add(((GridNode)path[path.Length - 1]).worldPosition);

        return keys.ToArray();
    }

    // NOTE: Normally I'd pull in an existing 3rd party easing or tweening plugin but figured I'd just write it out here to keep everything custom.  Should put this in it's own class.
    private float EaseSineInOut(float k) {
        return 0.5f * (1f - Mathf.Cos(Mathf.PI * k));
    }
}
