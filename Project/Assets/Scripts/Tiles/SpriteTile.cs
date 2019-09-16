using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class SpriteTile : MonoBehaviour, IPointerClickHandler {
    public static event System.Action<SpriteTile> onTileClicked;
    public static event System.Action<SpriteTile, SpriteTile> onTilesMatched;
    
    [SerializeField] private SpriteTileInfo _info = null;

    public bool highlighted {
        get { return _highlighted; }
        set { SetHighlighted(value); }
    }

    public GridNode gridNode { get { return _currentNode; } }
    public SpriteTileInfo tileInfo { get { return _info; } }

    private bool _highlighted = false;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;
    private GridNode _currentNode = null;

    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        SetSpriteTileInfo(_info);
    }

    public void SetSpriteTileInfo(SpriteTileInfo info) {
        _info = info;
        _renderer.sprite = _info != null ? _info.sprite : null;
        SetHighlighted(highlighted);
    }

    public void SetGridNode(GridNode node) {
        if(_currentNode != null) {
            _currentNode.tile = null;
            _currentNode.pathfindingWeight = 1;
        }

        _currentNode = node;

        if(_currentNode != null) {
            if(_currentNode.tile != null) _currentNode.tile.SetGridNode(null);
            _currentNode.tile = this;
            _currentNode.pathfindingWeight = -1;
            transform.position = _currentNode.worldPosition;
            
            if(_renderer.sprite != null) {
                // Scale slightly under the node size to leave a bit of padding
                Vector3 scale = _currentNode.size * 0.9f / _renderer.sprite.bounds.size;
                float maxScale = Mathf.Max(scale.x, scale.y);
                transform.localScale = Vector3.one * maxScale;
            }
        }
    }

    public void SetHighlighted(bool highlighted) {
        if(_info != null) {
            _highlighted = highlighted;
            _renderer.color = _highlighted ? _info.highlightColor : _info.defaultColor;
        }
    }

    public void SetColliderEnabled(bool enabled) {
        _collider.enabled = enabled;
    }

    public bool Matches(SpriteTile other) {
        return other._info.tileName.Equals(_info.tileName);
    }

    public static void MatchTiles(SpriteTile tile1, SpriteTile tile2) {
        if(tile1._currentNode != null) tile1._currentNode.Clear();
        if(tile2._currentNode != null) tile2._currentNode.Clear();

        GameSettings.score += tile1._info.pointValue;
        GameSettings.score += tile2._info.pointValue;

        if(onTilesMatched != null) onTilesMatched(tile1, tile2);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(onTileClicked != null) onTileClicked(this);
    }
}
