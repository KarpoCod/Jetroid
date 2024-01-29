using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Block")]
public class BlockInfo : ScriptableObject
{
    public int ID;
    public BlockType BT;
    public int TileNumber;
    public TileBase Texture;
}
