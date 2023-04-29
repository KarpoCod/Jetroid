using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class ChunkRenderer : MonoBehaviour
{
    public const int chunkWide = 16;
    public const int size = 1;

    public ChunkData ChunkData;
    public WorldGen ParentWorld;

    public Tilemap Chunk;
    public Tilemap BgChunk;
    public TileBase[] tileBlocks = new TileBase[0];
    public BlockInfo[] blTyp;

    public BlockType[,] Blocks = new BlockType[chunkWide, chunkWide];
    public BlockType[,] BgBlocks = new BlockType[chunkWide, chunkWide];



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
            for (int y = 1; y < chunkWide - 2; y++)
            {
                coords = new Vector3(x + xOffset * chunkWide + 0.5f, y + yOffset * chunkWide + 0.5f, 0f);
                if (ChunkData.Blocks[x , y ] == BlockType.bgAir && ChunkData.Blocks[x + 1, y] == BlockType.bgAir 
                    && ChunkData.Blocks[x, y + 1 ] == BlockType.bgAir && ChunkData.Blocks[x + 1, y + 1] == BlockType.bgAir) return coords;
            
            }
        }
        return new Vector3(-1, -1, -1);
    }

    public void SetBlock(Vector3Int blockPos, BlockType BlockT)
    {
        ChunkData.Blocks[blockPos.x, blockPos.y] = BlockT;
        Chunk.SetTile(blockPos, blTyp.FirstOrDefault(b => b.BT == BlockT).Texture);
    }

    private void GenerateBlock(int x, int y)
    {	
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        if (ChunkData.Blocks[x, y] == BlockType.bgAir)return;
		
		BlockInfo inf = blTyp.FirstOrDefault(b => b.BT == ChunkData.Blocks[x, y]);
        Chunk.SetTile(blockPos, inf.Texture);
    }

    private void GenerateBgBlock(int x, int y)
    {	
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        BgChunk.SetTile(blockPos, blTyp.FirstOrDefault(b => b.BT == ChunkData.BgBlocks[x, y]).Texture);
    }

    public void DeleteBlock(Vector3Int position)
    {
        ChunkData.Blocks[position.x, position.y] = BlockType.bgAir;
        Chunk.SetTile(position, null);

    }
}
