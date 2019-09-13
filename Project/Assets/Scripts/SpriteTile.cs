using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteTile : MonoBehaviour, IPointerClickHandler {
    public static event System.Action<SpriteTile> onTileClicked;
    public static event System.Action<SpriteTile, SpriteTile> onTilesMatched;
    
    [SerializeField] private SpriteTileInfo _info = null;

    public SpriteTileInfo tileInfo;

    public bool highlighted {
        get { return _highlighted; }
        set { SetHighlighted(value); }
    }

    private bool _highlighted = false;
    private SpriteRenderer _renderer;
    private GridNode _currentNode = null;

    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        SetSpriteTileInfo(_info);
    }

    public void SetSpriteTileInfo(SpriteTileInfo info) {
        _info = info;
        _renderer.sprite = _info != null ? _info.sprite : null;
        SetHighlighted(highlighted);
    }

    public void SetGridNode(GridNode node, TileGrid grid) {
        if(_currentNode != null) {
            _currentNode.tile = null;
            _currentNode.pathfindingWeight = 1;
        }

        _currentNode = node;

        if(_currentNode != null) {
            _currentNode.tile = this;
            _currentNode.pathfindingWeight = -1;
            transform.position = grid.ConvertToWorldPosition(_currentNode.GetPosition());
        }
    }

    public void SetHighlighted(bool highlighted) {
        if(tileInfo != null) {
            _highlighted = highlighted;
            _renderer.color = _highlighted ? tileInfo.highlightColor : tileInfo.defaultColor;
        }
    }

    public bool Matches(SpriteTile other) {
        return other.tileInfo.tileName.Equals(tileInfo.tileName);
    }

    public static void MatchTiles(SpriteTile tile1, SpriteTile tile2) {
        if(onTilesMatched != null) onTilesMatched(tile1, tile2);

        Destroy(tile1.gameObject);
        Destroy(tile2.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Tile " + _info.tileName + " clicked!");
        if(onTileClicked != null) onTileClicked(this);
    }
}
