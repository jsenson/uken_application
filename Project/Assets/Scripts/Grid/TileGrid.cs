using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.ObjectPooling;

// A 2D grid representation comprosed of GridNode instances.
public class TileGrid : MonoBehaviour {
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _columns = 10;
    [SerializeField] private Vector2 _cellSize = Vector2.one;

    public int rows { get { return _rows; } }
    public int columns { get { return _columns; } }
    public int tileCount { get { return GetTileCount(); } }
    public Vector2 cellSize { get { return _cellSize; } }

    // Array accessor so this can be treated like its base 2d array.
    public GridNode this[int x, int y] {
        get { return _grid != null ? _grid[x, y] : null; }
    }

    private GridNode[,] _grid;

    void Awake() {
        ResizeGrid(columns, rows);
    }

    // Set the size of the grid in columns and rows.  Will lose all references to exisitng tiles so should not be called before handling them.
    public void ResizeGrid(int columnCount, int rowCount) {
        if(_grid != null) Clear();
        _columns = columnCount;
        _rows = rowCount;
        _grid = new GridNode[columnCount, rowCount];

        for(int x = 0; x < columnCount; x++) {
            for(int y = 0; y < rowCount; y++) {
                _grid[x, y] = new GridNode(new Vector2(x, y), this);
                
                // Hook up the neighbours to the left and below as we build the grid from the bottom-left.
                if(x > 0) {
                    _grid[x, y].AddNeighbour(_grid[x-1, y]);
                    _grid[x-1, y].AddNeighbour(_grid[x, y]);
                }

                if(y > 0) {
                    _grid[x, y].AddNeighbour(_grid[x, y-1]);
                    _grid[x, y-1].AddNeighbour(_grid[x, y]);
                }
            }
        }
    }

    // Clears all tiles that exist on the grid and returns them to the given object pool.  If a pool isn't provided the tiles will be orphaned but not destroyed.
    public void Clear(GameObjectPool<SpriteTile> tilePool = null) {
        for(int x = 0; x < _grid.GetLength(0); x++) {
            for(int y = 0; y < _grid.GetLength(1); y++) {
                GridNode n = _grid[x, y];
                if(tilePool != null && n.tile != null) tilePool.Push(n.tile);
                n.Clear();
            }
        }
    }

    public Vector2 ConvertToWorldPosition(Vector2 coordinates) {
        return ConvertToWorldPosition((int)coordinates.x, (int)coordinates.y);
    }

    // Convert the any x,y coordinates into a worldPosition based on the position of the grid.
    public Vector2 ConvertToWorldPosition(int column, int row) {
        Vector2 bottomLeft = new Vector2(transform.position.x - (columns - 1) * _cellSize.x * 0.5f, transform.position.y - (rows - 1) * _cellSize.y * 0.5f);
        return new Vector2(bottomLeft.x + column * _cellSize.x, bottomLeft.y + row * _cellSize.y);
    }

    // Get a count of all nodes in the grid with a SpriteTile attached to them.
    public int GetTileCount() {
        int count = 0;

        for(int x = 0; x < _grid.GetLength(0); x++) {
            for(int y = 0; y < _grid.GetLength(1); y++) {
                if(_grid[x,y].tile != null) count++;
            }
        }

        return count;
    }

#if UNITY_EDITOR
    void OnValidate() {
        _rows = Mathf.Clamp(rows, 2, 9999);
        _columns = Mathf.Clamp(columns, 2, 9999);
        if(_grid == null || _grid.GetLength(0) != columns || _grid.GetLength(1) != rows) ResizeGrid(columns, rows);
    }

    void OnGUI() {
        for(int x = 0; x < _grid.GetLength(0); x++) {
            for(int y = 0; y < _grid.GetLength(1); y++) {
                _grid[x,y].DrawDebug(_cellSize);
            }
        }
    }
#endif
}
