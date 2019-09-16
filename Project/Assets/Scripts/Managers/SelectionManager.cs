using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles responding to selection events, the selection box, and kicking off pathfinding checks when matching tiles are selected.
public class SelectionManager : MonoBehaviour {
    [SerializeField] private GameObject _selectionBox = null;

    private SpriteTile _currentTile = null;
    private SpriteRenderer _selectionBoxRenderer = null;

    void Start() {
        if(_selectionBox != null) {
            _selectionBox.SetActive(false);
            _selectionBoxRenderer = _selectionBox.GetComponent<SpriteRenderer>();
        }
    }

    void OnEnable() {
        SpriteTile.onTileClicked += OnTileClicked;
        GameManager.onGameReset += OnReset;
    }

    void OnDisable() {
        SpriteTile.onTileClicked -= OnTileClicked;
        GameManager.onGameReset -= OnReset;
    }

    void OnTileClicked(SpriteTile sender) {
        if(_currentTile != null) {
            if(sender == _currentTile) {
                // Deselect if we click the same tile again
                SetCurrentTile(null);
            } else if(sender.Matches(_currentTile)) {
                int turns = int.MaxValue;

                // Temporarily set the target node to be walkable before pathfinding since AStar needs to be able to walk onto the node to find it.
                float oldWeight = sender.gridNode.pathfindingWeight;
                sender.gridNode.pathfindingWeight = 1;
                IAStarNode[] path = AStar.CalculatePath(_currentTile.gridNode, sender.gridNode, out turns);
                sender.gridNode.pathfindingWeight = oldWeight;

                if(turns <= GameSettings.maxPathfindingTurns) {
                    // If it's a valid match spawn a MatchAnimation and play it
                    _currentTile.gameObject.AddComponent<MatchAnimation>().Play(_currentTile, sender, path);
                    SetCurrentTile(null);
                } else {
                    // If the path has too many turns simply select the tile instead
                    SetCurrentTile(sender);
                }
            } else {
                // Tile's don't match, select the new one
                SetCurrentTile(sender);
            }
        } else {
            // We don't have a selected tile yet, select the new one
            SetCurrentTile(sender);
        }
    }

    void SetCurrentTile(SpriteTile tile) {
        // Deselect our current tile
        if(_currentTile != null) _currentTile.SetHighlighted(false);

        if(tile != null) {
            tile.SetHighlighted(true);
            
            if(_selectionBox != null) {
                // Activate the selection box
                _selectionBox.SetActive(true);

                // Set the selection slightly in front of the tile
                Vector3 pos = tile.transform.position;
                pos.z -= 0.01f;
                _selectionBox.transform.position = pos;

                // Set the selection to the same size as the tiles grid node
                _selectionBox.transform.localScale = tile.gridNode.size / _selectionBoxRenderer.sprite.bounds.size;
            }
        } else {
            // We're not selecting anything. Disable the selection box
            if(_selectionBox != null) _selectionBox.SetActive(false);
        }

        _currentTile = tile;
    }

    // Make sure the selection is cleared when the game resets
    void OnReset() {
        SetCurrentTile(null);
    }
}
