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
                if(_closedList.Find(x => x.node == n) != null) continue;

                // Immediately close any nodes that aren't walkable
                if(n.pathfindingWeight < 0) {
                    _closedList.Add(new StepInfo(n, currentStep));
                    continue;
                }

                StepInfo neighbour = new StepInfo(n, currentStep);

                bool changedDirection = neighbour.direction != currentStep.direction;
                // Should make the increase to g customizable via the IAStarNode interface but just adding 1 for each step since this is a simple grid with no diagonal movement.
                neighbour.g = currentStep.g + 1;
                // Same here.  Should make the Heuristic customizable if I was making this more fully featured.
                neighbour.f = neighbour.g + CalculateH(n, target, changedDirection);
                neighbour.turns = currentStep.turns;
                if(changedDirection) neighbour.turns++;

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
    private static float CalculateH(IAStarNode source, IAStarNode target, bool directionChanged) {
        Vector2 sourcePos = source.GetPosition();
        Vector2 targetPos = target.GetPosition();

        float h = Mathf.Abs(targetPos.x - sourcePos.x) + Mathf.Abs(targetPos.y - sourcePos.y);
        if(directionChanged) h *= 100; // Add a severe penalty for changing directions to encourage the fewest possible
        
        return h;
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

        pathList.Reverse();
        return pathList.ToArray();
    }
}
