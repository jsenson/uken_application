using UnityEngine;

public interface IAStarNode {
    float pathfindingWeight { get; }  // Multiplier for the weight of moving to a node. Weight < 0 is considered to be impassable
    IAStarNode[] GetNeighbours();
    Vector2 GetPosition();
}
