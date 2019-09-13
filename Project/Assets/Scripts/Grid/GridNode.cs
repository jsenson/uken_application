using System.Collections.Generic;
using UnityEngine;

public class GridNode : IAStarNode {
    public float pathfindingWeight { get; set; }

    public GameObject tile { get; set; }
    private Vector2 _position;
    private List<IAStarNode> _neighbours;

    public GridNode(Vector2 position) {
        _neighbours = new List<IAStarNode>();
        _position = position;
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

    public Vector2 GetPosition() {
        return _position;
    }

    public void Clear() {
        if(tile != null) {
            GameObject.Destroy(tile);
            tile = null;
            pathfindingWeight = 1;
        }
    }
}
