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

    public Vector2Int pos;

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

    private void Update()
    {
        if (Mathf.Abs(ParentWorld.CurrentChunk.x - pos.x) > ParentWorld.ChunkSpawnRad || Mathf.Abs(ParentWorld.CurrentChunk.y - pos.y) > ParentWorld.ChunkSpawnRad) gameObject.SetActive(false);
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
