using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class ChunkRenderer : MonoBehaviour
{
    //customing
    public const int chunkWide = 16;
    public const int size = 1;

    //main data
    public ChunkData ChunkData;
    private Vector2Int pos;

    //unity
    public Tilemap Chunk;
    public Tilemap BgChunk;
    public TileBase[] tileBlocks = new TileBase[0];
    public BlockInfo[] blTyp;

    void Start()
    {
        ChunkData.Edited = new bool[chunkWide, chunkWide];
        pos = ChunkData.coords;

        for (int y=0; y < chunkWide; y++)
        {
            for(int x=0; x < chunkWide; x++)
            {
                ChunkData.Edited[x, y] = false;
                GenerateBlock(x, y);
                GenerateBgBlock(x, y);
            }
        }
    }

    private void Update()
    {
        if (Mathf.Abs(ChunkData.ParentWorld.CurrentChunk.x - pos.x) > ChunkData.ParentWorld.ChunkSpawnRad || Mathf.Abs(ChunkData.ParentWorld.CurrentChunk.y - pos.y) > ChunkData.ParentWorld.ChunkSpawnRad) gameObject.SetActive(false);
    }

    public void SetBlock(Vector3Int blockPos, BlockType BlockT)
    {
        ChunkData.Edited[blockPos.x, blockPos.y] = true;
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
        ChunkData.Edited[position.x, position.y] = true;
        var Addition = transform.Find(ToFind);
        if (Addition != null) Destroy(Addition.gameObject);
        ChunkData.Blocks[position.x, position.y] = BlockType.bgAir;
        Chunk.SetTile(position, null);
    }
}
