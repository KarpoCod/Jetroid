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

    private BlockType[,] Blocks = new BlockType[chunkWide, chunkWide];
    private BlockType[,] BgBlocks = new BlockType[chunkWide, chunkWide];

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
        BlockInfo inf = blTyp.FirstOrDefault(b => b.BT == ChunkData.Blocks[blockPos.x, blockPos.y]);
        Chunk.SetTile(blockPos, inf.Texture);
        if (inf.Additional != null) { var blockAd = Instantiate(inf.Additional, blockPos, Quaternion.identity, transform); blockAd.name = (blockPos.x + " " + blockPos.y); }
    }

    private void GenerateBlock(int x, int y)
    {	
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        if (ChunkData.Blocks[x, y] == BlockType.bgAir)return;
		
		BlockInfo inf = blTyp.FirstOrDefault(b => b.BT == ChunkData.Blocks[x, y]);
        Chunk.SetTile(blockPos, inf.Texture);
        if (inf.Additional != null) { var blockAd = Instantiate(inf.Additional, blockPos + transform.position, Quaternion.identity, transform); blockAd.name = (blockPos.x + " " + blockPos.y); }
    }

    private void GenerateBgBlock(int x, int y)
    {	
        Vector3Int blockPos = new Vector3Int(x, y, 0);

        BgChunk.SetTile(blockPos, blTyp.FirstOrDefault(b => b.BT == ChunkData.BgBlocks[x, y]).Texture);
    }

    public void DeleteBlock(Vector3Int position)
    {
        string ToFind = (position.x + " " + position.y);
        var Addition = transform.Find(ToFind);
        if (Addition != null) Destroy(Addition.gameObject);
        ChunkData.Blocks[position.x, position.y] = BlockType.bgAir;
        Chunk.SetTile(position, null);
    }
}
