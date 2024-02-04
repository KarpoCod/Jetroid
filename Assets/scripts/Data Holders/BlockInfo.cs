using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Block")]
public class BlockInfo : ScriptableObject
{
    public BlockType BT;
    public TileBase Texture;
}
