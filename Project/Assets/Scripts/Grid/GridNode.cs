using System.Collections.Generic;
using UnityEngine;

// A general node for our game Grid, specialized to hold a SpriteTile.
public class GridNode : IAStarNode {
    // Sets the weight for this node to use during pathfinding.  < 0 means the tile in't walkable, otherwise higher numbers are harder to traverse.
    public float pathfindingWeight { get; set; }

    // The current tile connected to this node.
    public SpriteTile tile { get; set; }
    // The x,y coordinated of the node in the grid
    public Vector2 coordinates { get; private set; }
    // The world position of the mode based on the position of its parent grid
    public Vector2 worldPosition { get { return _grid != null ? _grid.ConvertToWorldPosition(coordinates) : coordinates; } }
    // The world unit size of the grid cell
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
