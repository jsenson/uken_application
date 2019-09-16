using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.ObjectPooling;

[RequireComponent(typeof(TileGrid))]
public class GridController : MonoBehaviour {
    public static event System.Action onAllTilesCleared;

    [SerializeField] private SpriteTile _tilePrefab = null;
    [SerializeField] private SpriteTileInfo[] _tileSet = null;

    private GameObjectPool<SpriteTile> _tilePool;
    private TileGrid _grid;

    void Awake() {
        _grid = GetComponent<TileGrid>();
        GameSettings.LevelSettings maxSettings = GameSettings.GetLevelSettings(int.MaxValue);
        _tilePool = new GameObjectPool<SpriteTile>(_tilePrefab, transform, maxSettings.gridSize.x * maxSettings.gridSize.y, 10);
    }

    void OnEnable() {
        SpriteTile.onTilesMatched += OnTilesMatched;
    }

    void OnDisable() {
        SpriteTile.onTilesMatched -= OnTilesMatched;
    }

    public void InitializeGrid() {
        _grid.Clear(_tilePool);
        GameSettings.LevelSettings settings = GameSettings.currentLevelSettings;

        _grid.ResizeGrid(settings.gridSize.x + 2, settings.gridSize.y + 2);
        int tileCount = settings.gridSize.x * settings.gridSize.y;

        SpriteTileInfo[] tiles = GetRandomTilePairs(settings.uniqueTileCount, tileCount);

        // Fill the grid leaving cells around the border empty
        for(int x = 1; x < _grid.columns - 1; x++) {
            for(int y = 1; y < _grid.rows - 1; y++) {
                int i = (y - 1) * (_grid.columns - 2) + (x - 1);

                if(tiles[i] != null) {
                    SpriteTile tile = _tilePool.Pop();
                    tile.SetSpriteTileInfo(tiles[i]);
                    tile.SetGridNode(_grid[x,y]);
                    tile.SetHighlighted(false);
                    tile.SetColliderEnabled(true);
                }
            }
        }

        FixUnwinnables();
    }

    SpriteTileInfo[] GetRandomTilePairs(int uniqueTileCount, int totalTileCount) {
        if(uniqueTileCount > _tileSet.Length) {
            Debug.LogWarning("GridController: Not enough unique tiles in tileSet.");
            uniqueTileCount = _tileSet.Length;
        }

        SpriteTileInfo[] tiles = new SpriteTileInfo[totalTileCount];
        _tileSet.Shuffle();
        int j = 0;

        for(int i = 0; i < tiles.Length - 1; i+=2) {
            SpriteTileInfo info = _tileSet[j++];
            tiles[i] = info;
            tiles[i+1] = info;
            if(j >= uniqueTileCount) j = 0;
        }

        tiles.Shuffle();

        return tiles;
    }

    void FixUnwinnables() {
        bool done = false;

        // repeat until no cross patterns are detected
        do {
            done = true;

            for(int x = 1; x < _grid.columns - 2; x++) {
                for(int y = 1; y < _grid.rows - 2; y++) {
                    // Check for 'crosses' of tiles that result in an unwinnable puzzle.
                    // [ n3, n4 ]
                    // [ n1, n2 ]
                    GridNode n1 = _grid[x,y];
                    GridNode n2 = _grid[x+1,y];
                    GridNode n3 = _grid[x,y+1];
                    GridNode n4 = _grid[x+1,y+1];

                    // Pattern is unwinnable if n1 == n4 and n2 == n3
                    if(n1.tile != null && n2.tile != null && n3.tile != null && n4.tile != null && n1.tile.Matches(n4.tile) && n2.tile.Matches(n3.tile)) {
                        // Swap n1 and n2 to move the pattern
                        SpriteTile t = n2.tile;
                        n1.tile.SetGridNode(n2);
                        t.SetGridNode(n1);
                        done = false;
                    }
                }
            }
        } while(!done);
    }

    void OnTilesMatched(SpriteTile tile1, SpriteTile tile2) {
        _tilePool.Push(tile1);
        _tilePool.Push(tile2);

        if(_grid.tileCount == 0) {
            OnAllTilesCleared();
        }
    }

    void OnAllTilesCleared() {
        if(onAllTilesCleared != null) onAllTilesCleared();
    }
}
