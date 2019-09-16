using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Handles displaying the sprite that is attached to tiles in the TileGrid.
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

    // Update the tile to display the given SpriteTileInfo
    public void SetSpriteTileInfo(SpriteTileInfo info) {
        _info = info;
        _renderer.sprite = _info != null ? _info.sprite : null;
        SetHighlighted(highlighted);
    }

    // Set the GridNode that this tile is currently attached to.  Will automatically remove itself from its current node and replace any tile that mught exist on the target.
    public void SetGridNode(GridNode node) {
        // Detatch ourselves from our current node
        if(_currentNode != null) {
            _currentNode.tile = null;
            _currentNode.pathfindingWeight = 1;
        }

        _currentNode = node;

        if(_currentNode != null) {
            // Make sure we tell the nodes tile that we're replacing it
            if(_currentNode.tile != null) _currentNode.tile.SetGridNode(null);

            // Attach to the node and set it to not be walkable during pathfinding
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

    // Set an optional highlight tint based on the setting in the SpriteTileInfo.
    public void SetHighlighted(bool highlighted) {
        if(_info != null) {
            _highlighted = highlighted;
            _renderer.color = _highlighted ? _info.highlightColor : _info.defaultColor;
        }
    }

    // Enable or disable interaction with the tile
    public void SetColliderEnabled(bool enabled) {
        _collider.enabled = enabled;
    }

    // Returns true if the given tile is a match with us.  Tiles are compared based on the SpriteTileInfo.tileName property.
    public bool Matches(SpriteTile other) {
        return other._info.tileName.Equals(_info.tileName);
    }

    // Triggers two tiles to be matched and fires the onTileMatched event.
    public static void MatchTiles(SpriteTile tile1, SpriteTile tile2) {
        // Make sure both tiles are cleared from the grid
        if(tile1._currentNode != null) tile1._currentNode.Clear();
        if(tile2._currentNode != null) tile2._currentNode.Clear();

        // Add score for each tile
        GameSettings.score += tile1._info.pointValue;
        GameSettings.score += tile2._info.pointValue;

        // Fire the match event.  The GridController handles returning the tiles to their object pool.
        if(onTilesMatched != null) onTilesMatched(tile1, tile2);
    }

    // Forward click events to anyone interested in them
    public void OnPointerClick(PointerEventData eventData) {
        if(onTileClicked != null) onTileClicked(this);
    }
}
