using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class ChunkRenderer : MonoBehaviour
{
    public const int chunkWide = 16;
    public const int size = 1;

    public ChunkData ChunkData;
    public WorldGen ParentWorld;

    public Tilemap Chunk;
    public Tilemap BgChunk;
    public TileBase[] tileBlocks = new TileBase[0];

    public BlockType[,] Blocks = new BlockType[chunkWide, chunkWide];
    public BlockType[,] BgBlocks = new BlockType[chunkWide, chunkWide];

    private List<Vector3> verticies = new List<Vector3>();
    private List<int> triangles = new List<int>();



    void Start()
    {
        for (int y=0; y < chunkWide; y++)
        {
            for(int x=0; x < chunkWide; x++)
            {
                GenerateBlock(x, y);
                GenerateBgBlock(x, y);
            }
        }
    }

    public Vector3 setSpawn(int xOffset, int yOffset)
    {
        Vector3 coords = new Vector3(-1, -1, -1);
        for (int x = 0; x < chunkWide - 2; x++)
        {
            for (int y = 0; y < chunkWide - 2; y++)
            {
                coords = new Vector3(x + xOffset * chunkWide + 0.5f, y + yOffset * chunkWide + 0.5f, 0f);
                if (ChunkData.Blocks[x , y ] == BlockType.bgAir && ChunkData.Blocks[x + 1, y] == BlockType.bgAir 
                    && ChunkData.Blocks[x, y + 1 ] == BlockType.bgAir && ChunkData.Blocks[x + 1, y + 1] == BlockType.bgAir) return coords;
            
            }
        }
        return coords;
    }

    private void GenerateBlock(int x, int y)
    {
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        if (ChunkData.Blocks[x, y] == BlockType.bgAir)return;

        Chunk.SetTile(blockPos, tileBlocks[(int) ChunkData.Blocks[x, y]]);

    }

    private void GenerateBgBlock(int x, int y)
    {
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        BgChunk.SetTile(blockPos, tileBlocks[(int)ChunkData.BgBlocks[x, y]]);

    }

    public void DeleteBlock(Vector3Int position)
    {

        Chunk.SetTile(position, null);

    }
}
