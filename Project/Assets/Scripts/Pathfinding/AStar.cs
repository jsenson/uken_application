using System.Collections.Generic;
using UnityEngine;

public static class AStar {
    // Class for storing info about each step of the pathfinding.  Needs to be able to reference a pointer to itself so can't be a struct.
    private class StepInfo {
        public float f; // f score = g score + heuristic
        public float g; // g score = score representing how far you've travelled from the start point to this step
        public int turns; // Counter for the total number of changes in direction up to this point
        public IAStarNode node; // The current node for this step
        public StepInfo parent; // The previous step in the current path

        // Accessor for the current direction of travel compared to the previous step.
        // Safe to check equality on these since we're dealing with unit vectors for the most part and Unity does an approximate check between Vector values by default.
        public Vector2 direction {
            get { return parent != null ? node.coordinates - parent.node.coordinates : Vector2.zero; }
        }

        public StepInfo(IAStarNode node, StepInfo parent) {
            this.node = node;
            this.parent = parent;
        }
    }

    private static List<StepInfo> _openList;
    private static List<StepInfo> _closedList;

    static AStar() {
        _openList = new List<StepInfo>();
        _closedList = new List<StepInfo>();
    }

    public static IAStarNode[] CalculatePath(IAStarNode source, IAStarNode target, out int directionChangeCount) {
        // Use the max value for an invalid path.
        directionChangeCount = int.MaxValue;

        if(source == null || target == null) {
            Debug.LogWarning("AStar.CalculatePath called with a null value.  Returned path will be empty.");
            return new IAStarNode[0];
        }

        _openList.Clear();
        _closedList.Clear();

        // Add the first step with default values
        _openList.Add(new StepInfo(source, null));

        do {
            // Always check the step in the open list with the lowest f score first.  Save the index so we can use RemoveAt rather than traverse the list again.
            int currentIndex;
            StepInfo currentStep = GetLowestFScore(_openList, out currentIndex);

            // We're done as soon as the target node has the lowest f score in the open list
            if(currentStep.node == target) {
                // Subtract 1 from the turn count since the first move is always considered a change in direction.
                directionChangeCount = currentStep.turns - 1;
                return ExtractPath(currentStep);
            }

            // Move the node from the open to the closed list
            _openList.RemoveAt(currentIndex);
            _closedList.Add(currentStep);

            IAStarNode[] neighbours = currentStep.node.GetNeighbours();

            foreach(IAStarNode n in neighbours) {
                StepInfo neighbour = new StepInfo(n, currentStep);
                // Make sure to compare both the node and the direction in the closed list.  We need to be able to add the same node to the open list multiple times
                // from different directions in order to get the lowest number of changes in direction.
                if(n.pathfindingWeight < 0 || _closedList.Find(x => x.node == neighbour.node && x.direction == neighbour.direction) != null) continue;

                neighbour.g = currentStep.g + n.pathfindingWeight;
                neighbour.turns = currentStep.turns;

                if(neighbour.direction != currentStep.direction) {
                    // Add a severe penalty for changing directions to encourage the fewest possible turns
                    neighbour.g += 100;
                    neighbour.turns++;
                }

                neighbour.f = neighbour.g + CalculateH(n, target);
                // Check if the node already exists in the open list.  Again, make sure to check both node and direction
                StepInfo existingOpenStep = _openList.Find(x => x.node == neighbour.node && x.direction == neighbour.direction);

                if(existingOpenStep == null || existingOpenStep.direction != neighbour.direction) {
                    // Add the new step to the open list if we didn't find it.
                    _openList.Add(neighbour);
                } else if(neighbour.g < existingOpenStep.g) {
                    // Copy values into the existing step so we don't have to traverse the List again to remove it.
                    existingOpenStep.g = neighbour.g;
                    existingOpenStep.f = neighbour.f;
                    existingOpenStep.turns = neighbour.turns;
                    existingOpenStep.parent = neighbour.parent;
                }
            }
        } while(_openList.Count > 0);

        // Hit the end of the open list without finding a valid path
        return new IAStarNode[0];
    }

    // Simple Manhatten distance heuristic since we want to tend toward 'squared' paths.
    private static float CalculateH(IAStarNode source, IAStarNode target) {
        Vector2 sourcePos = source.coordinates;
        Vector2 targetPos = target.coordinates;
        return Mathf.Abs(targetPos.x - sourcePos.x) + Mathf.Abs(targetPos.y - sourcePos.y);
    }

    // Search the given list for the step with the lowest f score.  For ties, the first node found will be chosen.
    private static StepInfo GetLowestFScore(List<StepInfo> stepList, out int index) {
        index = 0;

        if(stepList.Count == 0) {
            Debug.LogWarning("AStar.GetLowestFScore called with an empty list.  Result will be null.");    
            return null;
        } else {
            StepInfo lowest = stepList[0];

            for(int i = 1, count = stepList.Count; i < count; i++) {
                if(stepList[i].f < lowest.f) {
                    lowest = stepList[i];
                    index = i;               
                }
            }

            return lowest;
        }
    }

    // Extracts the IAStarNodes into an array in reverse order
    private static IAStarNode[] ExtractPath(StepInfo step) {
        List<IAStarNode> pathList = new List<IAStarNode>();

        while(step.parent != null) {
            pathList.Add(step.node);
            step = step.parent;
        }
        pathList.Add(step.node);

        pathList.Reverse();
        return pathList.ToArray();
    }

    // Helper to log all the steps of the given path
    public static void LogPath(IAStarNode[] path, int turns) {
        var sb = new System.Text.StringBuilder("Path: ");
        sb.Append(turns);
        sb.Append(" turns:\n");

        foreach(IAStarNode node in path) {
            sb.AppendLine(node.coordinates.ToString());
        }

        Debug.Log(sb.ToString());
    }
}
