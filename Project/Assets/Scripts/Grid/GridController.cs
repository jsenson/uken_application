using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToN.ObjectPooling;

[RequireComponent(typeof(TileGrid))]
public class GridController : MonoBehaviour {
    [SerializeField] private SpriteTile _tilePrefab = null;
    [SerializeField] private SpriteTileInfo[] _tileSet = null;

    private GameObjectPool<SpriteTile> _tilePool;
    private TileGrid _grid;

    void Awake() {
        _grid = GetComponent<TileGrid>();
        ResetGrid();
    }

    void ResetGrid() {
        if(_tilePool != null) _tilePool.Clear();
        _grid.ResizeGrid(14, 10); // TODO: Change the grid size based on level
        int tileCount = (_grid.columns - 2) * (_grid.rows - 2);
        
        _tilePool = new GameObjectPool<SpriteTile>(_tilePrefab, transform, tileCount, 10);

        // TODO: Change the number of unique tiles based on level.
        SpriteTileInfo[] tiles = GetRandomTilePairs(_tileSet.Length, tileCount);

        // Fill the grid leaving cells around the border empty
        for(int x = 1; x < _grid.columns - 1; x++) {
            for(int y = 1; y < _grid.rows - 1; y++) {
                int i = (y - 1) * (_grid.columns - 2) + (x - 1);

                if(tiles[i] != null) {
                    SpriteTile tile = _tilePool.Pop();
                    tile.SetSpriteTileInfo(tiles[i]);
                    tile.SetGridNode(_grid[x,y], _grid);
                }
            }
        }
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
}
