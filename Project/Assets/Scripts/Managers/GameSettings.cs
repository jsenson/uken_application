using UnityEngine;

public static class GameSettings {
    public static event System.Action<int> onScoreChanged;
    public static event System.Action<int> onLevelChanged;

    public struct LevelSettings {
        public Vector2Int gridSize;
        public float timeLimit;
        public int uniqueTileCount;

        public LevelSettings(int width, int height, float timeLimit, int tileCount) {
            gridSize = new Vector2Int(width, height);
            this.timeLimit = timeLimit;
            uniqueTileCount = tileCount;
        }
    }

    public static int level { 
        get { return _level; }
        set {
            _level = Mathf.Clamp(value, 1, _maxLevel);
            if(onLevelChanged != null) onLevelChanged(_level);
        }
    }

    public static int score { 
        get { return _score; }
        set {
            if(_score != value) {
                _score = value;
                if(onScoreChanged != null) onScoreChanged(_score);
            }
        }
    }

    public static LevelSettings currentLevelSettings { get { return GetLevelSettings(_level); } }
    public static int maxLevel { get { return _maxLevel; } }
    public static int maxPathfindingTurns { get { return _maxPathfindingTurns; } }

    private static int _level = 1;
    private static int _maxLevel = 3;
    private static int _score = 0;
    private static int _maxPathfindingTurns = 2;

    // Note: Level settings are simply hard coded here to save time and effort.  A proper system would have a LevelManager that loads from ScriptableObjects created in the editor.
    public static LevelSettings GetLevelSettings(int level) {
        if(level == 1) return new LevelSettings(10, 10, 70, 12);
        else if(level == 2) return new LevelSettings(14, 10, 100, 16);
        else return new LevelSettings(17, 10, 180, 20);
    }
}
