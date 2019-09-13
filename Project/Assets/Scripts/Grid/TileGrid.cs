using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour {
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _columns = 10;
    [SerializeField] private Vector2 cellSize = Vector2.one;

    public int rows { get { return _rows; } }
    public int columns { get { return _columns; } }

    public GridNode this[int x, int y] {
        get { return _grid != null ? _grid[x, y] : null; }
    }

    private GridNode[,] _grid;

    void Awake() {
        ResizeGrid(columns, rows);
    }

    public void ResizeGrid(int columnCount, int rowCount) {
        if(_grid != null) Clear();
        _grid = new GridNode[columnCount, rowCount];

        for(int x = 0; x < columnCount; x++) {
            for(int y = 0; y < rowCount; y++) {
                _grid[x, y] = new GridNode(new Vector2(x, y));
                
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

    public void Clear() {
        for(int x = 0; x < _grid.GetLength(0); x++) {
            for(int y = 0; y < _grid.GetLength(1); y++) {
                _grid[x, y].Clear();
            }
        }
    }

    public Vector2 ConvertToWorldPosition(int column, int row) {
        Vector2 bottomLeft = new Vector2(transform.position.x - (columns - 1) * cellSize.x * 0.5f, transform.position.y - (rows - 1) * cellSize.y * 0.5f);
        return new Vector2(bottomLeft.x + column * cellSize.x, bottomLeft.y + row * cellSize.y);
    }

#if UNITY_EDITOR
    void OnValidate() {
        _rows = Mathf.Clamp(rows, 2, 9999);
        _columns = Mathf.Clamp(columns, 2, 9999);
        if(_grid == null || _grid.GetLength(0) != columns || _grid.GetLength(1) != rows) ResizeGrid(columns, rows);
    }

    void OnGUI() {
        Vector2 bottomLeft = new Vector2(transform.position.x - columns * cellSize.x * 0.5f, transform.position.y - rows * cellSize.y * 0.5f);
        Vector2 topRight = new Vector2(bottomLeft.x + columns * cellSize.x, bottomLeft.y + rows * cellSize.y);
        
        for(int x = 0; x <= columns; x++) {
            float currentX = bottomLeft.x + cellSize.x * x;
            Debug.DrawLine(new Vector2(currentX, bottomLeft.y), new Vector2(currentX, topRight.y));
        }

        for(int y = 0; y <= rows; y++) {
            float currentY = bottomLeft.y + cellSize.y * y;
            Debug.DrawLine(new Vector2(bottomLeft.x, currentY), new Vector2(topRight.x, currentY));
        }
    }
#endif
}
