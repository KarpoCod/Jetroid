using UnityEngine;
public class ChunkData
{
    public int seed;
    public Vector2Int coords;
    public ChunkRenderer Chunk;
    public BlockType[,] Blocks;
    public BlockType[,] BgBlocks;
    public WorldGen ParentWorld;
    public int[,] Light;
}