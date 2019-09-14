using UnityEngine;

public interface IAStarNode {
    float pathfindingWeight { get; }  // Multiplier for the weight of moving to a node. Weight < 0 is considered to be impassable
    Vector2 coordinates { get; }
    IAStarNode[] GetNeighbours();
}
