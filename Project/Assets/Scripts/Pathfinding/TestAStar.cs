using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour {
    public TileGrid _grid;
    public GameObject _block;
    public float blockChance = 0.1f;
    public Vector2Int startPos = Vector2Int.zero;
    public Vector2Int endPos = Vector2Int.one;

    void Start() {
        GenerateRandomBlocks();
    }

    [ContextMenu("Generate Blocks")]
    public void GenerateRandomBlocks() {
        _grid.Clear();
        for(int x = 0; x < _grid.columns; x++) {
            for(int y = 0; y < _grid.rows; y++) {
                if(_grid[x, y].tile == null && Random.Range(0f, 1f) < blockChance) {
                    GameObject block = GameObject.Instantiate(_block, (Vector3)_grid.ConvertToWorldPosition(x, y), Quaternion.identity);
                    _grid[x,y].pathfindingWeight = -1;
                    _grid[x,y].tile = block;
                }
            }
        }
    }

    [ContextMenu("Run AStar Test")]
    public void RunTest() {
        int turns;
        IAStarNode[] path = AStar.CalculatePath(_grid[startPos.x, startPos.y], _grid[endPos.x, endPos.y], out turns);
        if(path.Length > 0) {
            Debug.Log("Start path:");
            for(int i = 0; i < path.Length; i++) {
                Debug.LogFormat("({0:N}, {1:N})", path[i].GetPosition().x, path[i].GetPosition().y);
            }
            Debug.LogFormat("End - {0} turns", turns);
        } else {
            Debug.Log("Did not find a valid path.");
        }
    }
}
