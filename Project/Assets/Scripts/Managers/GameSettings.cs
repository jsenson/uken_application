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
            value = Mathf.Clamp(value, 1, _maxLevel);

            if(_level != value) {
                _level = value;
                if(onLevelChanged != null) onLevelChanged(_level);
            }
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

    private static int _level = 1;
    private static int _maxLevel = 3;
    private static int _score = 0;

    // Note: Level settings are simply hard coded here to save time and effort.  A proper system would have a LevelManager that loads from ScriptableObjects created in the editor.
    public static LevelSettings GetLevelSettings(int level) {
        if(level == 1) return new LevelSettings(8, 10, 200, 6);
        else if(level == 2) return new LevelSettings(12, 10, 200, 10);
        else return new LevelSettings(15, 10, 200, 14);
    }
}
