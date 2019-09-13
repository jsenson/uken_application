using System.Collections.Generic;
using UnityEngine;

public static class AStar {
    private class StepInfo {
        public float f;
        public float g;
        public int turns;
        public IAStarNode node;
        public StepInfo parent;

        public Vector2 direction {
            get { return parent != null ? node.GetPosition() - parent.node.GetPosition() : Vector2.zero; }
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
        directionChangeCount = 0;

        if(source == null || target == null) {
            Debug.LogWarning("AStar.CalculatePath called with a null value.  Returned path will be empty.");
            return new IAStarNode[0];
        }

        _openList.Clear();
        _closedList.Clear();

        _openList.Add(new StepInfo(source, null));

        do {
            int currentIndex;
            StepInfo currentStep = GetLowestFScore(_openList, out currentIndex);

            if(currentStep.node == target) {
                // Subtract 1 from the turn count since the first move is always considered a change in direction.
                directionChangeCount = currentStep.turns - 1;
                return ExtractPath(currentStep);
            }

            _openList.RemoveAt(currentIndex);
            _closedList.Add(currentStep);

            IAStarNode[] neighbours = currentStep.node.GetNeighbours();

            foreach(IAStarNode n in neighbours) {
                if(n.pathfindingWeight < 0 || _closedList.Find(x => x.node == n) != null) continue;

                StepInfo neighbour = new StepInfo(n, currentStep);

                neighbour.g = currentStep.g + n.pathfindingWeight;
                neighbour.turns = currentStep.turns;

                if(neighbour.direction != currentStep.direction) {
                    neighbour.g += 100; // Add a severe penalty for changing directions to encourage the fewest possible
                    neighbour.turns++;
                }

                // Same here.  Should make the Heuristic customizable if I was making this more fully featured.
                neighbour.f = neighbour.g + CalculateH(n, target);

                StepInfo existingOpenStep = _openList.Find(x => x.node == neighbour.node);
                if(existingOpenStep == null) {
                    _openList.Add(neighbour);
                } else if(neighbour.f < existingOpenStep.f) {
                    // Copy values into the existing step so we don't have to traverse the List again to remove it.
                    existingOpenStep.g = neighbour.g;
                    existingOpenStep.f = neighbour.f;
                    existingOpenStep.turns = neighbour.turns;
                    existingOpenStep.parent = neighbour.parent;
                }
            }
        } while(_openList.Count > 0);

        return new IAStarNode[0];
    }

    // Simple Manhatten distance heuristic since we want to tend toward 'squared' paths.
    private static float CalculateH(IAStarNode source, IAStarNode target) {
        Vector2 sourcePos = source.GetPosition();
        Vector2 targetPos = target.GetPosition();
        return Mathf.Abs(targetPos.x - sourcePos.x) + Mathf.Abs(targetPos.y - sourcePos.y);
    }

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
}
