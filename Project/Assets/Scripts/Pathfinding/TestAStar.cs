using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.ObjectPooling;

public class TestAStar : MonoBehaviour {
    public TileGrid _grid;
    public GameObject _block;
    public LineRenderer _linePrefab;
    public float blockChance = 0.1f;
    public Vector2Int startPos = Vector2Int.zero;
    public Vector2Int endPos = Vector2Int.one;

    private GameObjectPool<LineRenderer> _linePool;

    void Start() {
        GenerateRandomBlocks();
        _linePool = new GameObjectPool<LineRenderer>(_linePrefab, transform, 3, 2);
    }

    [ContextMenu("Generate Blocks")]
    public void GenerateRandomBlocks() {
        _grid.Clear();
        for(int x = 0; x < _grid.columns; x++) {
            for(int y = 0; y < _grid.rows; y++) {
                if(_grid[x, y].tile == null && Random.Range(0f, 1f) < blockChance) {
                    GameObject block = GameObject.Instantiate(_block, (Vector3)_grid.ConvertToWorldPosition(x, y), Quaternion.identity);
                    _grid[x,y].pathfindingWeight = -1;
                    // _grid[x,y].tile = block;
                }
            }
        }
    }

    [ContextMenu("Run AStar Test")]
    public void RunTest() {
        int turns;
        IAStarNode[] path = AStar.CalculatePath(_grid[startPos.x, startPos.y], _grid[endPos.x, endPos.y], out turns);
        if(path.Length > 0) {
            StartCoroutine(ShowLine(path));
        } else {
            Debug.Log("Did not find a valid path.");
        }
    }


    IEnumerator ShowLine(IAStarNode[] path) {
        LineRenderer line = _linePool.Pop();
        line.positionCount = path.Length;

        for(int i = 0; i < path.Length; i++) {
            line.SetPosition(i, _grid.ConvertToWorldPosition(path[i].coordinates));
        }

        yield return new WaitForSeconds(5);
        _linePool.Push(line);
    }
}
