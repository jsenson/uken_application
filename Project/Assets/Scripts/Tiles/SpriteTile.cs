﻿using System.Collections;
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
            _currentNode.tile = this;
            _currentNode.pathfindingWeight = -1;
            transform.position = _currentNode.worldPosition;
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

        // TODO: Add LineManager to spawn a path line
        // TODO: Animate the tiles moving together with a MatchAnimation class before calling onTileMatched so the GridController can add them back to the pool.
        // TODO: Move this to after the animation plays out.
        if(onTilesMatched != null) onTilesMatched(tile1, tile2);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(onTileClicked != null) onTileClicked(this);
    }
}