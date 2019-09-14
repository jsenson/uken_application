using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {
    [SerializeField] private int _maxPathTurns = 2; // TODO: Put this somewhere more appropriate.  A GameSettings class that stores this + level + score etc.
    private SpriteTile _currentTile = null;

    void OnEnable() {
        SpriteTile.onTileClicked += OnTileClicked;
    }

    void OnDisable() {
        SpriteTile.onTileClicked -= OnTileClicked;
    }

    void OnTileClicked(SpriteTile sender) {
        if(_currentTile != null) {
            if(sender == _currentTile) {
                sender.SetHighlighted(false);
                _currentTile = null;
            } else if(sender.Matches(_currentTile)) {
                int turns = int.MaxValue;

                // Temporarily set the target node to be walkable before pathfinding since AStar needs to be able to walk onto the node to find it.
                float oldWeight = sender.gridNode.pathfindingWeight;
                sender.gridNode.pathfindingWeight = 1;
                IAStarNode[] path = AStar.CalculatePath(_currentTile.gridNode, sender.gridNode, out turns);
                sender.gridNode.pathfindingWeight = oldWeight;
                AStar.LogPath(path, turns);

                if(turns <= _maxPathTurns) {
                    SpriteTile.MatchTiles(_currentTile, sender);
                    SetCurrentTile(null);
                } else {
                    SetCurrentTile(sender);
                }
            } else {
                SetCurrentTile(sender);
            }
        } else {
            sender.SetHighlighted(true);
            _currentTile = sender;
        }
    }

    void SetCurrentTile(SpriteTile tile) {
        if(_currentTile != null) _currentTile.SetHighlighted(false);
        if(tile != null) tile.SetHighlighted(true);
        _currentTile = tile;
    }
}
