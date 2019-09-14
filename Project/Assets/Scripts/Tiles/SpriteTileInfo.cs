using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileInfo", menuName = "Sprite Tile Info")]
public class SpriteTileInfo : ScriptableObject {
    [SerializeField] private string _tileName = "None";
    [SerializeField] private Sprite _sprite = null;
    [SerializeField] private int _pointValue = 1;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.gray;
    
    public string tileName { get { return _tileName; } }
    public Sprite sprite { get { return _sprite; } }
    public int pointValue { get { return _pointValue; } }
    public Color highlightColor { get { return _highlightColor; } }
    public Color defaultColor { get { return _defaultColor; } }
}
