using System.Collections.Generic;
using UnityEngine;

public static class AStar {
    private class StepInfo {
        public float f;
        public float g;
        public int turns;
        public IAStarNode node;
        public StepInfo parent;

        public StepInfo(IAStarNode node, StepInfo parent) {
            this.node = node;
            this.parent = parent;
        }
    }

    private static List<StepInfo> _openList;
    private static List<StepInfo> _closedList;
    private static IAStarNode _previousNode;

    static AStar() {
        _openList = new List<StepInfo>();
        _closedList = new List<StepInfo>();
        _previousNode = null;
    }

    public static IAStarNode[] CalculatePath(IAStarNode source, IAStarNode target, int maxTurnsAllowed = 2) {
        if(source == null || target == null) {
            Debug.LogWarning("AStar.CalculatePath called with a null value.  Returned path will be empty.");
            return new IAStarNode[0];
        }

        // StepInfo currentStep = new StepInfo(source, null);
        _openList.Clear();
        _closedList.Clear();
        _previousNode = null;

        _openList.Add(new StepInfo(source, null));

        do {
            int currentIndex;
            StepInfo currentStep = GetLowestFScore(_openList, out currentIndex);
            Debug.LogFormat("Current Step: ({0:N}, {1:N})", currentStep.node.GetPosition().x, currentStep.node.GetPosition().y);

            if(currentStep.node == target) {
                return ExtractPath(currentStep);
            }

            _openList.RemoveAt(currentIndex);
            _closedList.Add(currentStep);

            IAStarNode[] neighbours = currentStep.node.GetNeighbours();
            Debug.Log("Neighbour count = " + neighbours.Length);
            foreach(IAStarNode n in neighbours) {
                Debug.LogFormat("Checking Neighbour: ({0:N}, {1:N})", n.GetPosition().x, n.GetPosition().y);
                if(_closedList.Find(x => x.node == n) != null) continue;

                if(n.pathfindingWeight < 0) {
                    Debug.Log("Weight < 0: Adding to closed list");
                    _closedList.Add(new StepInfo(n, currentStep));
                    continue;
                }

                StepInfo neighbour = new StepInfo(n, currentStep);

                // Should make the increase to g customizable via the IAStarNode interface but just adding 1 for each step since this is a simple grid with only diagonal movement.
                neighbour.g = currentStep.g + 1;
                // Same here.  Should make the Heuristic customizable if I was making this more fully featured.
                neighbour.f = neighbour.g + CalculateH(n, target);

                if(_openList.Find(x => x.node == neighbour.node) == null) {
                    Debug.LogFormat("Added to Open list: g = {0:N2}, f = {1:N2}", neighbour.g, neighbour.f);
                    _openList.Add(neighbour);
                }
            }
        } while(_openList.Count > 0);

        return new IAStarNode[0];
    }

    // Simple Manhatten distance heuristic
    private static float CalculateH(IAStarNode source, IAStarNode target) {
        Vector2 sourcePos = source.GetPosition();
        Vector2 targetPos = target.GetPosition();
        //return Mathf.Abs(targetPos.x - sourcePos.x) + Mathf.Abs(targetPos.y - sourcePos.y);
        return (targetPos - sourcePos).magnitude;
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
