﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {
    [SerializeField] private GameObject _selectionBox = null;

    private SpriteTile _currentTile = null;

    void Start() {
        if(_selectionBox != null) {
            _selectionBox.transform.localPosition = new Vector3(0, 0, -0.01f);
            _selectionBox.SetActive(false);
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
                SetCurrentTile(null);
            } else if(sender.Matches(_currentTile)) {
                int turns = int.MaxValue;

                // Temporarily set the target node to be walkable before pathfinding since AStar needs to be able to walk onto the node to find it.
                float oldWeight = sender.gridNode.pathfindingWeight;
                sender.gridNode.pathfindingWeight = 1;
                IAStarNode[] path = AStar.CalculatePath(_currentTile.gridNode, sender.gridNode, out turns);
                sender.gridNode.pathfindingWeight = oldWeight;

                if(turns <= GameSettings.maxPathfindingTurns) {
                    _currentTile.gameObject.AddComponent<MatchAnimation>().Play(_currentTile, sender, path);
                    SetCurrentTile(null);
                } else {
                    SetCurrentTile(sender);
                }
            } else {
                SetCurrentTile(sender);
            }
        } else {
            SetCurrentTile(sender);
        }
    }

    void SetCurrentTile(SpriteTile tile) {
        if(_currentTile != null) _currentTile.SetHighlighted(false);
        if(tile != null) {
            tile.SetHighlighted(true);
            
            if(_selectionBox != null) {
                _selectionBox.SetActive(true);
                _selectionBox.transform.SetParent(tile.transform, false);
                _selectionBox.transform.localPosition = new Vector3(0, 0, -0.01f);  // Set the selection box slightly in front of the tile
            }
        } else {
            if(_selectionBox != null) _selectionBox.SetActive(false);
        }
        _currentTile = tile;
    }

    void OnReset() {
        SetCurrentTile(null);
    }
}
