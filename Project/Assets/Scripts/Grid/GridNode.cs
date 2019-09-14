using System.Collections.Generic;
using UnityEngine;

public class GridNode : IAStarNode {
    public float pathfindingWeight { get; set; }

    public SpriteTile tile { get; set; }
    public Vector2 coordinates { get; private set; }
    public Vector2 worldPosition { get { return _grid != null ? _grid.ConvertToWorldPosition(coordinates) : coordinates; } }
    public Vector2 size { get { return _grid != null ? _grid.cellSize : Vector2.one; } }

    private List<IAStarNode> _neighbours;
    private TileGrid _grid;

    public GridNode(Vector2 coordinates, TileGrid parentGrid) {
        _neighbours = new List<IAStarNode>();
        _grid = parentGrid;
        this.coordinates = coordinates;
        pathfindingWeight = 1;
    }

    public void AddNeighbour(IAStarNode node) {
        if(!_neighbours.Contains(node)) _neighbours.Add(node);
    }

    public void RemoveNeighbour(IAStarNode node) {
        _neighbours.Remove(node);
    }

    public IAStarNode[] GetNeighbours() {
        return _neighbours.ToArray();
    }

    public void Clear() {
        if(tile != null) tile = null;
        pathfindingWeight = 1;
    }

#if UNITY_EDITOR
    public void DrawDebug(Vector2 size) {
        Vector2 start = worldPosition - size * 0.49f;
        Vector2 right = Vector2.right * size * 0.98f;
        Vector2 up = Vector2.up * size * 0.98f;
        Color color = pathfindingWeight < 0 ? Color.red : Color.white;

        Debug.DrawLine(start, start + up, color);
        Debug.DrawLine(start + up, start + up + right, color);
        Debug.DrawLine(start + up + right, start + right, color);
        Debug.DrawLine(start, start + right, color);
    }
#endif
}
